import { describe, expect, it } from 'vitest';
import { renderToString } from 'react-dom/server';
import { SquoraForm } from '../../SquoraForm';

const file = {
  type: 'object' as const,
  format: 'squora-file',
  properties: {
    name: { type: 'string' as const },
    data: { type: 'string' as const },
  },
};
const schema = {
  type: 'object' as const,
  properties: { cv: file, attachments: { type: 'array' as const, maxItems: 3, items: file } },
};
const uiSchema = {
  dynamicType: { componentType: 'VerticalLayout' },
  elements: [
    { dynamicType: { componentType: 'FileUpload' }, parameters: { scope: '$.cv', label: 'CV', accept: '.pdf' } },
    { dynamicType: { componentType: 'FileUpload' }, parameters: { scope: '$.attachments', label: 'Attachments' } },
  ],
};

describe('FileUpload field', () => {
  const html = renderToString(<SquoraForm schema={schema} uiSchema={uiSchema as never} />);

  it('renders a file input, taking accept from the definition', () => {
    expect(html).toContain('type="file"');
    expect(html).toContain('accept=".pdf"');
  });

  it('allows several files when the schema property is an array of files, and only one otherwise', () => {
    const inputs = html.match(/<input[^>]*type="file"[^>]*>/g) ?? [];
    expect(inputs).toHaveLength(2);
    expect(inputs[0]).not.toContain('multiple');
    expect(inputs[1]).toContain('multiple');
  });

  it('lists files that are already in the data', () => {
    const withData = renderToString(
      <SquoraForm
        schema={schema}
        uiSchema={uiSchema as never}
        initialValues={{ cv: { name: 'cv.pdf', contentType: 'application/pdf', size: 2048, data: 'aGk=' } }}
      />
    );
    expect(withData).toContain('cv.pdf');
    expect(withData).toContain('2.0 KB');
  });
});
