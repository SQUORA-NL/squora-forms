import { describe, expect, it } from 'vitest';
import { normalizeModel } from './normalize';

describe('normalizeModel', () => {
  it('reads a definition written the Blazor / designer way', () => {
    const model = normalizeModel({
      DynamicType: { ComponentType: 'VerticalLayout' },
      Elements: [
        { DynamicType: { ComponentType: 'String' }, Parameters: { Path: '$.name', Label: 'Name', MinimumLength: 2 } },
        { DynamicType: { ComponentType: 'Dropdown' }, Parameters: { Path: '$.team', Options: { Platform: 'Platform Team' } } },
      ],
    });

    expect(model).toEqual({
      dynamicType: { componentType: 'VerticalLayout' },
      elements: [
        { dynamicType: { componentType: 'String' }, parameters: { scope: '$.name', label: 'Name', minimumLength: 2 } },
        { dynamicType: { componentType: 'Dropdown' }, parameters: { scope: '$.team', options: { Platform: 'Platform Team' } } },
      ],
    });
  });

  it('leaves a camelCase definition unchanged', () => {
    const model = {
      dynamicType: { componentType: 'String' },
      parameters: { scope: '$.a', label: 'A', options: { X: 'x' } },
      elements: [],
    };
    expect(normalizeModel(model)).toEqual(model);
  });

  it('never renames data: option values, default values and rule schemas', () => {
    const model = normalizeModel({
      DynamicType: { ComponentType: 'String' },
      Parameters: { Path: '$.a', DefaultValue: { Name: 'Ada' }, Options: { Key: 'Label' } },
      Rules: [{ Effect: 'HIDE', Condition: { Scope: '$.b', Schema: { properties: { Name: { const: 'x' } } } } }],
    });

    expect(model.parameters).toEqual({ scope: '$.a', defaultValue: { Name: 'Ada' }, options: { Key: 'Label' } });
    expect(model.rules).toEqual([{ effect: 'HIDE', condition: { scope: '$.b', schema: { properties: { Name: { const: 'x' } } } } }]);
  });
});
