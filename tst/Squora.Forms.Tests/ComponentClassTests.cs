using Squora.Forms.Blazor.Components.Factories;
using Squora.Forms.Core.Parser;

namespace Squora.Forms.Tests
{
    public class ComponentClassTests
    {
        [Test]
        public void WithClass_EndsUpInTheModel()
        {
            var model = FormFactory.StringComponent("Email", "$.email", b => b.WithClass("sq-highlight"));

            Assert.That(model.Parameters.Entries["Class"], Is.EqualTo("sq-highlight"));
        }

        [Test]
        public void NoClass_LeavesTheModelWithoutOne()
        {
            var model = FormFactory.StringComponent("Email", "$.email");

            Assert.That(model.Parameters.Entries.ContainsKey("Class"), Is.False);
        }

        [Test]
        public void TheClass_SurvivesAJsonRoundTrip()
        {
            var parser = new DynamicFormModelParser();
            var form = FormFactory.VerticalLayout(
                FormFactory.StringComponent("Email", "$.email", b => b.WithClass("sq-highlight")));

            var back = parser.Deserialize(parser.Serialize(form));

            Assert.That(back.Elements[0].Parameters.Entries["Class"], Is.EqualTo("sq-highlight"));
        }
    }
}
