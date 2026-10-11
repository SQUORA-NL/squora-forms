# Typed Data Binding — Primitive List Sample

This sample demonstrates working with a list of primitive values (strings). A `ListContainer` object holds a `Name` string and a `Names` list of strings. The user can add items to the list via `AddAction` and delete individual items via `DeleteAction`. The `@` path notation refers to the current list item.

## Razor declarations

```html
<SampleDocumentation MarkDownFile="Samples.Typed.ListSample.razor.md" Data="@data" FormModel="@model">
    <BDForm FormModel="@model" Components="@UserSettingsService.Components" Value="@data" ValueChanged="@Update"></BDForm>
</SampleDocumentation>
```

## Code

```csharp
private void Update()
{
    StateHasChanged();
}

private DynamicFormModel model = FormFactory.GroupLayout(gr => gr
        .WithLabel("List sample"),
        FormFactory.StringComponent("Name", "$.Name", name => name.WithMinimumLength(3)),
        FormFactory.AddAction("Add", "$.Names", "zero"),
        FormFactory.ListComponent("Names", "$.Names",
             FormFactory.HorizontalLayout(
                 FormFactory.StringComponent("Text", "@"),
                 FormFactory.DeleteAction("Delete", "@")
             )
        )
       );

private ListContainer data = new ListContainer()
{
    Name = "Demo of primitive list",
    Names = new List<string> { "one", "two", "three" }
};
```
