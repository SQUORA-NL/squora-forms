# Horizontal

### Parameters

- none

### Width of an element in the row

Every element in a horizontal layout can have a `Width`: a percentage (`"10%"`, its share of the row; `"10%"` next to `"90%"` fills the row exactly) or a fixed size (`"200px"`). Elements without one share what is left equally. Set it with `WithWidth`, the **Width in the row** field in the editor, or `"Width"` in the JSON. It works for every component, a templated one included, as long as that component draws itself inside a `BDFormComponentWrapper`.

```csharp
FormFactory.HorizontalLayout(
    FormFactory.StringComponent("10%", "$.small", b => b.WithWidth("10%")),
    FormFactory.StringComponent("90%", "$.large", b => b.WithWidth("90%")));
```

## Sample code

````csharp
<BDForm FormModel="@model" Components="@UserSettingsService.Components" Value="@data" ValueChanged="@update"></BDForm>


@code {

    private void update()
    {
        StateHasChanged();
    }

    public ExpandoObject data = new ExpandoObject();

    public DynamicFormModel model = FormFactory.HorizontalLayout(
        FormFactory.BoolComponent("Check", "$.check"),
        FormFactory.StringComponent("Text", "$.text")
    );
}

````
