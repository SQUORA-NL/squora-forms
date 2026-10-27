using Squora.Forms.Blazor.Components.Factories;
using Squora.Forms.Blazor.Services;
using Squora.Forms.Blazor.Utillities;
using Squora.Forms.Common.Enums;
using Squora.Forms.Core.Models;

namespace Squora.Forms.Tests
{
    /// <summary>What the action bar on a selected element, and a click on a toolbox item, do to the form.</summary>
    public class EditorActionsTests
    {
        private static (DynamicFormModel root, DynamicFormModel a, DynamicFormModel group, DynamicFormModel inner, SelectionStateService selection) Form()
        {
            var a = FormFactory.StringComponent("A", "$.a");
            var inner = FormFactory.StringComponent("Inner", "$.inner");
            var group = FormFactory.GroupLayout(g => g.WithLabel("Group"), inner);
            var root = FormFactory.VerticalLayout(a, group);
            return (root, a, group, inner, new SelectionStateService { RootModel = root });
        }

        [Test]
        public void FindParent_OfDirectChildIsTheRoot_OfRootIsNothing()
        {
            var (root, a, group, inner, _) = Form();

            Assert.That(FormTree.FindParent(root, a), Is.SameAs(root));
            Assert.That(FormTree.FindParent(root, inner), Is.SameAs(group));
            Assert.That(FormTree.FindParent(root, root), Is.Null);
        }

        [Test]
        public void DeleteSelected_RemovesTheElementAndClearsTheSelection()
        {
            var (root, a, _, _, selection) = Form();
            var raised = 0;
            selection.ModelChanged += () => raised++;
            selection.SelectElement(a);

            Assert.That(selection.DeleteSelected(), Is.True);

            Assert.That(root.Elements, Does.Not.Contain(a));
            Assert.That(selection.SelectedElement, Is.Null);
            Assert.That(raised, Is.EqualTo(1));
        }

        [Test]
        public void DeleteSelected_NeverDeletesTheForm()
        {
            var (root, _, _, _, selection) = Form();
            selection.SelectElement(root);

            Assert.That(selection.DeleteSelected(), Is.False);
            Assert.That(selection.SelectedElement, Is.SameAs(root));
        }

        [Test]
        public void DeleteSelected_TakesTheChildrenOfALayoutWithIt()
        {
            var (root, _, group, inner, selection) = Form();
            selection.SelectElement(group);

            selection.DeleteSelected();

            Assert.That(root.Elements, Does.Not.Contain(group));
            Assert.That(FormTree.FindParent(root, inner), Is.Null);
        }

        [Test]
        public void DuplicateSelected_PutsACopyRightAfterAndSelectsIt()
        {
            var (root, a, _, _, selection) = Form();
            selection.SelectElement(a);

            Assert.That(selection.DuplicateSelected(), Is.True);

            var copy = selection.SelectedElement!;
            Assert.That(copy, Is.Not.SameAs(a));
            Assert.That(root.Elements.IndexOf(copy), Is.EqualTo(root.Elements.IndexOf(a) + 1));
            Assert.That(copy.DynamicType.ComponentType, Is.EqualTo(ComponentType.String));
        }

        [Test]
        public void Duplicate_GivesTheCopyPathsOfItsOwn()
        {
            var (root, a, _, _, _) = Form();

            var copy = FormTree.Duplicate(root, a)!;

            Assert.That(copy.Parameters.Entries["Path"], Is.EqualTo("$.a2"));
            Assert.That(a.Parameters.Entries["Path"], Is.EqualTo("$.a"));
        }

        [Test]
        public void Duplicate_OfALayoutRenamesEveryPathInIt_AndKeepsSharedPathsShared()
        {
            var one = FormFactory.StringComponent("One", "$.x");
            var two = FormFactory.StringComponent("Two", "$.x");
            var other = FormFactory.StringComponent("Other", "$.y");
            var group = FormFactory.GroupLayout(g => g.WithLabel("G"), one, two, other);
            var root = FormFactory.VerticalLayout(group);

            var copy = FormTree.Duplicate(root, group)!;

            Assert.That(copy.Elements[0].Parameters.Entries["Path"], Is.EqualTo("$.x2"));
            Assert.That(copy.Elements[1].Parameters.Entries["Path"], Is.EqualTo("$.x2"));
            Assert.That(copy.Elements[2].Parameters.Entries["Path"], Is.EqualTo("$.y2"));
        }

