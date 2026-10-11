using Squora.Forms.Blazor.Components.Factories;
using Squora.Forms.Common.Enums;
using System.Dynamic;
using Squora.Forms.Core.Models;

namespace SharedDemos.Samples.Rules
{
    public partial class RulesSample : SampleBasePage
    {
        // sample
        private DynamicFormModel model;
        public ExpandoObject data = new ExpandoObject();

        private void update()
        {
            StateHasChanged();
        }
        protected override void OnInitialized()
        {
            model = FormFactory.VerticalLayout(
                FormFactory.GroupLayout(
                         FormFactory.AddAction("Add person", "$.items", new ExpandoObject())
                            .AddSchemaRule(RuleEffect.HIDE, "$.items", @"
                                { 
                                    'type': 'array', 
                                    'items': { 
                                        'type': 'object', 
                                        'required': ['firstname', 'lastname'],
                                        'properties': {
                                            'firstname': { 'type': 'string', 'minLength': 2 },
                                            'lastname': { 'type': 'string', 'minLength': 2 }
                                        }
                                    } 
                                }"),
                         FormFactory.NumberDisplay("Aantal:", "$.items"),
                         FormFactory.ListComponent("Items", "$.items",
                             FormFactory.HorizontalLayout(
                                 FormFactory.StringComponent("Firstname", "@.firstname"),
                                 FormFactory.StringComponent("Lastname", "@.lastname"),
                                 FormFactory.DeleteAction("Delete", "@")
                                 )
                             )
                     ),
                FormFactory.GroupLayout(
                         FormFactory.AddAction("Add person", "$.persons", new ExpandoObject())
                            .AddSchemaRule(RuleEffect.HIDE, "$.persons", "{ 'type': 'array', 'maxItems': 2 }"),
                         FormFactory.NumberDisplay("Aantal:", "$.persons"),
                         FormFactory.ListComponent("Persons", "$.persons",
                             FormFactory.HorizontalLayout(
                                 FormFactory.StringComponent("Firstname", "@.firstname"),
                                 FormFactory.StringComponent("Lastname", "@.lastname"),
                                 FormFactory.DeleteAction("Delete", "@")
                                 )
                             )
                     )
                );
        }
    }
}
