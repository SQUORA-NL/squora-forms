# @squora/forms-react

Render [Squora Forms](https://github.com/SQUORA-NL/squora-forms) definitions - JSON Schema + UI Schema + rules - as React forms. The same definition also renders in Blazor via the `Squora.Forms.*` NuGet packages.

## Install

```bash
npm install @squora/forms-react
```

Peer dependency: `react >= 18`.

## Use

```tsx
import { SquoraForm } from '@squora/forms-react';
import '@squora/forms-react/styles.css';

export function ContactForm() {
  return (
    <SquoraForm
      schema={schema}        // JSON Schema (draft-07 subset)
      uiSchema={uiSchema}    // Squora Forms UI Schema
      initialValues={{ name: '' }}
      onSubmit={(values) => console.log(values)}
    />
  );
}
```

See `react/demo` in the repository for complete examples (lists, groups, rules, themes).

## Styling

`styles.css` ships a default theme driven by `--sq-*` CSS variables. Set `data-theme="default" | "dark" | "compact"` on a parent element, or override the variables.

## Custom components

`createRegistry` / `createDefaultRegistry` let you swap or add field and layout renderers.

## License

MIT
