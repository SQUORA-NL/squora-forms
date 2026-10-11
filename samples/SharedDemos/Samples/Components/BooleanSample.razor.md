# Boolean

### Parameters

- **Label** (`string`)
  - Description: The label associated with the boolean input component. This label is typically displayed alongside the checkbox or toggle to describe its purpose.
  - Type: `string`
  - Required: No
  - Example: `Label="Accept Terms"`

- **NeedsToBeChecked** (`bool?`)
  - Description: Specifies whether the checkbox must be checked for the input to be considered valid. This is useful in scenarios like agreeing to terms and conditions where a user's affirmative action is required.
  - Type: `bool?` (nullable)
  - Default: `null` (indicating no requirement by default)
  - Required: No
  - Example: `NeedsToBeChecked="true"`

## Sample code

````csharp
@page "/samples/components/boolean"

    <BDForm FormModel="@model" Components="@UserSettingsService.Components" Value="@data" ValueChanged="@update"></BDForm>

@code {

    private void update()
    {
        StateHasChanged();
    }

    public ExpandoObject data = new ExpandoObject();

    public DynamicFormModel model = FormFactory.BoolComponent("Check", "$.check");
}

````
