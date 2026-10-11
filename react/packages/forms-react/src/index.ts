// Public API — @squora/forms-react

// Main component
export { SquoraForm } from './SquoraForm';
export type { SquoraFormProps } from './SquoraForm';

// Hook
export { useSquoraForm } from './useSquoraForm';
export type { UseSquoraFormOptions } from './useSquoraForm';

// Registry
export { createRegistry } from './renderers/registry';
export { createDefaultRegistry } from './renderers/defaults';
export type { ComponentRegistry } from './renderers/registry';

// Context
export { useListItemContext, ListItemProvider } from './context/ListItemContext';
export type { ListItemContextValue } from './context/ListItemContext';

// New renderers
export { ListLayout } from './renderers/layouts/ListLayout';
export { AddAction } from './actions/AddAction';
export { DeleteAction } from './actions/DeleteAction';
export { NumberDisplay } from './renderers/fields/NumberDisplay';
export { StringDisplay } from './renderers/fields/StringDisplay';

// Core types
export {
  ComponentType,
  LAYOUT_TYPES,
  ACTION_TYPES,
} from './core/types';

export type {
  DynamicFormModel,
  ComponentSelectionKey,
  DynamicFormModelOptions,
  DynamicFormModelRule,
  RuleCondition,
  RuleEffect,
  JSONSchema,
  FieldRendererProps,
  LayoutRendererProps,
  FormState,
} from './core/types';

// Scope utilities
export {
  resolveScopePath,
  isRelativePath,
  resolveRelativeScope,
} from './core/scope';