        [Test]
        public void Duplicate_OfTheRoot_IsRefused()
        {
            var (root, _, _, _, _) = Form();

            Assert.That(FormTree.Duplicate(root, root), Is.Null);
        }

        [Test]
        public void Add_WithNothingSelected_GoesAtTheEndOfTheForm()
        {
            var (root, _, _, _, selection) = Form();
            var added = FormFactory.StringComponent("New", "$.new");

            selection.AddElement(added);

            Assert.That(root.Elements.Last(), Is.SameAs(added));
            Assert.That(selection.SelectedElement, Is.SameAs(added));
        }

        [Test]
        public void Add_WithAFieldSelected_GoesRightAfterIt()
        {
            var (root, a, _, _, selection) = Form();
            selection.SelectElement(a);
            var added = FormFactory.StringComponent("New", "$.new");

            selection.AddElement(added);

            Assert.That(root.Elements.IndexOf(added), Is.EqualTo(root.Elements.IndexOf(a) + 1));
        }

        [Test]
        public void Add_WithALayoutSelected_GoesInsideIt()
        {
            var (root, _, group, _, selection) = Form();
            selection.SelectElement(group);
            var added = FormFactory.StringComponent("New", "$.new");

            selection.AddElement(added);

            Assert.That(group.Elements.Last(), Is.SameAs(added));
            Assert.That(root.Elements, Does.Not.Contain(added));
        }

        [Test]
        public void Add_WithTheFormSelected_GoesAtTheEndOfTheForm()
        {
            var (root, _, _, _, selection) = Form();
            selection.SelectElement(root);
            var added = FormFactory.StringComponent("New", "$.new");

            selection.AddElement(added);

            Assert.That(root.Elements.Last(), Is.SameAs(added));
        }

        [Test]
        public void EditProperties_SelectsAndAsksForThePane()
        {
            var (_, a, _, _, selection) = Form();
            var requested = 0;
            selection.PropertiesRequested += () => requested++;

            selection.EditProperties(a);

            Assert.That(selection.SelectedElement, Is.SameAs(a));
            Assert.That(requested, Is.EqualTo(1));
            Assert.That(selection.FocusRequested, Is.True);
        }

        [Test]
        public void SelectParent_MovesUpOneLevel_AndStopsAtTheForm()
        {
            var (root, _, group, inner, selection) = Form();
            selection.SelectElement(inner);

            selection.SelectParent();
            Assert.That(selection.SelectedElement, Is.SameAs(group));

            selection.SelectParent();
            Assert.That(selection.SelectedElement, Is.SameAs(root));
            Assert.That(selection.CanSelectParent, Is.False);
        }

        [Test]
        public void Describe_UsesTheLabel_OtherwiseTheKindOfElement()
        {
            var labelled = FormFactory.StringComponent("City", "$.city");
            var plain = FormFactory.VerticalLayout();

            Assert.That(FormTree.Describe(labelled), Is.EqualTo("City"));
            Assert.That(FormTree.Describe(plain), Is.EqualTo("Vertical Layout"));
        }

        [Test]
        public void DragHover_OnlyTheLatestSpotCanClearIt()
        {
            var drag = new DragStateService();
            var layout = FormFactory.VerticalLayout();
            var first = new object();
            var second = new object();
            var raised = 0;
            drag.HoverChanged += () => raised++;

            drag.SetHover(first, layout, "Drop above A");
            drag.SetHover(second, layout, "Drop below A");
            drag.ClearHover(first); // the pointer already moved to the next spot: this late "leave" is ignored

            Assert.That(drag.HoverHint, Is.EqualTo("Drop below A"));
            Assert.That(drag.HoverContainer, Is.SameAs(layout));

            drag.ClearHover(second);

            Assert.That(drag.HoverContainer, Is.Null);
            Assert.That(raised, Is.EqualTo(3));
        }

        [Test]
        public void EndDrag_ClearsTheHover()
        {
            var drag = new DragStateService();
            drag.StartToolboxDrag(FormFactory.StringComponent("N", "$.n"));
            drag.SetHover(new object(), FormFactory.VerticalLayout(), "Drop here");

            drag.EndDrag();

            Assert.That(drag.HoverContainer, Is.Null);
            Assert.That(drag.HoverHint, Is.Null);
        }
    }
}
