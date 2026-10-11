'use client';

import type { FieldRendererProps } from '../../core/types';

export function UnknownField({ name, model }: FieldRendererProps) {
  const componentType = model.dynamicType?.componentType || 'unknown';

  if (typeof console !== 'undefined') {
    console.warn(
      `[SquoraForm] No renderer registered for component type "${componentType}" (field: "${name}")`
    );
  }

  return (
    <div
      className="ff-field ff-unknown-field"
      style={{
        border: '2px solid #e5a100',
        backgroundColor: '#fff8e1',
        padding: '8px 12px',
        borderRadius: '4px',
        margin: '4px 0',
      }}
    >
      <strong>Unknown field type:</strong> {componentType}
      {name && <span> (field: {name})</span>}
    </div>
  );
}
