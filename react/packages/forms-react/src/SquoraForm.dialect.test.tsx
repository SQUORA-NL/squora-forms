import { describe, expect, it } from 'vitest';
import { renderToString } from 'react-dom/server';
import { SquoraForm } from './SquoraForm';

const schema = { type: 'object' as const, properties: { fullName: { type: 'string' as const } }, required: ['fullName'] };

describe('SquoraForm', () => {
  it('renders a definition written the Blazor / designer way, same as the camelCase one', () => {
    const blazor = {
      DynamicType: { ComponentType: 'VerticalLayout' },
      Elements: [{ DynamicType: { ComponentType: 'String' }, Parameters: { Path: '$.fullName', Label: 'Full name' } }],
    };
    const camel = {
      dynamicType: { componentType: 'VerticalLayout' },
      elements: [{ dynamicType: { componentType: 'String' }, parameters: { scope: '$.fullName', label: 'Full name' } }],
    };

    const a = renderToString(<SquoraForm schema={schema} uiSchema={blazor as never} />);
    const b = renderToString(<SquoraForm schema={schema} uiSchema={camel as never} />);

    expect(a).toContain('Full name');
    expect(a).toContain('name="fullName"');
    expect(a).toBe(b);
  });
});
