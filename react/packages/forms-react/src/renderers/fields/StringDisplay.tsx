'use client';

export interface StringDisplayProps {
  label: string;
  value: unknown;
}

export function StringDisplay({ label, value }: StringDisplayProps) {
  return (
    <div className="sq-field sq-string-display" style={{ padding: '4px 0' }}>
      {label} {value !== undefined && value !== null ? String(value) : ''}
    </div>
  );
}
