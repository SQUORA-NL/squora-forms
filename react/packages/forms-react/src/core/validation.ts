/**
 * JSON Schema validation using ajv.
 * Zero React dependencies.
 */

import Ajv, { type ErrorObject } from 'ajv';
import type { JSONSchema } from './types';

const ajv = new Ajv({ allErrors: true, strict: false });

/**
 * Convert an ajv error to a human-readable message.
 */
function formatError(err: ErrorObject): string {
  const field = err.instancePath ? err.instancePath.replace(/^\//, '') : '';
  const prefix = field ? `${field}: ` : '';

  switch (err.keyword) {
    case 'required':
      return `${(err.params as { missingProperty: string }).missingProperty} is required`;
    case 'minLength':
      return `${prefix}must be at least ${(err.params as { limit: number }).limit} characters`;
    case 'maxLength':
      return `${prefix}must be at most ${(err.params as { limit: number }).limit} characters`;
    case 'minimum':
      return `${prefix}must be at least ${(err.params as { limit: number }).limit}`;
    case 'maximum':
      return `${prefix}must be at most ${(err.params as { limit: number }).limit}`;
    case 'pattern':
      return `${prefix}does not match the required pattern`;
    case 'enum':
      return `${prefix}must be one of the allowed values`;
    case 'const':
      return `${prefix}must equal ${JSON.stringify((err.params as { allowedValue: unknown }).allowedValue)}`;
    case 'type':
      return `${prefix}must be of type ${(err.params as { type: string }).type}`;
    default:
      return err.message || `${prefix}validation failed`;
  }
}

/**
 * Replace tokens in a custom validation message template with actual schema values.
 */
export function formatCustomMessage(
  template: string,
  schema: JSONSchema
): string {
  return template
    .replace(/\{MinimumLength\}/g, String(schema.minLength ?? ''))
    .replace(/\{MaximumLength\}/g, String(schema.maxLength ?? ''))
    .replace(/\{Minimum\}/g, String(schema.minimum ?? ''))
    .replace(/\{Maximum\}/g, String(schema.maximum ?? ''))
    .replace(/\{Pattern\}/g, schema.pattern ?? '');
}

/**
 * Validate a single field value against a JSON Schema property definition.
 * Returns an array of error messages (empty = valid).
 * If customMessage is provided, uses it instead of default ajv messages.
 */
export function validateField(
  value: unknown,
  fieldSchema: JSONSchema,
  customMessage?: string
): string[] {
  if (!fieldSchema) return [];

  const validate = ajv.compile(fieldSchema);
  const valid = validate(value);

  if (valid) return [];

  if (customMessage) {
    return [formatCustomMessage(customMessage, fieldSchema)];
  }

  return (validate.errors || []).map(formatError);
}

/**
 * Validate all form values against a full JSON Schema.
 * Returns errors keyed by field name.
 */
export function validateForm(
  values: Record<string, unknown>,
  schema: JSONSchema
): Record<string, string[]> {
  const errors: Record<string, string[]> = {};

  if (!schema || !schema.properties) return errors;

  const required = new Set(schema.required || []);

  for (const [fieldName, fieldSchema] of Object.entries(schema.properties)) {
    const value = values[fieldName];
    const fieldErrors: string[] = [];

    // Check required
    if (required.has(fieldName)) {
      if (value === undefined || value === null || value === '') {
        fieldErrors.push(`${fieldName} is required`);
      }
    }

    // Skip further validation if empty and not required
    if (
      value === undefined ||
      value === null ||
      value === ''
    ) {
      if (fieldErrors.length > 0) {
        errors[fieldName] = fieldErrors;
      }
      continue;
    }

    // Validate against field schema
    const schemaErrors = validateField(value, fieldSchema);
    fieldErrors.push(...schemaErrors);

    if (fieldErrors.length > 0) {
      errors[fieldName] = fieldErrors;
    }
  }

  return errors;
}
