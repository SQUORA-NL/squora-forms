using Squora.Forms.Blazor.HtmlComponents.Extensions;
using Squora.Forms.Blazor.MudBlazorComponents.Extensions;
using Squora.Forms.Blazor.RadzenComponents.Extensions;
using Squora.Forms.Common.Enums;
using Squora.Forms.Core.Models;
using MudBlazorServerApp.Shared.FormExtentions;

namespace MudBlazorServerApp.Shared;

/// <summary>A set of form components the same form can be shown with.</summary>
public record FormFramework(string Key, string Name, string Description, ComponentsList Components);

/// <summary>
/// The component sets the editor can be switched between. A form is only a description of fields, so the same
/// form (and the same data) renders with any of them.
/// </summary>
public static class FormFrameworks
{
    public static readonly IReadOnlyList<FormFramework> All = new List<FormFramework>
    {
        new("mudblazor", "MudBlazor", "Material design components", MudBlazor()),
        new("html", "HTML", "Plain HTML inputs, no UI library", new HtmlComponentsProvider().GetComponents()),
        new("radzen", "Radzen", "Radzen Blazor components", new RadzenComponentProvider().GetComponents()),
    };

    public static FormFramework Default => All[0];

    public static FormFramework Find(string? key) => All.FirstOrDefault(f => f.Key == key) ?? Default;

    private static ComponentsList MudBlazor()
    {
        var components = new MudBlazorComponentProvider().GetComponents();
        // a group that collapses; the other component sets show a "Collapsable" group as a plain group
        components[new ComponentSelectionKey(ComponentType.GroupLayout, "Collapsable")] = typeof(CollapsableGroup);
        return components;
    }
}
