'use client';

import React, { useCallback, useEffect, useMemo, useRef } from 'react';
import type {
  DynamicFormModel,
  JSONSchema,
  FieldRendererProps,
  LayoutRendererProps,
} from './core/types';
import { ComponentType, LAYOUT_TYPES } from './core/types';
import {
  resolveScopePath,
  getValueAtPath,
  isRelativePath,
  resolveRelativeScope,
} from './core/scope';
import { resolveVisibility, resolveEnabled } from './core/rules';
import { validateForm } from './core/validation';
import { useFormState } from './core/state';
import { createDefaultRegistry } from './renderers/defaults';
import { UnknownField } from './renderers/fields/UnknownField';
import { NumberDisplay } from './renderers/fields/NumberDisplay';
import { StringDisplay } from './renderers/fields/StringDisplay';
import { ListLayout } from './renderers/layouts/ListLayout';
import { AddAction } from './actions/AddAction';
import { DeleteAction } from './actions/DeleteAction';
import { useListItemContext } from './context/ListItemContext';
import {
  useSquoraFormContext,
  SquoraFormProvider,
} from './context/SquoraFormContext';
import type { ComponentRegistry } from './renderers/registry';

// ── Helper: resolve scope with optional list context ──

function resolveScope(
  scope: string,
  listContext: { basePath: string; index: number } | null
): string {
  if (!scope) return '';
  if (isRelativePath(scope) && listContext) {
    return resolveRelativeScope(scope, listContext.basePath, listContext.index);
  }
  return resolveScopePath(scope);
}

function getFieldSchema(absolutePath: string, schema: JSONSchema): JSONSchema {
  if (!absolutePath || !schema.properties) return {};
  const segments = absolutePath.split('.');
  let current: JSONSchema = schema;
  for (const segment of segments) {
    if (/^\d+$/.test(segment)) {
      if (current.items) current = current.items;
      continue;
    }
    if (current.properties && current.properties[segment]) {
      current = current.properties[segment];
    } else {
      return {};
    }
  }
  return current;
}

function hasSubmitAction(model: DynamicFormModel): boolean {
  if (model.dynamicType?.componentType === ComponentType.SubmitAction)
    return true;
  return (model.elements || []).some(hasSubmitAction);
}

// ── RenderNode — stable top-level component (FIX-001) ──

function RenderNode({
  model,
  nodeKey,
}: {
  model: DynamicFormModel;
  nodeKey: string;
}) {
  const ctx = useSquoraFormContext();
  const listCtx = useListItemContext();

  if (!model || !model.dynamicType) return null;

  const componentType = model.dynamicType.componentType;
  const visible = resolveVisibility(model.rules, ctx.state.values);
  const enabled = !ctx.disabled && resolveEnabled(model.rules, ctx.state.values);

  if (!visible) return null;

  const scope = (model.parameters?.scope as string) || '';

  // --- List ---
  if (componentType === ComponentType.List) {
    const arrayPath = resolveScope(scope, listCtx);
    const arr = getValueAtPath(ctx.state.values, arrayPath);
    const items = Array.isArray(arr) ? arr : [];

    return (
      <ListLayout
        model={model}
        items={items}
        arrayPath={arrayPath}
        visible={visible}
        onArrayAdd={ctx.addToArray}
        onArrayRemove={ctx.removeFromArray}
        renderNode={(child, childKey) => (
          <RenderNode key={childKey} model={child} nodeKey={childKey} />
        )}
      />
    );
  }

  // --- Layout types (non-List) ---
  if (LAYOUT_TYPES.has(componentType)) {
    const Renderer = ctx.registry.get(componentType) as
      | React.ComponentType<LayoutRendererProps>
      | undefined;

    const children = (model.elements || []).map((child, idx) => (
      <RenderNode
        key={`${nodeKey}-${idx}`}
        model={child}
        nodeKey={`${nodeKey}-${idx}`}
      />
    ));

    if (Renderer) {
      return (
        <Renderer model={model} visible={visible}>
          {children}
        </Renderer>
      );
    }
    return <div>{children}</div>;
  }

  // --- SubmitAction ---
  if (componentType === ComponentType.SubmitAction) {
    return (
      <button type="submit" disabled={!enabled} className="ff-submit-button">
        {(model.parameters?.label as string) || ctx.submitLabel}
      </button>
    );
  }

  // --- AddAction ---
  if (componentType === ComponentType.AddAction) {
    const targetPath = resolveScope(scope, listCtx);
    const defaultValue = model.parameters?.defaultValue ?? {};
    return (
      <AddAction
        label={(model.parameters?.label as string) || 'Add'}
        visible={visible}
        disabled={!enabled}
        onAdd={() => ctx.addToArray(targetPath, defaultValue)}
      />
    );
  }

  // --- DeleteAction ---
  if (componentType === ComponentType.DeleteAction) {
    if (!listCtx) return null;
    return (
      <DeleteAction
        label={(model.parameters?.label as string) || 'Delete'}
        visible={visible}
        disabled={!enabled}
        onDelete={() => ctx.removeFromArray(listCtx.basePath, listCtx.index)}
      />
    );
  }

  // --- NumberDisplay ---
  if (componentType === ComponentType.NumberDisplay) {
    const displayPath = resolveScope(scope, listCtx);
    const displayValue = getValueAtPath(ctx.state.values, displayPath);
    return (
      <NumberDisplay
        label={(model.parameters?.label as string) || ''}
        value={displayValue}
      />
    );
  }

  // --- StringDisplay ---
  if (componentType === ComponentType.StringDisplay) {
    const displayPath = resolveScope(scope, listCtx);
    const displayValue = getValueAtPath(ctx.state.values, displayPath);
    return (
      <StringDisplay
        label={(model.parameters?.label as string) || ''}
        value={displayValue}
      />
    );
  }

  // --- Regular field types ---
  const fieldName = resolveScope(scope, listCtx);

  if (!fieldName) {
    const unknownProps: FieldRendererProps = {
      name: nodeKey,
      value: undefined,
      model,
      schema: {},
      errors: [],
      disabled: !enabled,
      onChange: () => {},
      onBlur: () => {},
    };
    return <UnknownField {...unknownProps} />;
  }

  const fieldSchema = getFieldSchema(fieldName, ctx.schema);
  const fieldValue = getValueAtPath(ctx.state.values, fieldName);
  const fieldErrors =
    ctx.state.touched[fieldName] && ctx.state.errors[fieldName]
      ? ctx.state.errors[fieldName]
      : [];

  const fieldProps: FieldRendererProps = {
    name: fieldName,
    value: fieldValue,
    model,
    schema: fieldSchema,
    errors: fieldErrors,
    disabled: !enabled,
    onChange: (value: unknown) => ctx.changeField(fieldName, value),
    onBlur: () => ctx.touchField(fieldName),
  };

  const Renderer = ctx.registry.get(componentType) as
    | React.ComponentType<FieldRendererProps>
    | undefined;

  if (Renderer) {
    return <Renderer {...fieldProps} />;
  }

  return <UnknownField {...fieldProps} />;
}

