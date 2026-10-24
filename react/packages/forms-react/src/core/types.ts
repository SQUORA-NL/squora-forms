// Core TypeScript types mirroring the Squora Forms DynamicFormModel (shared with the .NET packages)
// Zero React dependencies — pure TypeScript

export const ComponentType = {
  String: 'String',
  Boolean: 'Boolean',
  Int: 'Int',
  Number: 'Number',
  Dropdown: 'Dropdown',
  HorizontalLayout: 'HorizontalLayout',
  VerticalLayout: 'VerticalLayout',
  Array: 'Array',
  Default: 'Default',
  Object: 'Object',
  Categorization: 'Categorization',
  GroupLayout: 'GroupLayout',
  Category: 'Category',
  List: 'List',
  DeleteAction: 'DeleteAction',
  AddAction: 'AddAction',
  SubmitAction: 'SubmitAction',
  DateTime: 'DateTime',
  NumberDisplay: 'NumberDisplay',
  StringDisplay: 'StringDisplay',
  TemplateContent: 'TemplateContent',
  FileUpload: 'FileUpload',
} as const;

export type ComponentType = (typeof ComponentType)[keyof typeof ComponentType];

export interface ComponentSelectionKey {
  componentType: ComponentType;
  typeDefinitionName?: string;
}

export interface DynamicFormModelOptions {
  label?: string;
  radioGroup?: boolean;
  readonly?: boolean;
  showSortButtons?: boolean;
  elementLabelProperty?: string;
}

export type RuleEffect = 'HIDE' | 'SHOW' | 'DISABLE' | 'ENABLE';

export interface RuleCondition {
  scope: string;
  schema: Record<string, unknown>;
}

export interface DynamicFormModelRule {
  effect: RuleEffect;
  condition: RuleCondition;
}

export interface DynamicFormModel {
  dynamicType: ComponentSelectionKey;
  parameters?: Record<string, unknown>;
  elements?: DynamicFormModel[];
  options?: DynamicFormModelOptions;
  rules?: DynamicFormModelRule[];
}

// JSON Schema types (Draft-07 subset used by Squora Forms definitions)

export interface JSONSchema {
  type?: 'string' | 'number' | 'integer' | 'boolean' | 'array' | 'object';
  title?: string;
  description?: string;
  format?: string;
  /** File upload ("format": "squora-file"): accepted file types and the largest file in bytes. */
  accept?: string;
  maxFileSize?: number;
  enum?: unknown[];
  const?: unknown;
  minimum?: number;
  maximum?: number;
  minLength?: number;
  maxLength?: number;
  pattern?: string;
  required?: string[];
  properties?: Record<string, JSONSchema>;
  items?: JSONSchema;
  default?: unknown;
}

// Renderer prop contracts

export interface FieldRendererProps {
  name: string;
  value: unknown;
  model: DynamicFormModel;
  schema: JSONSchema;
  errors: string[];
  disabled: boolean;
  onChange: (value: unknown) => void;
  onBlur: () => void;
}

export interface LayoutRendererProps {
  model: DynamicFormModel;
  children: React.ReactNode;
  visible: boolean;
}

// Form state types

export interface FormState {
  values: Record<string, unknown>;
  errors: Record<string, string[]>;
  touched: Record<string, boolean>;
}

export type FormAction =
  | { type: 'CHANGE'; payload: { name: string; value: unknown } }
  | { type: 'TOUCH'; payload: { name: string } }
  | { type: 'SUBMIT' }
  | { type: 'SET_ERRORS'; payload: { errors: Record<string, string[]> } }
  | { type: 'RESET'; payload?: { values: Record<string, unknown> } }
  | { type: 'ARRAY_ADD'; payload: { path: string; defaultValue: unknown } }
  | { type: 'ARRAY_REMOVE'; payload: { path: string; index: number } };

// Layout component types for registry dispatch
export const LAYOUT_TYPES: ReadonlySet<string> = new Set([
  ComponentType.HorizontalLayout,
  ComponentType.VerticalLayout,
  ComponentType.GroupLayout,
  ComponentType.Categorization,
  ComponentType.Category,
  ComponentType.Object,
  ComponentType.List,
]);

export const ACTION_TYPES: ReadonlySet<string> = new Set([
  ComponentType.SubmitAction,
  ComponentType.DeleteAction,
  ComponentType.AddAction,
]);
