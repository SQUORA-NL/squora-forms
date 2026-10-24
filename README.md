# Squora Forms

Schema-driven dynamic forms. Describe a form once as JSON (JSON Schema + UI Schema + rules) and render it with Blazor (HTML, MudBlazor, Radzen, Syncfusion) or React.

- **Open source, MIT** - model, renderers, AI adapters and samples.
- The visual **Squora Forms Designer** (drag-and-drop editor) is a separate, licensed product.

## Packages

| Package | Purpose |
|---|---|
| `Squora.Forms.Core` | Framework-neutral form model, rules and parsing (no Blazor dependency) |
| `Squora.Forms.UISchema`, `.JsonSchema`, `.Common`, `.Contracts` | Schema building blocks used by Core |
| `Squora.Forms.Blazor` | Blazor renderer base |
| `Squora.Forms.Blazor.Components`, `.Actions`, `.Extensions` | Component abstractions, form actions, DI registration |
| `Squora.Forms.Blazor.HtmlComponents` / `.MudBlazorComponents` / `.RadzenComponents` / `.SyncfusionComponents` | UI framework renderers |
| `Squora.Forms.Ai.Mcp`, `.Ai.OpenAiTools` | Expose forms to AI agents (MCP, tool-calling) |

Syncfusion components require your own Syncfusion license.

## File uploads

`FileUpload` is a field like any other, in Blazor (HTML, MudBlazor, Radzen, Syncfusion) and React. The user can pick one
file, or several with `Multiple`, and the form data holds each file as JSON with its content Base64 encoded:

```json
{ "cv": { "name": "cv.pdf", "contentType": "application/pdf", "size": 5, "data": "aGVsbG8=" } }
```

In JSON Schema an object with `"format": "squora-file"` is one file and an array of those objects is several files
(`maxItems`, `accept` and `maxFileSize` are read from it). In C#: `FormFactory.FileUploadComponent("CV", "$.cv", b => b.WithMultiple())`;
typed models use `UploadedFile` or `List<UploadedFile>`. See the *File upload* sample.

## Quick start (Blazor)

```bash
dotnet add package Squora.Forms.Blazor.MudBlazorComponents
```

```csharp
using Squora.Forms.Blazor.Extensions;
using Squora.Forms.Blazor.MudBlazorComponents.Extensions;

builder.Services.AddSquoraForms(cfg => cfg.UseDefaultMudBlazorComponents());
```

See `samples/` for runnable apps (`cd samples/MudBlazorServerApp && dotnet run`) and `react/` for the React renderer demo.

## Repository layout

```
src/        libraries (NuGet packages)
samples/    Blazor sample apps
react/      React/Next.js renderer + demo
tst/        tests
```

## Contributing

Issues and pull requests are welcome. Run `dotnet test SquoraForms.sln` before submitting.

## License

MIT - see [LICENSE](LICENSE).
