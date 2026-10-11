'use client';

import { createContext, useContext } from 'react';
import type { JSONSchema, FormState } from '../core/types';
import type { ComponentRegistry } from '../renderers/registry';

export interface SquoraFormContextValue {
  state: FormState;
  schema: JSONSchema;
  registry: ComponentRegistry;
  disabled: boolean;
  submitLabel: string;
  changeField: (name: string, value: unknown) => void;
  touchField: (name: string) => void;
  addToArray: (path: string, defaultValue: unknown) => void;
  removeFromArray: (path: string, index: number) => void;
}

const SquoraFormContext = createContext<SquoraFormContextValue | null>(null);

export function useSquoraFormContext(): SquoraFormContextValue {
  const ctx = useContext(SquoraFormContext);
  if (!ctx) throw new Error('useSquoraFormContext must be used inside SquoraForm');
  return ctx;
}

export const SquoraFormProvider = SquoraFormContext.Provider;
