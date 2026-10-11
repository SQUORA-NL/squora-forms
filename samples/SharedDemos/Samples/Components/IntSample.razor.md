# Integer

### Parameters

- **Label** (`string`)
  - **Description**: This parameter is used to define the label associated with the integer input component. It serves as a textual description or context for the input field.
  - **Type**: `string`
  - **Required**: No. If not provided, the component will not display a label.
  - **Usage**: To provide a label for the integer input, e.g., `Label="Age"`

- **Minimum** (`int`)
  - **Description**: Specifies the minimum value that can be entered in the integer input field. This parameter can be used to set a lower limit on the value that a user can input.
  - **Type**: `int`
  - **Default**: `int.MinValue`, representing the smallest possible integer value.
  - **Required**: No. If not specified, it defaults to the minimum value that an `int` can hold.
  - **Usage**: To set a lower bound for the input value, e.g., `Minimum=0` for non-negative values.

- **Maximum** (`int`)
  - **Description**: Defines the maximum value that can be entered in the integer input field. This parameter is used to set an upper limit on the value a user can input.
  - **Type**: `int`
  - **Default**: `int.MaxValue`, representing the largest possible integer value.
  - **Required**: No. If not specified, it defaults to the maximum value that an `int` can hold.
  - **Usage**: To set an upper bound for the input value, e.g., `Maximum=100` for values up to 100.


## Sample code

````csharp
    <BDForm FormModel="@model" Components="@UserSettingsService.Components" Value="@data" ValueChanged="@update"></BDForm>

@code {

    private void update()
    {
        StateHasChanged();
    }

    public ExpandoObject data = new ExpandoObject();

    public DynamicFormModel model = FormFactory.IntComponent("Number", "$.int");
}
````
