The `DropDownComponentBuilder`, inheriting from the `FormComponentBuilder` class, is designed to construct and configure a dropdown component for a form. It combines its own specific methods with those inherited from `FormComponentBuilder` to provide a comprehensive way to build a dropdown input component.

| Method Name             | Description                                                                                                 | Example Usage                                      |
|-------------------------|-------------------------------------------------------------------------------------------------------------|----------------------------------------------------|
| `WithLabel`             | Sets the label for the dropdown component, typically displayed next to the dropdown field.                  | `WithLabel("Country")`                            |
| `WithOptions`           | Configures the options available in the dropdown list.                                                      | `WithOptions(new Dictionary<object, string> { { "US", "United States" }, ... })` |
| `WithSelectedValue`     | Sets the initially selected value in the dropdown list.                                                     | `WithSelectedValue("US")`                         |
| `WithComponents`        | Allows setting custom sub-components or additional components to be used within the dropdown component.     | `WithComponents(customComponents)`               |
| `WithPath`              | Sets the data binding path for the dropdown component, linking it to a specific property in a data model.   | `WithPath("$.user.country")`                      |
| `WithInvalidMessage`    | Sets a validation message to be displayed if the input fails validation.                                    | `WithInvalidMessage("Please select a country")`    |
| `WithStyle`             | Customizes the CSS style for the dropdown component.                                                        | `WithStyle("width: 100%;")`                       |
| `WithClass`             | Applies one or more CSS classes to the dropdown component.                                                  | `WithClass("country-dropdown")`                   |
| `WithParameter`         | Adds additional parameters to the component for extra data or settings.                                     | `WithParameter("aria-label", "Country selection")` |
| `WithFormModel`         | Sets the `DynamicFormModel` to define the structure and settings of the dropdown component.                  | `WithFormModel(formModel)`                        |
| `WithElements`          | Adds multiple `DynamicFormModel` elements to the dropdown component, useful for complex form structures.    | `WithElements(formElements)`                      |
| `WithElement`           | Incorporates a single `DynamicFormModel` element into the dropdown component.                                | `WithElement(singleElement)`                      |
| `WithValueChanged`      | Sets a callback function to be triggered when the value of the dropdown changes.                            | `WithValueChanged(callback)`                      |
| `ConfigureFormModel`    | Provides a way to apply custom configurations to the `DynamicFormModel`.                                    | `ConfigureFormModel(configAction)`                |
| `Build`                 | Finalizes the configurations and constructs a `DynamicFormModel` for the dropdown component.                | `Build()`                                         |


### Sample Usage

Here's how these methods can be utilized in practice:

```csharp
var dropDownComponent = new DropDownComponentBuilder()
    .WithLabel("Country")
    .WithOptions(new Dictionary<object, string> { { "US", "United States" }, { "CA", "Canada" }, { "MX", "Mexico" } })
    .WithSelectedValue("US")
    .WithPath("$.user.country")
    .WithInvalidMessage("Please select a country")
    .WithStyle("width: 100%;")
    .WithClass("country-dropdown")
    .WithParameter("aria-label", "Country selection")
    .Build();
```



