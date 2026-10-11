'use client';

import type { LayoutRendererProps } from '../../core/types';

export function GroupLayout({
  model,
  children,
  visible,
}: LayoutRendererProps) {
  if (!visible) return null;

  const label = model.options?.label || (model.parameters?.label as string);

  return (
    <fieldset className="ff-layout ff-group-layout">
      {label && <legend>{label}</legend>}
      <div style={{ display: 'flex', flexDirection: 'column', gap: 'var(--ff-gap, 12px)' }}>
        {children}
      </div>
    </fieldset>
  );
}
