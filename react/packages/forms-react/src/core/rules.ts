/**
 * Rule evaluation for SHOW/HIDE/ENABLE/DISABLE conditional logic.
 * Zero React dependencies.
 */

import Ajv from 'ajv';
import type { DynamicFormModelRule } from './types';
import { resolveScopePath, getValueAtPath } from './scope';

const ajv = new Ajv({ strict: false });

/**
 * Evaluate whether a single rule's condition matches the current form values.
 * Returns true if the condition matches.
 */
export function evaluateRule(
  rule: DynamicFormModelRule,
  formValues: Record<string, unknown>
): boolean {
  if (!rule.condition || !rule.condition.scope || !rule.condition.schema) {
    return false;
  }

  const fieldPath = resolveScopePath(rule.condition.scope);
  const value = getValueAtPath(formValues, fieldPath);

  try {
    const validate = ajv.compile(rule.condition.schema);
    return validate(value) as boolean;
  } catch {
    return false;
  }
}

/**
 * Resolve the visibility state for a model node based on its rules.
 * Default: visible (true) when no rules are present.
 *
 * SHOW rules: visible only when condition matches.
 * HIDE rules: hidden when condition matches.
 */
export function resolveVisibility(
  rules: DynamicFormModelRule[] | undefined,
  formValues: Record<string, unknown>
): boolean {
  if (!rules || rules.length === 0) return true;

  for (const rule of rules) {
    const matches = evaluateRule(rule, formValues);
    if (rule.effect === 'SHOW') return matches;
    if (rule.effect === 'HIDE') return !matches;
  }

  return true;
}

/**
 * Resolve the enabled state for a model node based on its rules.
 * Default: enabled (true) when no rules are present.
 *
 * ENABLE rules: enabled only when condition matches.
 * DISABLE rules: disabled when condition matches.
 */
export function resolveEnabled(
  rules: DynamicFormModelRule[] | undefined,
  formValues: Record<string, unknown>
): boolean {
  if (!rules || rules.length === 0) return true;

  for (const rule of rules) {
    const matches = evaluateRule(rule, formValues);
    if (rule.effect === 'ENABLE') return matches;
    if (rule.effect === 'DISABLE') return !matches;
  }

  return true;
}
