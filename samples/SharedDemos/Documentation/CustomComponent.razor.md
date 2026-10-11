## Creating a custom component in Blazor using SquoraForms and MudBlazor

In this guide, you'll learn how to create a custom collapsible group component using SquoraForms and MudBlazor. This component will 
be a dynamic form element that can toggle between expanded and collapsed states.

### Step 1: Define the custom component

Create a new Razor component for your custom group layout. This component will inherit from `Squora.Forms.Blazor.Components.Layout.GroupLayoutBase`. 
Here's the code for your custom component:

```razor
@inherits Squora.Forms.Blazor.Components.Layout.GroupLayoutBase

<div class="groupcomponent @Class" style="@Style">
    @if (ruleEffect != Squora.Forms.Common.Enums.RuleEffect.HIDE)
    {
        @if (collapsed)
        {
            <MudCard>
                <MudCardHeader>
                    <MudStack AlignItems="AlignItems.Stretch" Row=true>
                        @if (!string.IsNullOrEmpty(Label))
                        {
                            <MudText>@Label</MudText>
                        }
                        <MudButton OnClick="@Toggle">Expand</MudButton>
                    </MudStack>
                </MudCardHeader>
                <MudCardContent>
                    <MudStack>
                            <DynamicComponent Type=@GetComponentTypeFor(HeaderModel) Parameters=@GetValidParametersFor(HeaderModel) />
                    </MudStack>
                </MudCardContent>
            </MudCard>
        }
        else
        {
            <MudCard>

                <MudCardHeader>
                    <MudStack AlignItems="AlignItems.Stretch" Row=true>
                        @if (!string.IsNullOrEmpty(Label))
                        {
                            <MudText>@Label</MudText>
                        }
                        <MudButton OnClick="@Toggle">Collapse</MudButton>
                    </MudStack>
                </MudCardHeader>

                <MudCardContent>
                    <MudStack>
                        @foreach (var element in this.FormModel.Elements)
                        {
                            <DynamicComponent Type=@GetComponentTypeFor(element) Parameters=@GetValidParametersFor(element) />
                        }
                    </MudStack>
                </MudCardContent>
            </MudCard>
        }
    }
</div>

@code {
    private bool collapsed;
    [Parameter]
    public bool StartCollapsed { get; set; } = true;

    [Parameter]
    public DynamicFormModel HeaderModel { get; set; } = new();

    protected override void OnInitialized()
    {
        collapsed = StartCollapsed; // Set the initial state only once
    }

    private void Toggle()
    {
        collapsed = !collapsed;
    }
}
```

### Step 2: Register the custom component

In your `program.cs`, register your custom component. Add this line to the SquoraForms configuration:

```csharp
builder.Services.AddSquoraForms(cfg => 
    cfg.UseDefaultMudBlazorComponents()
       .RegisterComponent(ComponentType.GroupLayout, "Collapsable", typeof(YourCustomComponentClassName)));
```

Replace `YourCustomComponentClassName` with the actual name of your custom component class.

### Step 3: Add or modify the model in the page

Change the model in your page to use the new custom component. Here's how you can define it:

```csharp
public DynamicFormModel model = FormFactory.GroupLayout(group =>
    group
       .WithParameter("Collapsed", false)
       .WithParameter("HeaderModel", HeaderForm())
       .WithElements(DetailForm())
    ).WithTypeDefinionName("Collapsable");

private static DynamicFormModel HeaderForm()
{
    return FormFactory.StringComponent("Name", "$.name");
}

private static DynamicFormModel DetailForm()
{
    return FormFactory.VerticalLayout(
        FormFactory.StringComponent("Name", "$.name"),
        FormFactory.StringComponent("City", "$.city")
        );
}
```

### Conclusion

With these steps, you have created a custom collapsible group component in Blazor using SquoraForms and MudBlazor. You can now use this 
component in your Blazor forms to create dynamic, interactive layouts.