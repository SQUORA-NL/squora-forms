# List

### Parameters

- **Label** (`string`)
  - **Description**: This parameter specifies the label for the list component. It serves as a textual identifier or header for the list, providing context or a descriptive title for the list items contained within the component.
  - **Type**: `string`
  - **Required**: No. If not provided, the list component will not have a visible label.
  - **Usage**: To provide a heading or descriptive label for the list, such as `Label="Item List"`.


## Sample code

````csharp
<BDForm FormModel="@model" Components="@UserSettingsService.Components" Value="@data" ValueChanged="@update"></BDForm>

@code {

    private void update()
    {
        StateHasChanged();
    }

    public Container data = new Container();

    public DynamicFormModel model =
    FormFactory.GroupLayout(
        FormFactory.AddAction("Add person", "$.items", new Name { firstname = "<firstname>", lastname = "<lastname>" }),
        FormFactory.NumberDisplay("Aantal:", "$.items"),
        FormFactory.ListComponent("Items", "$.items",
            FormFactory.HorizontalLayout(
                FormFactory.StringComponent("Firstname", "@.firstname"),
                FormFactory.StringComponent("Lastname", "@.lastname"),
                FormFactory.DeleteAction("Delete", "@")
                )
            )
       );

    public class Container
    {
        public List<Name> items { get; set; } = new List<Name>();
    }

    public class Name
    {
        public string firstname { get; set; } = "<firstname>";
        public string lastname { get; set; } = "<lastname>";
    }
}
````
