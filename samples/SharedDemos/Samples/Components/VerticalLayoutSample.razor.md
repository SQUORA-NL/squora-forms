# Vertical

### Parameters

- None

## Sample code

````csharp
<BDForm FormModel="@model" Components="@UserSettingsService.Components" Value="@data" ValueChanged="@update"></BDForm>


@code {

    private void update()
    {
        StateHasChanged();
    }

    public ExpandoObject data = new ExpandoObject();

    public DynamicFormModel model = FormFactory.VerticalLayout(
        FormFactory.BoolComponent("Check", "$.check"),
        FormFactory.StringComponent("Text", "$.text")
    );
}

````
