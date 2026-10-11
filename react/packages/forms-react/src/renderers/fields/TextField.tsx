'use client';

import type { FieldRendererProps } from '../../core/types';

export function TextField({
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
  const isMultiline = model.parameters?.multiline === true;

  return (
    <div className="sq-field sq-text-field">
      <label htmlFor={name}>{label}</label>
      {isMultiline ? (
        <textarea
          id={name}
          name={name}
          value={(value as string) ?? ''}
          disabled={disabled}
          aria-invalid={hasErrors}
          aria-describedby={hasErrors ? errorId : undefined}
          onChange={(e) => onChange(e.target.value)}
          onBlur={onBlur}
          rows={4}
        />
      ) : (
        <input
          type="text"
          id={name}
          name={name}
          value={(value as string) ?? ''}
          disabled={disabled}
          aria-invalid={hasErrors}
          aria-describedby={hasErrors ? errorId : undefined}
          onChange={(e) => onChange(e.target.value)}
          onBlur={onBlur}
        />
      )}
      {hasErrors && (
        <span id={errorId} className="sq-error" role="alert">
          {errors.join(', ')}
        </span>
      )}
    </div>
  );
}
