# String

### Parameters

- **Label** (`string`)
  - Description: The label associated with the string input component.
  - Type: `string`
  - Required: No
  - Example: `Label="Username"`

- **MinimumLength** (`long?`)
  - Description: The minimum length allowed for the string input. 
  - Type: `long?` (nullable)
  - Default: `0`
  - Required: No
  - Example: `MinimumLength="3"`

- **MaximumLength** (`long?`)
  - Description: The maximum length allowed for the string input.
  - Type: `long?` (nullable)
  - Default: `long.MaxValue`
  - Required: No
  - Example: `MaximumLength="255"`

### Properties

- **ValidationString**
  - Description: Gets the validation message after token replacement.
  - Type: `string`

- **StringValue**
  - Description: The current value of the string input.
  - Type: `string`
  - Access: Read/Write

## Sample code

````csharp
<BDForm FormModel="@model" Components="@UserSettingsService.Components" Value="@data" ValueChanged="@update"></BDForm>

@code {

    private void update()
    {
        StateHasChanged();
    }

    public ExpandoObject data = new ExpandoObject();

    public DynamicFormModel model = FormFactory.StringComponent("Name", "$.name");
}
````
