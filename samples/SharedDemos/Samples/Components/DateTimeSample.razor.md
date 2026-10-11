# Datetime

### Parameters

- **Label** (`string`)
  - **Description**: This parameter is used to specify the label associated with the date-time input component. The label describes the purpose or context of the input field.
  - **Type**: `string`
  - **Required**: No. If not specified, the component may not have a visible label.
  - **Example Usage**: `Label="Start Date"`

- **MinimumDateTime** (`DateTime?`)
  - **Description**: Defines the minimum allowable date and time for the input. This parameter can be used to restrict user selection to a specific range starting from this minimum value.
  - **Type**: `DateTime?` (nullable)
  - **Default**: `DateTime.MinValue`, indicating the earliest possible date and time.
  - **Required**: No. If not specified, it defaults to the minimum value allowed by the `DateTime` structure.

- **MaximumDateTime** (`DateTime?`)
  - **Description**: Sets the maximum allowable date and time for the input. This parameter restricts user selection to a range ending at this maximum value.
  - **Type**: `DateTime?` (nullable)
  - **Default**: `DateTime.MaxValue`, indicating the latest possible date and time.
  - **Required**: No. If not specified, it defaults to the maximum value allowed by the `DateTime` structure.

## Sample code

````csharp

<BDForm FormModel="@model" Components="@UserSettingsService.Components" Value="@data" ValueChanged="@update"></BDForm>


@code {

    private void update()
    {
        StateHasChanged();
    }

    public ExpandoObject data = new ExpandoObject();

    public DynamicFormModel model = FormFactory.DateTimeComponent("DateTime", "$.dateTime");
}

````



