using Squora.Forms.Blazor.Components.Factories;
using Squora.Forms.Core.Models;
using Squora.Forms.Core.Parser;

namespace Squora.Forms.Tests
{
    public class FormModelRoundTripTests
    {
        [Test]
        public void NestedFormModelParameter_SurvivesJsonRoundTrip()
        {
            var root = FormFactory.GroupLayout(group => group
                .WithParameter("HeaderModel", FormFactory.StringDisplay("Name", "$.name"))
                .WithElements(FormFactory.StringComponent("Name", "$.name")));
            var parser = new DynamicFormModelParser();

            var copy = parser.Deserialize(parser.Serialize(root))!;

            Assert.That(copy.Parameters.Entries["HeaderModel"], Is.TypeOf<DynamicFormModel>());
        }
    }
}
