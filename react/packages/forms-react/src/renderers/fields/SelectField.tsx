'use client';

import type { FieldRendererProps } from '../../core/types';

export function SelectField({
  name,
  value,
  model,
  schema,
  errors,
  disabled,
  onChange,
  onBlur,
}: FieldRendererProps) {
  const label =
    model.options?.label ||
    (model.parameters?.label as string) ||
    name;
  const errorId = `${name}-error`;
  const hasErrors = errors.length > 0;

  // Support both parameters.options dict and schema.enum
  const paramOptions = model.parameters?.options as Record<string, string> | undefined;
  const optionEntries: { key: string; label: string }[] = [];

  if (paramOptions && typeof paramOptions === 'object') {
    for (const [k, v] of Object.entries(paramOptions)) {
      optionEntries.push({ key: k, label: String(v) });
    }
  } else if (schema.enum) {
    for (const opt of schema.enum) {
      optionEntries.push({ key: String(opt), label: String(opt) });
    }
  }

  return (
    <div className="sq-field sq-select-field">
      <label htmlFor={name}>{label}</label>
      <select
        id={name}
        name={name}
        value={(value as string) ?? ''}
        disabled={disabled}
        aria-invalid={hasErrors}
        aria-describedby={hasErrors ? errorId : undefined}
        onChange={(e) => onChange(e.target.value || undefined)}
        onBlur={onBlur}
      >
        <option value="">-- Select --</option>
        {optionEntries.map((opt) => (
          <option key={opt.key} value={opt.key}>
            {opt.label}
          </option>
        ))}
      </select>
      {hasErrors && (
        <span id={errorId} className="sq-error" role="alert">
          {errors.join(', ')}
        </span>
      )}
    </div>
  );
}
