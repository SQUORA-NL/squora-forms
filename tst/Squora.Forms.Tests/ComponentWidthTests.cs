using Squora.Forms.Blazor.Components.Factories;
using Squora.Forms.Core.Parser;

namespace Squora.Forms.Tests
{
    public class ComponentWidthTests
    {
        [Test]
        public void WithWidth_OnABuilder_EndsUpInTheModel()
        {
            var model = FormFactory.StringComponent("Nr", "$.nr", b => b.WithWidth("10%"));

            Assert.That(model.Parameters.Entries["Width"], Is.EqualTo("10%"));
        }

        [Test]
        public void WithWidth_OnAFinishedComponent_EndsUpInTheModel()
        {
            var model = FormFactory.DropDownComponent("Title", "$.title", new Dictionary<object, string> { ["mr"] = "Mr" }).WithWidth("15%");

            Assert.That(model.Parameters.Entries["Width"], Is.EqualTo("15%"));
        }

        [Test]
        public void WithWidth_OnALayout_EndsUpInTheModel()
        {
            var model = FormFactory.GroupLayout(group => group.WithWidth("70%"));

            Assert.That(model.Parameters.Entries["Width"], Is.EqualTo("70%"));
        }

        [Test]
        public void NoWidth_LeavesTheModelWithoutOne()
        {
            var model = FormFactory.StringComponent("Nr", "$.nr");

            Assert.That(model.Parameters.Entries.ContainsKey("Width"), Is.False);
        }

        [Test]
        public void TheWidths_SurviveAJsonRoundTrip()
        {
            var parser = new DynamicFormModelParser();
            var form = FormFactory.HorizontalLayout(
                FormFactory.StringComponent("Nr", "$.nr", b => b.WithWidth("10%")),
                FormFactory.StringComponent("Description", "$.description", b => b.WithWidth("90%")));

            var back = parser.Deserialize(parser.Serialize(form));

            Assert.That(back.Elements[0].Parameters.Entries["Width"], Is.EqualTo("10%"));
            Assert.That(back.Elements[1].Parameters.Entries["Width"], Is.EqualTo("90%"));
        }
    }
}
