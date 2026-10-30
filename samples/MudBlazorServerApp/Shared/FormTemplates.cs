using System.Dynamic;
using Squora.Forms.Blazor.Components.Factories;
using Squora.Forms.Common.Enums;
using Squora.Forms.Core.Models;

namespace MudBlazorServerApp.Shared;

public record FormTemplate(string Name, string Description, Func<DynamicFormModel> Create);

/// <summary>Forms to look at: each one is a JSON form definition built with the factory. "All components" holds one of every component.</summary>
public static class FormTemplates
{
    public static readonly IReadOnlyList<FormTemplate> All = new List<FormTemplate>
    {
        new("Contact form", "Name, email and a message", Contact),
        new("Profile", "A collapsible group with a summary line", Profile),
        new("Address", "A group with rows of fields and a dropdown", Address),
        new("Column widths", "Each element in a row gets its own width: 10% and 90%, 25/75, fixed sizes", ColumnWidths),
        new("Contact card", "A custom (templated) component: a card that shows what is typed next to the fields", ContactCard),
        new("Job application", "A realistic form: rows with different column widths, a styled field and a collapsible part", JobApplication),
        new("Order", "A list of order lines with add and delete", Order),
        new("Order with rules", "A webshop checkout where fields hide, show, disable and enable", OrderWithRules),
        new("All components", "One of every component", AllComponents),
    };

    public static DynamicFormModel Contact() =>
        FormFactory.VerticalLayout(
            FormFactory.HorizontalLayout(
                FormFactory.StringComponent("First name", "$.firstName"),
                FormFactory.StringComponent("Last name", "$.lastName")),
            // a class on one component: only this textbox gets the .sq-highlight look (see wwwroot/css/form-themes.css)
            FormFactory.StringComponent("Email", "$.email", b => b.WithClass("sq-highlight")),
            FormFactory.StringComponent("Message", "$.message"),
            FormFactory.BoolComponent("Send me a copy", "$.copy"));

    /// <summary>
    /// The Width of an element in a horizontal layout. A percentage is its share of the row (10% next to 90% fills it exactly,
    /// the gap included); a size like 120px is fixed and the others share what is left. Empty shares the row equally.
    /// </summary>
    public static DynamicFormModel ColumnWidths() =>
        FormFactory.VerticalLayout(
            FormFactory.StringDisplay("10% and 90%", "$.none"),
            FormFactory.HorizontalLayout(
                FormFactory.StringComponent("Nr", "$.nr", b => b.WithWidth("10%")),
                FormFactory.StringComponent("Description", "$.description", b => b.WithWidth("90%"))),
            FormFactory.StringDisplay("25% and 75%", "$.none"),
            FormFactory.HorizontalLayout(
                FormFactory.StringComponent("Code", "$.code", b => b.WithWidth("25%")),
                FormFactory.StringComponent("Name", "$.name", b => b.WithWidth("75%"))),
            FormFactory.StringDisplay("20%, 60% and 20%", "$.none"),
            FormFactory.HorizontalLayout(
                FormFactory.IntComponent("Qty", "$.qty", b => b.WithWidth("20%")),
                FormFactory.StringComponent("Product", "$.product", b => b.WithWidth("60%")),
                FormFactory.NumberComponent("Price", "$.price", b => b.WithWidth("20%"))),
            FormFactory.StringDisplay("A fixed 120px, the rest shared", "$.none"),
            FormFactory.HorizontalLayout(
                FormFactory.IntComponent("Number", "$.houseNumber", b => b.WithWidth("120px")),
                FormFactory.StringComponent("Street", "$.street")),
            FormFactory.StringDisplay("No widths: shared equally", "$.none"),
            FormFactory.HorizontalLayout(
                FormFactory.StringComponent("One", "$.one"),
                FormFactory.StringComponent("Two", "$.two"),
                FormFactory.StringComponent("Three", "$.three")),
            FormFactory.StringDisplay("A collapsible group (a templated component) takes a width the same way", "$.none"),
            FormFactory.HorizontalLayout(
                FormFactory.StringComponent("Side note", "$.sideNote", b => b.WithWidth("30%")),
                FormFactory.GroupLayout(group => group
                        .WithLabel("Details")
                        .WithWidth("70%")
                        .WithParameter("StartCollapsed", true)
                        .WithParameter("HeaderModel", FormFactory.StringDisplay("Side note", "$.sideNote")),
                    FormFactory.StringComponent("Inside the collapsible group", "$.details")
                ).WithTypeDefinionName("Collapsable")));

