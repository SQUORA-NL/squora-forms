# Typed Data Binding — Base Sample

This sample demonstrates binding a form model to a strongly-typed C# object (`TestData`). Changes to any form field are reflected immediately in the bound object. The form includes string validation (min/max length), a custom validation message using tokens, a submit action, and a date field with a display format.

## Razor declarations

```html
<SampleDocumentation MarkDownFile="Samples.Typed.BaseSample.razor.md" Data="@data" FormModel="@model">
    <BDForm FormModel="@model" Components="@UserSettingsService.Components" Value="@data" ValueChanged="@update" OnSubmitted="@Submit"></BDForm>
</SampleDocumentation>
```

## Code

```csharp
[Inject]
ISnackbar? Snackbar { get; set; }

private void Submit()
{
    if (Snackbar != null)
    {
        Snackbar.Add("Form is Submitted", Severity.Normal, item => item.VisibleStateDuration = 1000);
    }
}

private void update()
{
    StateHasChanged();
}

TestData data = new TestData() { name = "demo", birthDate = "none" };

private DynamicFormModel model = FormFactory.GroupLayout(gr => gr
        .WithParameter("style", ""),
        FormFactory.StringComponent("voornaam", "$.name", name => name
              .WithMinimumLength(7)
              .WithMaximumLength(14)
              .WithInvalidMessage("Name should be at least {MinimumLength} and max {MaximumLength} long!")
              ),
        FormFactory.SubmitAction("Submit form - from the middle of the form"),
        FormFactory.StringComponent("Date", "$.birthDate", birthdate => birthdate
              .WithFormat("dd-MM-YYYY")
              )
       );

public class TestData
{
    public string name { get; set; }
    public string birthDate { get; set; }
}
```
