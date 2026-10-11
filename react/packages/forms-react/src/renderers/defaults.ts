/**
 * Default registry — maps all built-in ComponentTypes to their renderers.
 */

import { ComponentType } from '../core/types';
import { createRegistry } from './registry';
import { TextField } from './fields/TextField';
import { NumberField } from './fields/NumberField';
import { BooleanField } from './fields/BooleanField';
import { SelectField } from './fields/SelectField';
import { DateField } from './fields/DateField';
import { UnknownField } from './fields/UnknownField';
import { HorizontalLayout } from './layouts/HorizontalLayout';
import { VerticalLayout } from './layouts/VerticalLayout';
import { GroupLayout } from './layouts/GroupLayout';

export function createDefaultRegistry() {
  const registry = createRegistry();

  // Field renderers
  registry.register(ComponentType.String, TextField);
  registry.register(ComponentType.Int, NumberField);
  registry.register(ComponentType.Number, NumberField);
  registry.register(ComponentType.Boolean, BooleanField);
  registry.register(ComponentType.Dropdown, SelectField);
  registry.register(ComponentType.DateTime, DateField);

  // Display renderers (read-only variants — use same components for MVP)
  registry.register(ComponentType.StringDisplay, TextField);
  registry.register(ComponentType.NumberDisplay, NumberField);

  // Layout renderers
  registry.register(ComponentType.HorizontalLayout, HorizontalLayout);
  registry.register(ComponentType.VerticalLayout, VerticalLayout);
  registry.register(ComponentType.GroupLayout, GroupLayout);

  // Fallback for unknown types is handled in SquoraForm via UnknownField
  // We explicitly register Default to UnknownField
  registry.register(ComponentType.Default, UnknownField);

  return registry;
}