    /// <summary>
    /// A custom component in a form: "contactcard" is a templated component (see Shared/FormExtentions/ContactCard.razor, registered
    /// in Program.cs and FormFrameworks.cs). It reads the form's data itself, and takes a Width in the row like any other element.
    /// </summary>
    public static DynamicFormModel ContactCard() =>
        FormFactory.VerticalLayout(
            FormFactory.HorizontalLayout(
                FormFactory.VerticalLayout(
                    FormFactory.HorizontalLayout(
                        FormFactory.StringComponent("First name", "$.firstName"),
                        FormFactory.StringComponent("Last name", "$.lastName")),
                    FormFactory.StringComponent("Email", "$.email"),
                    FormFactory.StringComponent("Phone", "$.phone"),
                    FormFactory.StringComponent("City", "$.city")).WithWidth("55%"),
                FormFactory.TemplateDisplay("contactcard", "$").WithWidth("45%")));

    public static DynamicFormModel JobApplication() =>
        FormFactory.VerticalLayout(
            FormFactory.GroupLayout(group => group.WithLabel("About you"),
                FormFactory.HorizontalLayout(
                    FormFactory.DropDownComponent("Title", "$.title", new Dictionary<object, string> { ["mr"] = "Mr", ["ms"] = "Ms", ["x"] = "Other" })
                        .WithWidth("15%"),
                    FormFactory.StringComponent("First name", "$.firstName", b => b.WithWidth("40%")),
                    FormFactory.StringComponent("Last name", "$.lastName", b => b.WithWidth("45%"))),
                FormFactory.HorizontalLayout(
                    FormFactory.StringComponent("Email", "$.email", b => b.WithWidth("60%").WithClass("sq-highlight")),
                    FormFactory.StringComponent("Phone", "$.phone", b => b.WithWidth("40%"))),
                FormFactory.HorizontalLayout(
                    FormFactory.StringComponent("Street", "$.address.street", b => b.WithWidth("55%")),
                    FormFactory.IntComponent("Number", "$.address.number", b => b.WithWidth("15%")),
                    FormFactory.StringComponent("Postal code", "$.address.postalCode", b => b.WithWidth("30%"))),
                FormFactory.StringComponent("City", "$.address.city")),
            FormFactory.GroupLayout(group => group.WithLabel("The job"),
                FormFactory.HorizontalLayout(
                    FormFactory.DropDownComponent("Position", "$.position", new Dictionary<object, string>
                    {
                        ["dev"] = "Developer", ["design"] = "Designer", ["pm"] = "Product manager",
                    }).WithWidth("40%"),
                    FormFactory.DateTimeComponent("Available from", "$.startDate", b => b.WithWidth("35%")),
                    FormFactory.NumberComponent("Salary wish", "$.salary", b => b.WithWidth("25%"))),
                FormFactory.StringComponent("Why do you want this job?", "$.motivation"),
                FormFactory.FileUploadComponent("CV and letter", "$.documents", b => b.WithMultiple())),
            FormFactory.GroupLayout(group => group
                    .WithLabel("More about you")
                    .WithParameter("StartCollapsed", true)
                    .WithParameter("HeaderModel", FormFactory.StringDisplay("Name", "$.firstName")),
                FormFactory.HorizontalLayout(
                    FormFactory.StringComponent("Current employer", "$.employer", b => b.WithWidth("70%")),
                    FormFactory.IntComponent("Years", "$.years", b => b.WithWidth("30%")))
            ).WithTypeDefinionName("Collapsable"),
            FormFactory.BoolComponent("I agree to be contacted about this job", "$.consent"));

