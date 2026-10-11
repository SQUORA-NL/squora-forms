The `StringComponentBuilder`, which inherits from the `FormComponentBuilder` class, is designed to construct and configure a string input component for a form. It integrates the functionalities from both its own methods and those inherited from `FormComponentBuilder`.

| Method Name                  | Description                                                                                                 | Example Usage                                      |
|------------------------------|-------------------------------------------------------------------------------------------------------------|----------------------------------------------------|
| `WithLabel`                  | Sets the label for the string component, usually displayed next to the input field.                         | `WithLabel("Username")`                           |
| `WithMinimumLength`          | Specifies the minimum number of characters required in the input.                                           | `WithMinimumLength(3)`                            |
| `WithMaximumLength`          | Defines the maximum number of characters allowed in the input.                                              | `WithMaximumLength(255)`                          |
| `WithFormat`                 | Sets a specific format for the string input, like date or phone number formats.                            | `WithFormat("yyyy-MM-dd")`                        |
| `WithPattern`                | Assigns a regular expression pattern for validating the input.                                              | `WithPattern(@"^\d{4}-\d{2}-\d{2}$")`             |
| `WithComponents`             | Allows setting a dictionary of components to be used within the form component, useful for including custom components. | `WithComponents(customComponents)`               |
| `WithPath`                   | Sets the data binding path for the component, linking it to a specific property in a data model.           | `WithPath("$.user.name")`                         |
| `WithInvalidMessage`         | Sets a message that will be displayed if the input validation fails.                                       | `WithInvalidMessage("Invalid format")`            |
| `WithStyle`                  | Customizes the CSS style of the component.                                                                 | `WithStyle("width: 100%;")`                       |
| `WithClass`                  | Sets one or more CSS classes for the component.                                                            | `WithClass("form-control")`                       |
| `WithParameter`              | Adds a custom parameter to the component, which can be used for additional data or settings.               | `WithParameter("placeholder", "Enter your name")` |
| `WithFormModel`              | Sets the `DynamicFormModel` which defines the structure and settings of the form component.                | `WithFormModel(formModel)`                        |
| `WithValueChanged`           | Sets a callback function that will be invoked when the value of the component changes.                      | `WithValueChanged(callback)`                      |
| `Build`                      | Finalizes the configurations and creates a `DynamicFormModel` representing the string input component.     | `Build()`                                         |


### Sample Usage

Here's how these methods can be combined in practice:

```csharp
var stringComponent = new StringComponentBuilder()
    .WithLabel("Email")
    .WithMinimumLength(5)
    .WithMaximumLength(50)
    .WithPath("$.user.email")
    .WithInvalidMessage("Invalid email address")
    .WithStyle("width: 100%;")
    .WithClass("email-input")
    .WithParameter("placeholder", "Enter your email")
    .Build();
```

In this example, the `StringComponentBuilder` is used to create a string input component labeled "Email" with specific constraints on length, data binding path, validation message, styling, and an additional placeholder parameter. This illustrates the flexibility and power of combining the specific methods of `StringComponentBuilder` with the more general methods inherited from `FormComponentBuilder`.


