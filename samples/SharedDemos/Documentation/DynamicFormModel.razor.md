## `DynamicFormModel`
This is the core model for the dynamic form components. It represents a form element, with properties like `DynamicType`, `Elements`, `Parameters`, and others. The `DynamicType` is a key component that defines the type of form element (e.g., string, number, vertical layout).

### Creating a Simple Form
Here's an example of how you might use `FormFactory` to create a simple form with a vertical layout and a text input:

```csharp
using Squora.Forms.Blazor.Components.Factories;
using Squora.Forms.Models;

public class MyFormComponent
{
    private DynamicFormModel myForm;

    public MyFormComponent()
    {
        myForm = FormFactory.VerticalLayout(
            FormFactory.StringComponent("Name", "name"),
            FormFactory.BooleanComponent("Accept Terms", "acceptTerms"),
        );
    }
}
```

### Explanation
- **`VerticalLayout`**: This creates a vertical layout for the form elements.
- **`StringComponent`**: This adds a text input field for the name.
- **`BooleanComponent`**: This adds a checkbox for accepting terms.

### Advanced Usage
For more complex forms, you can use builders and more specific configurations. For example, if you want to create a group layout with specific configurations:

```csharp
public DynamicFormModel CreateAdvancedForm()
{
    return FormFactory.GroupLayout(builder =>
    {
        builder.AddElement(FormFactory.StringComponent("Email", "email"));
        builder.AddElement(FormFactory.NumberComponent("Age", "age"));
        builder.AddElement(FormFactory.DateTimeComponent("Birthdate", "birthdate"));
    });
}
```

### Builders
Builders like `StringComponentBuilder`, `BooleanComponentBuilder`, etc., provide a fluent interface to configure form elements in detail. They are particularly useful when you need to set multiple properties on a form element.

