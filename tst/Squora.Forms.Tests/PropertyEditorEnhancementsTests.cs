using Squora.Forms.Core.Models;
using Squora.Forms.Core.Parser;
using Squora.Forms.Blazor.Components.Factories;

namespace Squora.Forms.Tests
{
    public class PropertyEditorEnhancementsTests
    {
        [Test]
        public void DeleteElement_ShouldRemoveElementFromParent()
        {
            // Arrange
            var parent = FormFactory.VerticalLayout(
                FormFactory.StringComponent("First Name", "$.firstName"),
                FormFactory.StringComponent("Last Name", "$.lastName")
            );

            var elementToDelete = parent.Elements[0]; // First Name element
            var initialCount = parent.Elements.Count;

            // Act
            var result = RemoveElementFromParent(parent, elementToDelete);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(parent.Elements.Count, Is.EqualTo(initialCount - 1));
            Assert.That(parent.Elements.Contains(elementToDelete), Is.False);
            Assert.That(GetParameterValue(parent.Elements[0], "Label"), Is.EqualTo("Last Name"));
        }

        [Test]
        public void DeleteElement_ShouldRemoveFromNestedParent()
        {
            // Arrange
            var root = FormFactory.VerticalLayout(
                FormFactory.GroupLayout(group =>
                    group.WithParameter("Label", "Personal Info")
                         .WithElements(
                             FormFactory.StringComponent("First Name", "$.firstName"),
                             FormFactory.StringComponent("Last Name", "$.lastName")
                         )
                )
            );

            var groupElement = root.Elements[0];
            var elementToDelete = groupElement.Elements[0]; // First Name inside group

            // Act
            var result = RemoveElementFromParent(root, elementToDelete);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(groupElement.Elements.Count, Is.EqualTo(1));
            Assert.That(groupElement.Elements.Contains(elementToDelete), Is.False);
            Assert.That(GetParameterValue(groupElement.Elements[0], "Label"), Is.EqualTo("Last Name"));
        }

        [Test]
        public void JsonSerialization_ShouldSerializeAndDeserializeCorrectly()
        {
            // Arrange
            var parser = new DynamicFormModelParser();
            var element = FormFactory.StringComponent("Test Field", "$.test");
            element.Parameters.Add("Required", true);
            element.Parameters.Add("MinimumLength", 5);

            // Act
            var json = parser.Serialize(element);
            var deserializedElement = parser.Deserialize(json);

            // Assert
            Assert.That(deserializedElement, Is.Not.Null);
            Assert.That(GetParameterValue(deserializedElement, "Label"), Is.EqualTo("Test Field"));
            Assert.That(GetParameterValue(deserializedElement, "Path"), Is.EqualTo("$.test"));
            Assert.That(GetBooleanParameter(deserializedElement, "Required"), Is.True);
            Assert.That(GetParameterValue(deserializedElement, "MinimumLength"), Is.EqualTo("5"));
        }

        [Test]
        public void ReplaceElement_ShouldUpdateElementInParent()
        {
            // Arrange
            var parent = FormFactory.VerticalLayout(
                FormFactory.StringComponent("Old Field", "$.old"),
                FormFactory.StringComponent("Other Field", "$.other")
            );

            var oldElement = parent.Elements[0];
            var newElement = FormFactory.StringComponent("New Field", "$.new");

            // Act
            var result = ReplaceElementInParent(parent, oldElement, newElement);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(parent.Elements.Count, Is.EqualTo(2));
            Assert.That(parent.Elements[0], Is.EqualTo(newElement));
            Assert.That(GetParameterValue(parent.Elements[0], "Label"), Is.EqualTo("New Field"));
            Assert.That(GetParameterValue(parent.Elements[1], "Label"), Is.EqualTo("Other Field"));
        }

        // Helper methods (copied from PropertyEditor logic)
        private bool RemoveElementFromParent(DynamicFormModel parent, DynamicFormModel elementToRemove)
        {
            if (parent.Elements.Contains(elementToRemove))
            {
                parent.Elements.Remove(elementToRemove);
                return true;
            }

            foreach (var child in parent.Elements)
            {
                if (RemoveElementFromParent(child, elementToRemove))
                {
                    return true;
                }
            }

            return false;
        }

        private bool ReplaceElementInParent(DynamicFormModel parent, DynamicFormModel oldElement, DynamicFormModel newElement)
        {
            var index = parent.Elements.IndexOf(oldElement);
            if (index >= 0)
            {
                parent.Elements[index] = newElement;
                return true;
            }

            foreach (var child in parent.Elements)
            {
                if (ReplaceElementInParent(child, oldElement, newElement))
                {
                    return true;
                }
            }

            return false;
        }

        private string GetParameterValue(DynamicFormModel element, string parameterName)
        {
            if (element.Parameters.Entries.TryGetValue(parameterName, out var value))
            {
                return value?.ToString() ?? "";
            }
            return "";
        }

        private bool GetBooleanParameter(DynamicFormModel element, string parameterName)
        {
            if (element.Parameters.Entries.TryGetValue(parameterName, out var value))
            {
                return value is bool boolValue && boolValue;
            }
            return false;
        }
    }
}