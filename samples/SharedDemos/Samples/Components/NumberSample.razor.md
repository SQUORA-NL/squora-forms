# Number

### Parameters

- **Label** (`string`)
  - **Description**: This parameter sets the label for the number input component. It provides a textual description or context for the input field, helping users understand what the field represents.
  - **Type**: `string`
  - **Required**: No. If it is not provided, the number input field will not have an associated label.
  - **Usage**: To provide a label for the number input, such as `Label="Temperature"`

- **Minimum** (`double`)
  - **Description**: Specifies the minimum value that can be entered in the number input field. This parameter is used to define a lower boundary for the allowed numeric value.
  - **Type**: `double`
  - **Default**: `double.MinValue`, representing the smallest possible value for a `double`.
  - **Required**: No. If not specified, it defaults to the minimum value for a `double`.
  - **Usage**: To set a minimum limit for the input value, e.g., `Minimum=0.0` for non-negative numbers.

- **Maximum** (`double`)
  - **Description**: Defines the maximum value that can be entered in the number input field. This parameter sets an upper boundary for the numeric value that a user can input.
  - **Type**: `double`
  - **Default**: `double.MaxValue`, representing the largest possible value for a `double`.
  - **Required**: No. If not specified, it defaults to the maximum value for a `double`.
  - **Usage**: To set a maximum limit for the input value, e.g., `Maximum=100.0` for values up to 100.0.


## Sample code

````csharp
    <BDForm FormModel="@model" Components="@UserSettingsService.Components" Value="@data" ValueChanged="@update"></BDForm>

@code {

    private void update()
    {
        StateHasChanged();
    }

    public ExpandoObject data = new ExpandoObject();

    public DynamicFormModel model = FormFactory.NumberComponent("Number", "$.number");
}
````
