# Squora.Forms.Blazor

Blazor renderer base for Squora Forms: form component, layout and state services.

```bash
dotnet add package Squora.Forms.Blazor
```

Install a UI framework package (HTML, MudBlazor, Radzen or Syncfusion) to get concrete components.

## The form editor

`<BDForm EditMode="true" ShowEditModeToggle="true" ... />` turns a form into an editor: a Components panel on the left, the form in the middle, the properties of the selected element on the right.

- **Add**: drag a component onto the form, or click it. A click adds it inside the selected layout, after the selected field, or at the end of the form; the panel says which.
- **Move**: drag an element by its handle. While dragging, every layout shows a dashed outline, every possible spot a line, and the spot under the pointer says where the drop lands ("Drop above City", "Into Address").
- **Edit**: click an element. Its properties open on the right (the pane opens by itself if it was hidden); double click, or the sliders button on the element, moves the cursor to its first field. `Esc` lets go of the selection.
- **Actions on the selected element**: edit properties, select the layout it is in, duplicate (the copy gets its own paths) and delete.

The editor works with every UI framework package, and a form can be shown with any of them: pass the `ComponentsList` of the one you want as `Components`.

### Colours

The editor takes its colours from CSS variables, so one rule themes all of it. All are optional.

```css
.my-editor {
    --sq-accent: #7c3aed;       /* selection, drop targets, buttons */
    --sq-surface: #fff;         /* panels */
    --sq-surface-2: #f1f5f9;    /* the canvas behind the form */
    --sq-paper: #fff;           /* the form's own page */
    --sq-border: #dbe1ea;
    --sq-text: #1e293b;
    --sq-muted: #64748b;
    --sq-danger: #dc2626;
    --sq-sticky-top: 72px;      /* keep the side panels below a fixed app bar */
    --sq-toolbox-width: 224px;
    --sq-pane-width: 332px;
}
```

Part of [Squora Forms](https://github.com/SQUORA-NL/squora-forms) - open-source, schema-driven forms for Blazor and React. MIT licensed.
