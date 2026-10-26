using System.Dynamic;
using Squora.Forms.Blazor.Components.Factories;
using Squora.Forms.Common.Enums;
using Squora.Forms.Core.Models;

namespace MudBlazorServerApp.Shared;

public record FormTemplate(string Name, string Description, Func<DynamicFormModel> Create);

/// <summary>Starting points for the editor. "All components" holds one of everything the editor can build.</summary>
public static class FormTemplates
{
    public static readonly IReadOnlyList<FormTemplate> All = new List<FormTemplate>
    {
        new("Empty", "A blank form", Empty),
        new("Contact form", "Name, email and a message", Contact),
        new("Profile", "A collapsible group with a summary line", Profile),
        new("Address", "A group with rows of fields and a dropdown", Address),
        new("Order", "A list of order lines with add and delete", Order),
        new("Order with rules", "A webshop checkout where fields hide, show, disable and enable", OrderWithRules),
        new("All components", "One of every component", AllComponents),
    };

    public static DynamicFormModel Empty() => FormFactory.VerticalLayout();

    public static DynamicFormModel Contact() =>
        FormFactory.VerticalLayout(
            FormFactory.HorizontalLayout(
                FormFactory.StringComponent("First name", "$.firstName"),
                FormFactory.StringComponent("Last name", "$.lastName")),
            FormFactory.StringComponent("Email", "$.email"),
            FormFactory.StringComponent("Message", "$.message"),
            FormFactory.BoolComponent("Send me a copy", "$.copy"));

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
