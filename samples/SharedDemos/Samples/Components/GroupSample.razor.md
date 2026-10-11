# Group

### Parameters

- **Label** (`string`)
  - **Description**: This parameter is used to set the label for the group layout component. The label typically serves as a heading or a title for a section of the form, providing context or grouping for the enclosed form elements.
  - **Type**: `string`
  - **Required**: No. It is optional, and if not provided, the group layout will not display a label or title.
  - **Usage**: To provide a heading for a group of form elements, e.g., `Label="Contact Information"`.


## Sample code

````csharp
<BDForm FormModel="@model" Components="@UserSettingsService.Components" Value="@data" ValueChanged="@update"></BDForm>

@code {

    private void update()
    {
        StateHasChanged();
    }

    public ExpandoObject data = new ExpandoObject();

    public DynamicFormModel model = FormFactory.GroupLayout(
        FormFactory.BoolComponent("Check", "$.check"),
        FormFactory.StringComponent("Text", "$.text")
    );
}

````
