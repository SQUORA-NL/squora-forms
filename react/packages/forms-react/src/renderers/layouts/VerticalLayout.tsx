'use client';

import type { LayoutRendererProps } from '../../core/types';

export function VerticalLayout({
  children,
  visible,
}: LayoutRendererProps) {
  if (!visible) return null;

  return (
    <div
      className="sq-layout sq-vertical-layout"
      style={{ display: 'flex', flexDirection: 'column', gap: '12px' }}
    >
      {children}
    </div>
  );
}
