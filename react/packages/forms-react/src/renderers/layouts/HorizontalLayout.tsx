'use client';

import type { LayoutRendererProps } from '../../core/types';

export function HorizontalLayout({
  children,
  visible,
}: LayoutRendererProps) {
  if (!visible) return null;

  return (
    <div
      className="sq-layout sq-horizontal-layout"
      // flex-end: buttons beside inputs (Delete, Add) line up with the input fields, not with the labels above them
      style={{ display: 'flex', gap: '16px', flexWrap: 'wrap', alignItems: 'flex-end' }}
    >
      {children}
    </div>
  );
}
