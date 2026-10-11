'use client';

import type { FieldRendererProps } from '../../core/types';

export function BooleanField({
  name,
  value,
  model,
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
    <div className="sq-field sq-boolean-field">
      <label htmlFor={name}>
        <input
          type="checkbox"
          id={name}
          name={name}
          checked={Boolean(value)}
          disabled={disabled}
          aria-invalid={hasErrors}
          aria-describedby={hasErrors ? errorId : undefined}
          onChange={(e) => onChange(e.target.checked)}
          onBlur={onBlur}
        />
        {' '}{label}
      </label>
      {hasErrors && (
        <span id={errorId} className="sq-error" role="alert">
          {errors.join(', ')}
        </span>
      )}
    </div>
  );
}
