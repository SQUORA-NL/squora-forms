# Spec 001 — Display and data components, named templates, React parity

**Repo**: squora-forms (open source, MIT)
**Status**: TODO (draft, not started)
**Written**: 2026-10-31
**Why now**: the Morphrix platform front office wants to render record pages (lists, headers, detail bodies, tables of
versions and positions, status badges, charts) from a JSON definition, and it needs a renderer that is extendable
without a core change per screen. Squora Forms is the candidate. A follow-up spec in the Morphrix repo
(`docs/design/squora-forms-front-office.md`) adopts the result there; it depends on this one.

This spec is written after reading the code, not the README. What was found is in "Current state"; every requirement
below closes a gap listed there.

---

## 1. Current state (verified in the code, 2026-10-31)

### Model
- `ComponentType` has 22 values, in `src/Squora.Forms.Common/Enums/ComponentType.cs` and mirrored by hand as a closed
  `as const` union in `react/packages/forms-react/src/core/types.ts`:
  String, Boolean, Int, Number, Dropdown, HorizontalLayout, VerticalLayout, Array, Default, Object, Categorization,
  GroupLayout, Category, List, DeleteAction, AddAction, SubmitAction, DateTime, NumberDisplay, StringDisplay,
  TemplateContent, FileUpload.
- A model node is `DynamicFormModel`: `DynamicType` = `ComponentSelectionKey(ComponentType, TypeDefinitionName)`,
  `Parameters`, `Elements`, `DynamicFormModelOptions` (label, radioGroup, readonly, showSortButtons,
  elementLabelProperty), `Rules` (SHOW, HIDE, DISABLE, ENABLE with a JSON Schema condition on a scope).
- The key is a pair, so a component can be selected by type **and name**. Blazor uses this for custom components:
  `ComponentsList.Add(new ComponentSelectionKey(ComponentType.TemplateContent, "intro"), typeof(MyComponent))`, used in
  a model as `FormFactory.TemplateDisplay("intro", "$")`. The component receives the form data at the path.

### Blazor
- Four renderer packages (Html, MudBlazor, Radzen, Syncfusion), each with a provider that lists what it renders. The
  Html provider renders: String, DateTime, FileUpload, Horizontal/Vertical/GroupLayout, Number, Int, Boolean,
  Dropdown, List, Delete/Add/SubmitAction, NumberDisplay, StringDisplay.
- No default renderer for **Array, Object, Categorization, Category, TemplateContent**. The enum has them; nothing
  draws them. TemplateContent works only through a custom registration.
- List: `ListComponentBase` iterates items with an iterator path (`$.cars[0]`), element template per item, add and
  delete actions. It is a form list (editable items), not a table.

### React (`@squora/forms-react` 1.0.0)
- The registry (`renderers/registry.ts`) is a `Map<ComponentType, Renderer>`. **It ignores `typeDefinitionName`**, so
  named custom components do not work in React. `createRegistry` and `createDefaultRegistry` are exported and
  `<SquoraForm registry>` accepts one, so replacing the renderer of a built-in type works.
- `SquoraForm.tsx` hard-codes the branches for List, SubmitAction, AddAction, DeleteAction, NumberDisplay and
  StringDisplay; they are not registry-driven and cannot be replaced.
- Registered: String, Int, Number, Boolean, Dropdown, DateTime, FileUpload, StringDisplay (as TextField), NumberDisplay
  (as NumberField), Horizontal/Vertical/GroupLayout, Default (UnknownField).
- Not rendered: Categorization, Category, Object (they are in `LAYOUT_TYPES` and fall back to a bare `<div>`), Array,
  TemplateContent.
- `ListLayout` is a form list with inline styles; no table, sort, paging, empty-state theming or item label.
- The TS `JSONSchema` type is a Draft-07 subset (no `$ref`, `oneOf`, `allOf`).
- Definitions can be written the Blazor way (PascalCase, `Path`); `core/normalize.ts` maps them.

### Not in the library at all
Table, tabs, formatted read-only values (date, date range, money, status badge, badges, link), key-value facts, card or
section with a title, a generic action button that raises an event, and anything chart-like.

### Conventions found
- Tests: `tst/Squora.Forms.Tests` (xUnit, schemas under `DynamicFormSchemas` and `Schemas`), React tests next to the
  code (`SquoraForm.dialect.test.tsx`, `normalize.test.ts`).
- Samples: `samples/SharedDemos/Samples/Components/<Name>Sample.razor` + `.razor.md`; React demo
  `react/demo/src/samples/*.json` (schema, ui-schema, data).
- One definition must render in Blazor and React (see commit "one definition for Blazor and React"); rule-effects were
  recently aligned. Keep that.

---

## 2. Goals

1. **Named templates work in React** exactly as in Blazor, with no core change per custom component.
2. **A small, generic set of display and data components**, in Blazor and React from one definition, enough to build a
   record page: table, formatted values, facts, badge, link, section, tabs, nested object, action button.