    public static DynamicFormModel Profile() =>
        FormFactory.GroupLayout(group => group
                .WithParameter("StartCollapsed", true)
                .WithParameter("HeaderModel", FormFactory.StringDisplay("Name", "$.name")),
            FormFactory.VerticalLayout(
                FormFactory.StringComponent("Name", "$.name"),
                FormFactory.StringComponent("City", "$.city"),
                FormFactory.FileUploadComponent("Documents", "$.documents", b => b.WithMultiple()))
        ).WithTypeDefinionName("Collapsable");

    public static DynamicFormModel Address() =>
        FormFactory.GroupLayout(group => group.WithLabel("Address"),
            FormFactory.HorizontalLayout(
                FormFactory.StringComponent("Street", "$.address.street"),
                FormFactory.IntComponent("Number", "$.address.number")),
            FormFactory.HorizontalLayout(
                FormFactory.StringComponent("Postal code", "$.address.postalCode"),
                FormFactory.StringComponent("City", "$.address.city")),
            FormFactory.DropDownComponent("Country", "$.address.country", Countries()));

    public static DynamicFormModel Order() =>
        FormFactory.VerticalLayout(
            FormFactory.StringComponent("Customer", "$.customer"),
            FormFactory.DateTimeComponent("Order date", "$.date"),
            FormFactory.AddAction("Add line", "$.lines", new ExpandoObject()),
            FormFactory.ListComponent("Lines", "$.lines",
                FormFactory.HorizontalLayout(
                    FormFactory.StringComponent("Product", "@.product"),
                    FormFactory.IntComponent("Quantity", "@.quantity"),
                    FormFactory.NumberComponent("Price", "@.price"),
                    FormFactory.DeleteAction("Delete", "@"))),
            FormFactory.NumberDisplay("Number of lines:", "$.lines"));

    /// <summary>
    /// A checkout where the answers decide what is asked next. Every rule effect is used once or more:
    /// SHOW (only while it matches), HIDE (not while it matches), DISABLE and ENABLE (read-only unless it matches).
    /// A rule compares the data at a path with a JSON schema; an unanswered field never matches.
    /// </summary>
    public static DynamicFormModel OrderWithRules() =>
        FormFactory.VerticalLayout(
            FormFactory.GroupLayout(group => group.WithLabel("Customer"),
                FormFactory.BoolComponent("This is a company order", "$.company"),
                // SHOW: only asked for companies
                FormFactory.StringComponent("Company name", "$.companyName")
                    .AddSchemaRule(RuleEffect.SHOW, "$.company", "{ \"const\": true }"),
                FormFactory.StringComponent("VAT number", "$.vat")
                    .AddSchemaRule(RuleEffect.SHOW, "$.company", "{ \"const\": true }"),
                // HIDE: private customers only
                FormFactory.StringComponent("Date of birth", "$.birthDate")
                    .AddSchemaRule(RuleEffect.HIDE, "$.company", "{ \"const\": true }")),
            FormFactory.GroupLayout(group => group.WithLabel("Delivery"),
                FormFactory.DropDownComponent("How do you want it?", "$.method", new Dictionary<object, string>
                {
                    ["pickup"] = "Pick up in the store",
                    ["delivery"] = "Deliver to my address",
                    ["digital"] = "Digital download",
                }),
                FormFactory.StringComponent("Store", "$.store")
                    .AddSchemaRule(RuleEffect.SHOW, "$.method", "{ \"const\": \"pickup\" }"),
                FormFactory.StringComponent("E-mail for the download link", "$.downloadEmail")
                    .AddSchemaRule(RuleEffect.SHOW, "$.method", "{ \"const\": \"digital\" }"),
                // a whole group can have a rule too
                FormFactory.GroupLayout(group => group.WithLabel("Delivery address"),
                    FormFactory.HorizontalLayout(
                        FormFactory.StringComponent("Street", "$.address.street"),
                        FormFactory.IntComponent("Number", "$.address.number")),
                    FormFactory.HorizontalLayout(
                        FormFactory.StringComponent("Postal code", "$.address.postalCode"),
                        FormFactory.StringComponent("City", "$.address.city")),
                    FormFactory.BoolComponent("Express delivery", "$.express"),
                    // DISABLE: express always arrives tomorrow
                    FormFactory.DateTimeComponent("Delivery date", "$.deliveryDate")
                        .AddSchemaRule(RuleEffect.DISABLE, "$.express", "{ \"const\": true }"))
                    .AddSchemaRule(RuleEffect.SHOW, "$.method", "{ \"const\": \"delivery\" }")),
            FormFactory.GroupLayout(group => group.WithLabel("Order"),
                FormFactory.IntComponent("Quantity", "$.quantity"),
                // ENABLE: read-only until the schema matches (here: 10 or more)
                FormFactory.StringComponent("Bulk discount code", "$.discountCode")
                    .AddSchemaRule(RuleEffect.ENABLE, "$.quantity", "{ \"minimum\": 10 }"),
                FormFactory.BoolComponent("I accept the terms", "$.terms"),
                FormFactory.StringComponent("Remark for the shop", "$.remark")
                    .AddSchemaRule(RuleEffect.ENABLE, "$.terms", "{ \"const\": true }")));

