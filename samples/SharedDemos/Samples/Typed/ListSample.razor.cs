using Squora.Forms.Core.Models;
using Squora.Forms.Blazor.Components.Factories;
using SharedDemos.Samples.Models;

namespace SharedDemos.Samples.Typed
{
    public partial class ListSample : SampleBasePage
    {
        private void Update()
        {
            StateHasChanged();
        }

 
        private DynamicFormModel model = FormFactory.GroupLayout(gr => gr
                .WithLabel("List sample")
                ,
                FormFactory.StringComponent("Name", "$.Name", name => name.WithMinimumLength(3)),
                FormFactory.AddAction("Add", "$.Names", "zero"),
                FormFactory.ListComponent("Names", "$.Names",
                     FormFactory.HorizontalLayout(
                         FormFactory.StringComponent("Text", "@"),
                         FormFactory.DeleteAction("Delete", "@")
                         )
                    )
               );

        private ListContainer data = new ListContainer()
        {
            Name = "Demo of primative list",
            Names = new List<string> { "one", "two", "three" }
        };
    }
}
