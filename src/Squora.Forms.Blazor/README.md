# Squora.Forms.Blazor

Blazor renderer base for Squora Forms: form component, layout and state services.

```bash
dotnet add package Squora.Forms.Blazor
```

Install a UI framework package (HTML, MudBlazor, Radzen or Syncfusion) to get concrete components.

## Class, style and width of an element

Every element of a form can carry its own settings, in the form definition (JSON) or with the builders:

- **Class**: extra CSS classes on that one element (`WithClass("highlight")`), so your own CSS styles just that textbox.
- **Style**: an inline style on the element's wrapper (`WithStyle("...")`).
- **Width**: in a horizontal layout, the share of the row an element takes: `"10%"` next to `"90%"` fills the row, or a fixed size such as `"200px"` (`WithWidth("10%")`). Elements without a width share what is left.

A custom component gets all of this when it draws itself inside a `BDFormComponentWrapper`.

## Editing forms

This package only renders and validates forms. Editing them, the drag and drop, selecting an element and its properties, is what the
**Squora Forms Designer** does (a separate product). It plugs in through one extension point: an `IFormEditSurface` registered as a service.
It tells the runtime which component draws the wrapper around every element while a form is being edited (`ElementWrapperType`, derived from
`ElementWrapperBase`) and which one shows the empty-layout placeholder (`EmptyLayoutType`). With none registered, a form renders as it always
did, and `EditMode` changes nothing.

The `BDForm` draws a `bd-stage` and a `bd-sheet` around every form (what it is on, and its own page), and takes `StageClass` and `SheetClass`
to add to them, so themes and editors have something to hold on to.

Part of [Squora Forms](https://github.com/SQUORA-NL/squora-forms) - open-source, schema-driven forms for Blazor and React. MIT licensed.
