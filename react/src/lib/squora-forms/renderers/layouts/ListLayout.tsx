'use client';

import React from 'react';
import type { DynamicFormModel } from '../../core/types';
import { ListItemProvider } from '../../context/ListItemContext';

export interface ListLayoutProps {
  model: DynamicFormModel;
  items: unknown[];
  arrayPath: string;
  visible: boolean;
  onArrayAdd: (path: string, defaultValue: unknown) => void;
  onArrayRemove: (path: string, index: number) => void;
  renderNode: (model: DynamicFormModel, key: string) => React.ReactNode;
}

export function ListLayout({
  model,
  items,
  arrayPath,
  visible,
  onArrayAdd,
  onArrayRemove,
  renderNode,
}: ListLayoutProps) {
  if (!visible) return null;

  const label =
    model.options?.label || (model.parameters?.label as string);
  const template = model.elements || [];

  return (
    <div className="ff-layout ff-list-layout">
      {label && (
        <div className="ff-list-label" style={{ fontWeight: 600, marginBottom: '8px' }}>
          {label}
        </div>
      )}
      {items.map((_, idx) => (
        <ListItemProvider
          key={`${arrayPath}-${idx}`}
          value={{
            basePath: arrayPath,
            index: idx,
            onArrayAdd,
            onArrayRemove,
          }}
        >
          <div className="ff-list-item">
            {template.map((child, childIdx) =>
              renderNode(child, `${arrayPath}-${idx}-${childIdx}`)
            )}
          </div>
        </ListItemProvider>
      ))}
      {items.length === 0 && (
        <div style={{ color: '#999', fontStyle: 'italic', padding: '8px 0' }}>
          No items. Use the Add button to create one.
        </div>
      )}
    </div>
  );
}
