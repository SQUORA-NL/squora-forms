# DropDownSample

### Parameters

- **Label** (`string`)
  - **Description**: This parameter specifies the label for the dropdown component. It is used to give context or describe the purpose of the dropdown in the user interface.
  - **Type**: `string`
  - **Required**: No. It is optional and if not provided, the dropdown will not have a visible label.
  - **Usage**: To provide a visible label for the dropdown, e.g., `Label="Select Country"`

- **Options** (`Dictionary<object, string>`)
  - **Description**: This parameter is used to define the options available in the dropdown. It is a dictionary where the key represents the value of the option and the string represents the text to be displayed to the user.
  - **Type**: `Dictionary<object, string>`
  - **Default**: An empty dictionary. This means the dropdown will have no options unless explicitly set.
  - **Required**: No. However, to have a functional dropdown, options should be provided.
  - **Usage**: To populate the dropdown with selectable options, e.g., `Options=new Dictionary<object, string> { {1, "Option 1"}, {2, "Option 2"} }`


## Sample code

````csharp
<BDForm FormModel="@model" Components="@UserSettingsService.Components" Value="@data" ValueChanged="@update"></BDForm>

@code {

    private void update()
    {
        StateHasChanged();
    }

    public ExpandoObject data = new ExpandoObject();

    public DynamicFormModel model = FormFactory.DropDownComponent("Selection", "$.selected", new Dictionary<object, string>()
            {
                {
                    "Selection1", "Selection one"
                },
                {
                    "Selection2", "Selection two"
                },
                {
                    "Selection3", "Selection three"
                },
                {
                    "Selection4", "Selection four"
                }
            }
    );
}
````