3. **Everything registry-driven in React**, including List and actions, so a host can replace any renderer.
4. **Read-only mode** for a whole form or a subtree.
5. The same definition keeps rendering in the Designer; the Designer gets the new blocks in its own spec (non-goal
   here).

### Non-goals
- A chart library. Charts are templates a host registers (goal 1). A tiny example template with SVG may ship in the
  React demo; the core takes no charting dependency.
- Editable table cells (a later spec; a table here is read-only plus row link and row actions).
- Server-side paging. Paging, sort and filter are client-side over the array in the data.
- A new rules language. Existing SHOW/HIDE/DISABLE/ENABLE stay.

---

## 3. Requirements

Numbering is stable; tasks in section 5 refer to these.

### R1 Named templates and component names (React first, then verify Blazor)
- R1.1 `registry.register(type, renderer, name?)` and `registry.get(type, name?)`: lookup by `(type, name)`, falling
  back to `(type)` when the name is not registered. `ComponentSelectionKey.typeDefinitionName` is passed from the node.
- R1.2 `TemplateContent` has a default renderer. Props: `name`, `path`, `value` (data at the path), `data` (whole form
  data), `parameters` (node parameters), `readonly`, `onChange(path, value)`. An unregistered name renders a visible
  notice ("Template 'x' is not registered"), never a blank or a crash.
- R1.3 Named overrides for any type work, e.g. `(String, "meterPicker")`, in React and Blazor.
- R1.4 `ComponentType` becomes extendable in TS without a cast: `ComponentType | (string & {})` for custom types, with
  the built-in constants kept. (Decision needed, see O1.)
- R1.5 Definition format is identical to Blazor's (`TypeDefinitionName` / `typeDefinitionName`).

### R2 Registry-driven React
- R2.1 List, SubmitAction, AddAction, DeleteAction, NumberDisplay and StringDisplay are looked up in the registry like
  every other type; the current code becomes their default renderers.
- R2.2 Categorization and Category render as tabs by default (an accessible tab list; the Category label is the tab).
  Object renders as a titled group of its elements bound to the object path.
- R2.3 No change in behaviour for existing definitions (the dialect and sample tests stay green).

### R3 Table
- R3.1 New `ComponentType.Table`, bound to an array path. Columns are `Elements` (each a display node with a path
  relative to the row) or `Parameters.columns`; choose one in the plan (O2).
- R3.2 Column: `field` (dotted path), `label`, `format`, `align`, `link` (a template, e.g. `/meter-points/{ean}`),
  `template` (text with `{path}` placeholders), `width`, `sortable`.
- R3.3 Client-side sort (click header), page size and pager, empty text, optional row link, optional row actions
  (buttons that raise `onAction(id, row)`).
- R3.4 Rows are read-only. Nested arrays are not required.
- R3.5 Works inside a List item and inside a Category; honours rules (SHOW/HIDE) on the table node.
- R3.6 Accessible: real `<table>`, headers with `scope`, sort state announced.

### R4 Formatted values (Display)
- R4.1 `Display` component type with a `format` parameter, replacing the need for per-format types. Formats: `text`,
  `number` (min/max fraction digits, unit), `date`, `dateTime`, `dateRange` (second path), `money` (currency from a path
  or parameter), `percent`, `status` (coloured badge from a value→tone map), `badge`, `badges` (array), `keys`
  (object keys), `link` (template + label), `json` (pretty-printed fallback).
- R4.2 Locale and currency come from the host (`SquoraFormProvider` / `BDForm` option), with a safe default.
- R4.3 `StringDisplay` and `NumberDisplay` keep working and are documented as the simple cases of `Display`.
- R4.4 A missing value renders an empty marker (configurable), never "undefined" or "null".

### R5 Facts, section, action button
- R5.1 `Facts`: a row of label/value pairs (a record header), items are Display nodes with labels.
- R5.2 `Section` (card): title, description, optional collapsible; contains elements.
- R5.3 `ActionButton`: label, icon name (host maps names to icons), `actionId`; raises `onAction(actionId, data)`;
  honours rules and `disabled`; no form behaviour of its own.
- R5.4 `SubmitAction` stays; `ActionButton` is for host-defined actions.

### R6 Read-only mode
- R6.1 Form-level `readonly` (React prop, Blazor parameter) renders every input as its Display equivalent without
  inputs, and hides Add, Delete and Submit. `DynamicFormModelOptions.readonly` on a node does the same for a subtree.
- R6.2 Validation is not shown in read-only mode.

### R7 Parity, tests, docs, samples
- R7.1 Every new type renders in Blazor (Html provider first; Mud, Radzen, Syncfusion in later increments, marked
  per package in the plan) and React from one definition.
- R7.2 A shared definition fixture per type under `tst/.../DynamicFormSchemas` and `react/demo/src/samples`, used by
  both test suites (.NET parse + render, React render).
