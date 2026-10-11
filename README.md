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
