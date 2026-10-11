'use client';

import type { LayoutRendererProps } from '../../core/types';

export function HorizontalLayout({
  children,
  visible,
}: LayoutRendererProps) {
  if (!visible) return null;

  return (
    <div
      className="ff-layout ff-horizontal-layout"
      style={{ display: 'flex', gap: '16px', flexWrap: 'wrap' }}
    >
      {children}
    </div>
  );
}