// ── SquoraForm — public component ──

export interface SquoraFormProps {
  schema: JSONSchema;
  uiSchema: DynamicFormModel;
  initialValues?: Record<string, unknown>;
  onSubmit?: (data: Record<string, unknown>) => void;
  onChange?: (
    data: Record<string, unknown>,
    errors: Record<string, string[]>
  ) => void;
  submitLabel?: string;
  disabled?: boolean;
  registry?: ComponentRegistry;
}

export function SquoraForm({
  schema,
  uiSchema,
  initialValues,
  onSubmit,
  onChange,
  submitLabel = 'Submit',
  disabled = false,
  registry: externalRegistry,
}: SquoraFormProps) {
  const registryRef = useRef(externalRegistry ?? createDefaultRegistry());
  const {
    state,
    changeField,
    touchField,
    setErrors,
    submitForm,
    addToArray,
    removeFromArrayAction,
  } = useFormState(initialValues);

  useEffect(() => {
    onChange?.(state.values, state.errors);
  }, [state.values, state.errors, onChange]);

  const revalidate = useCallback(
    (values: Record<string, unknown>) => {
      const errors = validateForm(values, schema);
      setErrors(errors);
      return errors;
    },
    [schema, setErrors]
  );

  const handleSubmit = useCallback(
    (e: React.FormEvent) => {
      e.preventDefault();
      submitForm();
      const errors = revalidate(state.values);
      const hasErrors = Object.keys(errors).length > 0;
      if (!hasErrors && onSubmit) {
        onSubmit(state.values);
      }
    },
    [state.values, onSubmit, submitForm, revalidate]
  );

  // Error guards
  if (!uiSchema || !uiSchema.dynamicType) {
    return (
      <div className="ff-error-state" style={{ border: '2px solid #d32f2f', backgroundColor: '#ffebee', padding: '16px', borderRadius: '4px' }}>
        <strong>SquoraForm Error:</strong> Invalid or missing uiSchema.
      </div>
    );
  }
  if (!schema || typeof schema !== 'object') {
    return (
      <div className="ff-error-state" style={{ border: '2px solid #d32f2f', backgroundColor: '#ffebee', padding: '16px', borderRadius: '4px' }}>
        <strong>SquoraForm Error:</strong> Invalid or missing JSON Schema.
      </div>
    );
  }

  const ctxValue = useMemo(
    () => ({
      state,
      schema,
      registry: registryRef.current,
      disabled,
      submitLabel,
      changeField,
      touchField,
      addToArray,
      removeFromArray: removeFromArrayAction,
    }),
    [state, schema, disabled, submitLabel, changeField, touchField, addToArray, removeFromArrayAction]
  );

  const showDefaultSubmit = !hasSubmitAction(uiSchema);

  return (
    <SquoraFormProvider value={ctxValue}>
      <form onSubmit={handleSubmit} noValidate className="ff-form">
        <RenderNode model={uiSchema} nodeKey="root" />
        {showDefaultSubmit && (
          <button
            type="submit"
            disabled={disabled}
            className="ff-submit-button"
            style={{ marginTop: '16px' }}
          >
            {submitLabel}
          </button>
        )}
      </form>
    </SquoraFormProvider>
  );
}