    public static DynamicFormModel AllComponents() =>
        FormFactory.VerticalLayout(
            FormFactory.GroupLayout(group => group.WithLabel("Inputs"),
                FormFactory.StringComponent("Text field", "$.text"),
                FormFactory.StringComponent("Text field with its own class", "$.styled", b => b.WithClass("sq-highlight")),
                FormFactory.BoolComponent("Checkbox", "$.checkbox"),
                FormFactory.IntComponent("Integer", "$.integer"),
                FormFactory.NumberComponent("Number", "$.number"),
                FormFactory.DateTimeComponent("Date/Time", "$.dateTime"),
                FormFactory.DropDownComponent("Dropdown", "$.dropdown", Countries()),
                FormFactory.FileUploadComponent("File upload", "$.files", b => b.WithMultiple())),
            FormFactory.GroupLayout(group => group.WithLabel("Layouts"),
                FormFactory.HorizontalLayout(
                    FormFactory.StringComponent("Left", "$.left"),
                    FormFactory.StringComponent("Right", "$.right")),
                FormFactory.VerticalLayout(
                    FormFactory.StringComponent("Top", "$.top"),
                    FormFactory.StringComponent("Bottom", "$.bottom"))),
            FormFactory.GroupLayout(group => group.WithLabel("Lists and actions"),
                FormFactory.AddAction("Add item", "$.items", new ExpandoObject()),
                FormFactory.ListComponent("Items", "$.items",
                    FormFactory.HorizontalLayout(
                        FormFactory.StringComponent("Item", "@.name"),
                        FormFactory.DeleteAction("Delete", "@")))),
            FormFactory.GroupLayout(group => group.WithLabel("Display"),
                FormFactory.StringDisplay("Text field shown as text", "$.text"),
                FormFactory.NumberDisplay("Number of items:", "$.items")),
            FormFactory.GroupLayout(group => group
                    .WithLabel("Collapsible group")
                    .WithParameter("StartCollapsed", true)
                    .WithParameter("HeaderModel", FormFactory.StringDisplay("Text field", "$.text")),
                FormFactory.StringComponent("Inside the collapsible group", "$.collapsible")
            ).WithTypeDefinionName("Collapsable"));

    private static Dictionary<object, string> Countries() => new()
    {
        ["nl"] = "Netherlands",
        ["be"] = "Belgium",
        ["de"] = "Germany",
    };
}
