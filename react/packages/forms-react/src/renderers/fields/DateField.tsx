'use client';

import type { FieldRendererProps } from '../../core/types';

export function DateField({
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
  const inputType = schema.format === 'date-time' ? 'datetime-local' : 'date';

  return (
    <div className="sq-field sq-date-field">
      <label htmlFor={name}>{label}</label>
      <input
        type={inputType}
        id={name}
        name={name}
        value={(value as string) ?? ''}
        disabled={disabled}
        aria-invalid={hasErrors}
        aria-describedby={hasErrors ? errorId : undefined}
        onChange={(e) => onChange(e.target.value || undefined)}
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
