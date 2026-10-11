'use client';

import type { FieldRendererProps } from '../../core/types';

export function NumberField({
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

  return (
    <div className="sq-field sq-number-field">
      <label htmlFor={name}>{label}</label>
      <input
        type="number"
        id={name}
        name={name}
        value={value !== undefined && value !== null ? String(value) : ''}
        min={schema.minimum}
        max={schema.maximum}
        disabled={disabled}
        aria-invalid={hasErrors}
        aria-describedby={hasErrors ? errorId : undefined}
        onChange={(e) => {
          const raw = e.target.value;
          if (raw === '') {
            onChange(undefined);
          } else {
            const num = Number(raw);
            onChange(Number.isNaN(num) ? raw : num);
          }
        }}
        onBlur={onBlur}
      />
      {hasErrors && (
        <span id={errorId} className="sq-error" role="alert">
          {errors.join(', ')}
        </span>
      )}
    </div>
  );
}