- R7.3 A sample page and `.razor.md` per Blazor type; a demo sample per type in the React demo.
- R7.4 README of Core and of the React package list the full component set and how to register named components.
- R7.5 Version: React `1.1.0`; NuGet minor bump. The change log names the new types.

---

## 4. Definition shapes (target, to be confirmed in the plan)

```json
{ "DynamicType": { "ComponentType": "Table" },
  "Parameters": { "Path": "$.positions", "PageSize": 24, "EmptyText": "No positions",
    "Columns": [
      { "Field": "position", "Label": "#", "Format": "number", "Align": "right" },
      { "Field": "quantity", "Label": "kWh", "Format": "number", "Unit": "kWh", "Align": "right" },
      { "Field": "origin",   "Label": "Origin", "Format": "badge" },
      { "Field": "validationStatus", "Label": "Status", "Format": "status",
        "Tones": { "VLD": "ok", "NVL": "danger", "MNL": "warn" } } ] } }
```

```json
{ "DynamicType": { "ComponentType": "TemplateContent", "TypeDefinitionName": "series-chart" },
  "Parameters": { "Path": "$.positions", "Height": 220 } }
```

```json
{ "DynamicType": { "ComponentType": "Display" },
  "Parameters": { "Path": "$.date", "Label": "Day", "Format": "date" } }
```

Host registration (React):

```ts
const registry = createDefaultRegistry();
registry.register(ComponentType.TemplateContent, SeriesChart, 'series-chart');
<SquoraForm schema={schema} uiSchema={ui} initialValues={data} registry={registry} readonly />
```

---

## 5. Tasks

Order matters: T1 unblocks the Morphrix work; T2 to T7 can follow in parallel pairs.

- [ ] **T1 Named templates in React** (R1.1, R1.2, R1.3, R1.5). Registry keyed by (type, name); pass
      `typeDefinitionName`; `TemplateContent` renderer; unregistered-name notice. Tests: named hit, fallback to type,
      miss notice. Demo sample "template". Blazor: confirm (type, name) override for non-template types, test it.
- [ ] **T2 Registry-driven React** (R2.1, R2.3). Move List, actions and displays into registry defaults. Existing tests
      unchanged and green.
- [ ] **T3 Tabs and Object** (R2.2). Categorization/Category → tabs, Object → group, in React; Blazor Html provider
      renderers for the same three (they have none today). Samples and fixtures.
- [ ] **T4 Display** (R4). Core model (parameters), React and Blazor Html renderers, locale/currency option, formats
      listed in R4.1, missing-value marker. Fixture per format.
- [ ] **T5 Table** (R3). Parameter model decided (O2), React and Blazor Html, sort, paging, links, row actions,
      accessibility. Fixture with 96 rows and with an empty array.
- [ ] **T6 Facts, Section, ActionButton** (R5). React and Blazor Html.
- [ ] **T7 Read-only mode** (R6). Form and subtree. Tests: no inputs rendered, actions hidden, no validation shown.
- [ ] **T8 Component enum and TS type** (R1.4). Apply decision O1, keep source compatibility of the .NET enum (append
      only).
- [ ] **T9 Other Blazor renderer packages** (R7.1): MudBlazor, Radzen, Syncfusion for T3 to T6; one PR per package.
- [ ] **T10 Docs, samples, versions** (R7.3 to R7.5).
- [ ] **T11 Follow-up specs** (not done here): Designer palette entries and property editors for the new types (repo
      squora-forms-designer, next free number after 022); MCP and OpenAI adapters learn the new types.

### Acceptance (whole spec)
1. A definition with Facts + Tabs (Details, Positions, Versions) + a Table of 96 rows + a named `series-chart`
   template renders the same structure in Blazor (Html) and React from one JSON file, read-only.
2. A host registers `series-chart` in React with no change to `@squora/forms-react`.
3. All existing tests in `tst/` and the React package pass unchanged; `SquoraForm.dialect.test.tsx` included.
4. Table sorts, pages, links and shows an empty state; keyboard and screen-reader checks pass for table and tabs.

---

## 6. Open questions

- **O1** Extendable type: widen the TS type to `ComponentType | (string & {})`, or keep it closed and require named
  `TemplateContent` for everything custom? Recommendation: widen, keep constants.
- **O2** Table columns as child `Elements` (reuses the Designer's tree editing, heavier JSON) or as
  `Parameters.Columns` (compact, needs its own property editor)? Recommendation: `Parameters.Columns`, one editor.
- **O3** Display formats: one `Display` type with `format`, or separate types? Recommendation: one type.
- **O4** Package publishing for the React package (npm public, GitHub Packages, or workspace link) so the Morphrix
  front office can consume 1.1.0. Needed before the Morphrix spec starts.
- **O5** Locale handling: provider-level only, or per-node override? Recommendation: provider-level plus `Currency`
  and `Unit` per node.
- **O6** Does the Designer need the new types before the Morphrix adoption, or is hand-written JSON acceptable at
  first? Recommendation: hand-written first.
