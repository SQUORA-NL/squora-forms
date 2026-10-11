'use client';

import { useMemo, useCallback } from 'react';
import type { JSONSchema, DynamicFormModel } from './core/types';
import { useFormState } from './core/state';
import { validateForm } from './core/validation';
import { SquoraForm } from './SquoraForm';
import type { SquoraFormProps } from './SquoraForm';
import type { ComponentRegistry } from './renderers/registry';

export interface UseSquoraFormOptions {
  schema: JSONSchema;
  uiSchema: DynamicFormModel;
  initialValues?: Record<string, unknown>;
  onSubmit?: (data: Record<string, unknown>) => void;
  onChange?: (
    data: Record<string, unknown>,
    errors: Record<string, string[]>
  ) => void;
  registry?: ComponentRegistry;
}

export function useSquoraForm(options: UseSquoraFormOptions) {
  const { schema, uiSchema, initialValues, onSubmit, onChange, registry } =
    options;

  const { state, changeField, touchField, setErrors, submitForm, resetForm } =
    useFormState(initialValues);

  const isValid = useMemo(
    () => Object.keys(state.errors).length === 0,
    [state.errors]
  );

  const submit = useCallback(() => {
    submitForm();
    const errors = validateForm(state.values, schema);
    setErrors(errors);
    const hasErrors = Object.keys(errors).length > 0;
    if (!hasErrors && onSubmit) {
      onSubmit(state.values);
    }
  }, [state.values, schema, onSubmit, submitForm, setErrors]);

  const reset = useCallback(
    (values?: Record<string, unknown>) => {
      resetForm(values);
    },
    [resetForm]
  );

  // Pre-bound SquoraForm component
  const BoundSquoraForm = useMemo(() => {
    const Bound = (overrides: Partial<SquoraFormProps>) => (
      <SquoraForm
        schema={schema}
        uiSchema={uiSchema}
        initialValues={initialValues}
        onSubmit={onSubmit}
        onChange={onChange}
        registry={registry}
        {...overrides}
      />
    );
    Bound.displayName = 'BoundSquoraForm';
    return Bound;
  }, [schema, uiSchema, initialValues, onSubmit, onChange, registry]);

  return {
    values: state.values,
    errors: state.errors,
    isValid,
    submit,
    reset,
    changeField,
    touchField,
    SquoraForm: BoundSquoraForm,
  };
}
