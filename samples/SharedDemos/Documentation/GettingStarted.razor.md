## Getting Started with Blazor using MudBlazor and SquoraForms

This guide will help you set up a basic Blazor application using MudBlazor for UI components and SquoraForms for dynamic form handling.

### Step 1: Create a MudBlazor Application

First, create a standard Blazor application. You can follow the official Blazor documentation for this step if you're not familiar with creating a new Blazor project.

### Step 2: Install Necessary Packages

Once your Blazor application is set up, you need to install two essential packages:

- **SquoraForms**: This package provides dynamic functionalities in your Blazor applications.
- **Squora.Forms.Blazor.MudBlazorComponents**: This package integrates MudBlazor components with Squora.Forms.

You can install these packages via your preferred package manager or the command line.

### Step 3: Configure `program.cs`

Add the following lines of code to your `program.cs` file. This step is crucial as it registers the necessary services for SquoraForms and sets up the license.

```csharp
builder.Services.AddSquoraForms(cfg => 
                cfg.UseDefaultMudBlazorComponents());
              
builder.Services.AddSquoraFormsLicense("");

// To add your own components
builder.Services.AddSquoraForms(cfg => 
                cfg.UseDefaultMudBlazorComponents()
                .RegisterComponent(ComponentType.GroupLayout, "Collapsable", typeof(CollapsableGroup)));
```

### Step 4: Inject Components in Your Page

In your Blazor page, add the following line at the top:

```razor
@inject ComponentsList DynamicComponentList
```

This line injects the component list that you'll use for dynamic form generation.

### Step 5: Add the Dynamic Form

Now, add the dynamic form to your page with the following code:

```razor
<BDForm FormModel=@model Components=@DynamicComponentList Value="@data" ValueChanged="@update"></BDForm>
```

This form will be dynamically generated based on your model.

### Step 6: Implement the Code Behind

In the code section of your Blazor page, add the following:

```csharp
public DynamicFormModel model = FormFactory.StringComponent("Name", "$.name");

public ExpandoObject data = new ExpandoObject();

private void update()
{
    // Implement your update logic here
}
```

This code initializes your dynamic form model and handles the update logic.

### Step 7: Run Your Application

Finally, run your application to see your dynamic form in action. You can now start building more complex forms and components using SquoraForms and MudBlazor.

