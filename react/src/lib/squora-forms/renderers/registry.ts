/**
 * Component registry — maps ComponentType to React renderer components.
 */

import type { ComponentType } from '../core/types';
import type { FieldRendererProps, LayoutRendererProps } from '../core/types';

export type RendererComponent = React.ComponentType<FieldRendererProps> | React.ComponentType<LayoutRendererProps>;

export interface ComponentRegistry {
  register(type: ComponentType, renderer: RendererComponent): void;
  get(type: ComponentType): RendererComponent | undefined;
}

export function createRegistry(): ComponentRegistry {
  const map = new Map<ComponentType, RendererComponent>();

  return {
    register(type: ComponentType, renderer: RendererComponent) {
      map.set(type, renderer);
    },
    get(type: ComponentType): RendererComponent | undefined {
      return map.get(type);
    },
  };
}
