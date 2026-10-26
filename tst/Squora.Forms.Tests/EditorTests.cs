using Squora.Forms.Blazor.Components;
using Squora.Forms.Blazor.Components.Factories;
using Squora.Forms.Blazor.Services;
using Squora.Forms.Blazor.Utillities;
using Squora.Forms.Core.Models;
using Squora.Forms.Core.Parser;

namespace Squora.Forms.Tests
{
    public class EditorTests
    {
        private class Container : FormComponentBase
        {
            public Container(DynamicFormModel model) { FormModel = model; }
            public override string ValidationString => string.Empty;
        }

        [Test]
        public void ModelChanged_IsRaisedToSubscribers()
        {
            var selection = new SelectionStateService();
            var raised = 0;
            selection.ModelChanged += () => raised++;

            selection.NotifyModelChanged();

            Assert.That(raised, Is.EqualTo(1));
        }

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

        [Test]
        public void UniquePath_AddsNumberWhenTaken()
        {
            var root = FormFactory.VerticalLayout(
                FormFactory.StringComponent("A", "$.newField"),
                FormFactory.VerticalLayout(FormFactory.StringComponent("B", "$.newField2")));

            Assert.That(UniquePath.For(root, "$.other"), Is.EqualTo("$.other"));
            Assert.That(UniquePath.For(root, "$.newField"), Is.EqualTo("$.newField3"));
        }

        [Test]
        public void ToolboxDrop_InsertsAtIndex()
        {
            var model = FormFactory.VerticalLayout(FormFactory.StringComponent("A", "$.a"), FormFactory.StringComponent("B", "$.b"));
            var target = new Container(model);
            var drag = new DragStateService();
            var added = FormFactory.StringComponent("New", "$.n");

            drag.StartToolboxDrag(added);

            Assert.That(drag.ExecuteDrop(target, 1), Is.True);
            Assert.That(model.Elements[1], Is.SameAs(added));
            Assert.That(model.Elements, Has.Count.EqualTo(3));
        }

        [Test]
        public void MoveWithinContainer_ReordersElements()
        {
            var a = FormFactory.StringComponent("A", "$.a");
            var b = FormFactory.StringComponent("B", "$.b");
            var c = FormFactory.StringComponent("C", "$.c");
            var model = FormFactory.VerticalLayout(a, b, c);
            var target = new Container(model);
            var drag = new DragStateService();

            drag.StartDrag(a, target, 0);

            // dropping "after C" is index 3
            Assert.That(drag.ExecuteDrop(target, 3), Is.True);
            Assert.That(model.Elements, Is.EqualTo(new[] { b, c, a }));
        }

        [Test]
        public void LayoutCannotBeDroppedIntoItself_OrItsDescendants()
        {
            var inner = FormFactory.VerticalLayout();
            var outer = FormFactory.VerticalLayout(inner);
            var root = FormFactory.VerticalLayout(outer);
            var rootContainer = new Container(root);
            var drag = new DragStateService();

            drag.StartDrag(outer, rootContainer, 0);

            Assert.That(drag.CanDrop(new Container(outer)), Is.False);
            Assert.That(drag.CanDrop(new Container(inner)), Is.False);
            Assert.That(drag.CanDrop(rootContainer), Is.True);
        }

        [Test]
        public void EmptyLayoutCannotBeDroppedIntoItself()
        {
            var empty = FormFactory.VerticalLayout();
            var root = FormFactory.VerticalLayout(empty);
            var drag = new DragStateService();

            drag.StartDrag(empty, new Container(root), 0);

            Assert.That(drag.CanDrop(new Container(empty)), Is.False);
        }
    }
}
