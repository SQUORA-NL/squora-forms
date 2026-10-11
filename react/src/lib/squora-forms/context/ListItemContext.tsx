'use client';

import { createContext, useContext } from 'react';

export interface ListItemContextValue {
  basePath: string;
  index: number;
  onArrayAdd: (path: string, defaultValue: unknown) => void;
  onArrayRemove: (path: string, index: number) => void;
}

const ListItemContext = createContext<ListItemContextValue | null>(null);

export function useListItemContext(): ListItemContextValue | null {
  return useContext(ListItemContext);
}

export const ListItemProvider = ListItemContext.Provider;

export default ListItemContext;
