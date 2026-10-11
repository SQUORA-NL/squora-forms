'use client';

export interface NumberDisplayProps {
  label: string;
  value: unknown;
}

export function NumberDisplay({ label, value }: NumberDisplayProps) {
  let display: string;
  if (Array.isArray(value)) {
    display = String(value.length);
  } else if (typeof value === 'number') {
    display = String(value);
  } else {
    display = '—';
  }

  return (
    <div
      className="ff-field ff-number-display"
      style={{ fontWeight: 600, padding: '4px 0' }}
    >
      {label} {display}
    </div>
  );
}
