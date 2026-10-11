// The Blazor packages and the Squora designer write a form definition (DynamicFormModel) with PascalCase keys
// (DynamicType.ComponentType, Elements, Parameters.Path). This package reads camelCase keys and `scope`. Both describe the
// same model, so SquoraForm accepts either: this turns the .NET spelling into the one the renderers use.
// A model that is already camelCase passes through unchanged.
//
// Only the structure is renamed. Values are data and are never touched: the contents of Options, DefaultValue and the
// JSON Schema inside a rule condition keep their keys exactly as written.

import type { DynamicFormModel } from './types';

function lowerFirst(key: string): string {
  return key.charAt(0).toLowerCase() + key.slice(1);
}

function isObject(v: unknown): v is Record<string, unknown> {
  return typeof v === 'object' && v !== null && !Array.isArray(v);
}

function shallowRename(node: Record<string, unknown>, rename: (key: string) => string): Record<string, unknown> {
  const out: Record<string, unknown> = {};
  for (const [key, value] of Object.entries(node)) out[rename(key)] = value;
  return out;
}

function normalizeRule(rule: unknown): unknown {
  if (!isObject(rule)) return rule;
  const out = shallowRename(rule, lowerFirst);
  if (isObject(out.condition)) {
    out.condition = shallowRename(out.condition, lowerFirst); // condition.schema is a JSON Schema: left as written
  }
  return out;
}

function normalizeNode(node: unknown): unknown {
  if (!isObject(node)) return node;
  const out: Record<string, unknown> = {};
  for (const [key, value] of Object.entries(node)) {
    const name = lowerFirst(key);
    if (name === 'parameters' && isObject(value)) {
      // Blazor calls a field's location Path, this package calls it scope. Same JSON path ("$.name") either way.
      out[name] = shallowRename(value, (k) => (lowerFirst(k) === 'path' ? 'scope' : lowerFirst(k)));
    } else if (name === 'elements' && Array.isArray(value)) {
      out[name] = value.map(normalizeNode);
    } else if (name === 'rules' && Array.isArray(value)) {
      out[name] = value.map(normalizeRule);
    } else if ((name === 'dynamicType' || name === 'options') && isObject(value)) {
      out[name] = shallowRename(value, lowerFirst);
    } else {
      out[name] = value;
    }
  }
  return out;
}

/** Accepts a DynamicFormModel written either way and returns the camelCase / `scope` form the renderers read. */
export function normalizeModel(model: unknown): DynamicFormModel {
  return normalizeNode(model) as DynamicFormModel;
}
