# Custom TemplateContent Component

This sample demonstrates registering and using a custom `TemplateContent` component via `TypeDefinitionName`. A custom Blazor component (`SampleTemplateComponent`) is registered in a `ComponentsList` under the key `(ComponentType.TemplateContent, "intro")`. The form model references it using `FormFactory.TemplateDisplay("intro", "$")`.

This mechanism lets you inject arbitrary Blazor markup into a form at any position — useful for help text, banners, conditional info panels, or any custom UI that receives the current form data.

## Razor declarations

```html
<SampleDocumentation MarkDownFile="Samples.Components.TemplateDisplaySample.razor.md" Data="@data" FormModel="@model">
    <BDForm FormModel="@model" Components="@customComponents" Value="@data" ValueChanged="@update"></BDForm>
</SampleDocumentation>
```

## Code

```csharp
private void update()
{
    StateHasChanged();
}

public ComponentsList customComponents => GetCustomComponents();

public ComponentsList GetCustomComponents()
{
    var result = new ComponentsList(UserSettingsService.Components);
    result.Add(
        new ComponentSelectionKey(ComponentType.TemplateContent, "intro"),
        typeof(SampleComponents.SampleTemplateComponent)
    );
    return result;
}

public Person data = new Person
{
    FirstName = "John",
    LastName = "Demo",
    BirthDate = new DateTime(1980, 09, 15),
    Address = new Address
    {
        StreetName = "Kabouterstraat",
        Number = 45,
        City = "Utrecht"
    }
};

private DynamicFormModel model;

protected override void OnInitialized()
{
    model = FormFactory.GroupLayout(
        FormFactory.StringComponent("First Name", "$.FirstName"),
        FormFactory.StringComponent("Last Name", "$.LastName"),
        CreateAddressForm(),
        FormFactory.GroupLayout(
            FormFactory.TemplateDisplay("intro", "$")
        )
    );
}

private DynamicFormModel CreateAddressForm()
{
    return FormFactory.GroupLayout(
        FormFactory.StringComponent("Street Name", "$.Address.StreetName"),
        FormFactory.DropDownComponent("City", "$.Address.City",
            new Dictionary<object, string>()
            {
                { "Utrecht", "Utrecht" },
                { "Oradea", "Oradea" },
                { "Amsterdam", "Amsterdam" }
            }
        )
    );
}
```
