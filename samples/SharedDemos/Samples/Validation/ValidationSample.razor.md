# Validation Sample

This sample demonstrates string field validation using `MinimumLength`, `MaximumLength`, and `InvalidMessage`. The `InvalidMessage` parameter supports `{MinimumLength}` and `{MaximumLength}` tokens that are replaced at runtime with the configured values.

Three fields are shown:
- A field with both min and max length constraints and a custom message using both tokens
- A field with only a max length constraint and a message using the `{MaximumLength}` token
- A field with only a max length constraint and the default message

## Code

```csharp
private void update()
{
    StateHasChanged();
}

public ValidationModel data = new ValidationModel
{
    First = "First text",
    Second = "Second text",
    Third = "Third Text"
};

public DynamicFormModel model = FormFactory.GroupLayout(
    FormFactory.StringComponentBuilder("$.First")
            .WithLabel("regular string")
            .WithMinimumLength(4)
            .WithMaximumLength(10)
            .WithInvalidMessage("The length should be between {MinimumLength} and {MaximumLength}")
            .Build(),
    FormFactory.StringComponentBuilder("$.Second")
            .WithLabel("normal string")
            .WithMaximumLength(4)
            .WithInvalidMessage("The length should less than {MaximumLength}")
            .Build(),
    FormFactory.StringComponentBuilder("$.Third")
            .WithLabel("string")
            .WithMaximumLength(10)
            .Build()
);

public class ValidationModel
{
    public string First { get; set; }
    public string Second { get; set; }
    public string Third { get; set; }
}
```
