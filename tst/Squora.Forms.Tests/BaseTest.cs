using Squora.Forms.Contracts;
using Squora.Forms.UISchema.Helpers;
using Squora.Forms.UISchema.Implementations;
using Squora.Forms.UISchema.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using NSubstitute;

namespace Squora.Forms.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var _scopeProviderMock = Substitute.For<IScopeProvider>();

            var uiSchema = JToken.Parse("{\"type\":\"VerticalLayout\",\"elements\":[{\"type\":\"Control\",\"scope\":\"#/properties/name\"},{\"type\":\"HorizontalLayout\",\"elements\":[{\"type\":\"Control\",\"scope\":\"#/properties/age\"},{\"type\":\"Control\",\"scope\":\"#/properties/country\"}]}]}");
            var result = new UISchemaParser(_scopeProviderMock).ParseUISchema(uiSchema);
            Assert.Pass();
        }

        [Test]
        [TestCase("First Name")]
        public void TestControl(string label)
        {
            // Arrange
            var _scopeProviderMock = Substitute.For<IScopeProvider>();

            var scope = "#/properties/name";
            var uiSchema = JToken.Parse("{\"type\":\"Control\",\"label\":\"" + label + "\",\"scope\":\"" + scope + "\", \"options\": {\"detail\" : \"Generated\", \"readonly\" : true, \"elementLabelProp\": \"name\",\"showSortButtons\": true, \"format\": \"radio\"  }}");

            //Act
            var result = new UISchemaParser(_scopeProviderMock).ParseUISchema(uiSchema).ToList();

            // Assert
            var item = result[0] as ControlDescriptionItem;
            Assert.That(item.Scope, Is.EqualTo(scope));
            Assert.That(item.Options.Detail.DetailType, Is.EqualTo("Generated"));
            Assert.That(item.Options.ElementLabelProperty, Is.EqualTo("name"));
            Assert.That(item.Options.Readonly, Is.EqualTo(true));
            Assert.That(item.Options.ShowSortButtons, Is.EqualTo(true));
            Assert.That(item.Options.Format, Is.EqualTo("radio"));
            Assert.That(item.Label, Is.EqualTo(label));
            Assert.That(item.ShowLabel, Is.EqualTo(true));
        }

        [Test]
        public void TestControlLabelIsFalse()
        {
            // Arrange
            var _scopeProviderMock = Substitute.For<IScopeProvider>();

            var scope = "#/properties/name";
            var uiSchema = JToken.Parse("{\"type\":\"Control\",\"label\": false, \"scope\":\"" + scope + "\", \"options\": {\"detail\" : \"Generated\", \"readonly\" : true, \"elementLabelProp\": \"name\",\"showSortButtons\": true, \"format\": \"radio\"  }}");

            // Act
            var result = new UISchemaParser(_scopeProviderMock).ParseUISchema(uiSchema).ToList();

            // Assert
            var item = result[0] as ControlDescriptionItem;
            Assert.That(item.Scope, Is.EqualTo(scope));
            Assert.That(item.Label, Is.EqualTo(string.Empty));
            Assert.That(item.ShowLabel, Is.EqualTo(false));
        }

        [Test]
        public void TestHorizontalLayout()
        {
            // Arrange
            var _scopeProviderMock = Substitute.For<IScopeProvider>();

            var uiSchema = JToken.Parse("{\"type\":\"HorizontalLayout\",\"elements\":[{\"type\":\"Control\",\"label\":\"Name\",\"scope\":\"#/properties/name\"},{\"type\":\"Control\",\"label\":\"Birth Date\",\"scope\":\"#/properties/birthDate\"}]}");

            // Act
            var result = new UISchemaParser(_scopeProviderMock).ParseUISchema(uiSchema).ToList();

            // Assert
            var item = result[0] as HorizontalLayoutDescriptionItem;

            Assert.That(item.Elements.Count, Is.EqualTo(2));

            var controlItem = item.Elements[0] as ControlDescriptionItem;
            Assert.That(controlItem.Scope, Is.EqualTo("#/properties/name"));
            Assert.That(controlItem.Label, Is.EqualTo("Name"));
            Assert.That(controlItem.ShowLabel, Is.EqualTo(true));
        }

        [Test]
        public void TestVerticalLayout()
        {
            // Arrange
            var _scopeProviderMock = Substitute.For<IScopeProvider>();

            var uiSchema = JToken.Parse("{\"type\":\"VerticalLayout\",\"elements\":[{\"type\":\"Control\",\"label\":\"Name\",\"scope\":\"#/properties/name\"},{\"type\":\"Control\",\"label\":\"Birth Date\",\"scope\":\"#/properties/birthDate\"}]}");

            // Act
            var result = new UISchemaParser(_scopeProviderMock).ParseUISchema(uiSchema).ToList();

            // Assert
            var item = result[0] as VerticalLayoutDescriptionItem;

            Assert.That(item.Elements.Count, Is.EqualTo(2));

            var controlItem = item.Elements[0] as ControlDescriptionItem;
            Assert.That(controlItem.Scope, Is.EqualTo("#/properties/name"));
            Assert.That(controlItem.Label, Is.EqualTo("Name"));
            Assert.That(controlItem.ShowLabel, Is.EqualTo(true));
        }

        [Test]
        public void TestGroupLayout()
        {
            // Arrange
            var _scopeProviderMock = Substitute.For<IScopeProvider>();

            var uiSchema = JToken.Parse("{\"type\":\"Group\",\"label\":\"My Group\",\"elements\":[{\"type\":\"Control\",\"label\":\"Name\",\"scope\":\"#/properties/name\"},{\"type\":\"Control\",\"label\":\"Birth Date\",\"scope\":\"#/properties/birthDate\"}]}");

            // Act
            var result = new UISchemaParser(_scopeProviderMock).ParseUISchema(uiSchema).ToList();

            // Assert
            var item = result[0] as GroupDescriptionItem;

            Assert.That(item.Elements.Count, Is.EqualTo(2));
            Assert.That(item.Label, Is.EqualTo("My Group"));

            var controlItem = item.Elements[0] as ControlDescriptionItem;
            Assert.That(controlItem.Scope, Is.EqualTo("#/properties/name"));
            Assert.That(controlItem.Label, Is.EqualTo("Name"));
            Assert.That(controlItem.ShowLabel, Is.EqualTo(true));
        }

        [Test]
        public void TestCategorization()
        {
            // Arrange
            var _scopeProviderMock = Substitute.For<IScopeProvider>();

            var uiSchema = JToken.Parse("{\"type\":\"Categorization\",\"elements\":[{\"type\":\"Category\",\"label\":\"Personal Data\",\"elements\":[{\"type\":\"Control\",\"scope\":\"#/properties/firstName\"},{\"type\":\"Control\",\"scope\":\"#/properties/lastName\"},{\"type\":\"Control\",\"scope\":\"#/properties/age\"},{\"type\":\"Control\",\"scope\":\"#/properties/height\"},{\"type\":\"Control\",\"scope\":\"#/properties/dateOfBirth\"}]},{\"type\":\"Category\",\"label\":\"Dietary requirements\",\"elements\":[{\"type\":\"Control\",\"scope\":\"#/properties/diet\"},{\"type\":\"Control\",\"scope\":\"allergicToPeanuts\"}]}]}");

            // Act
            var result = new UISchemaParser(_scopeProviderMock).ParseUISchema(uiSchema).ToList();

            // Assert
            var item = result[0] as CategorizationDescriptionItem;

            Assert.That(item.Elements.Count, Is.EqualTo(2));

            var controlItem = item.Elements[0];
            Assert.That(controlItem.Label, Is.EqualTo("Personal Data"));
            Assert.That(controlItem.Elements.Count, Is.EqualTo(5));
        }

        [Test]
        public void ValidateInputAgainstSchema()
        {
            var uiSchema = JToken.Parse("{\"type\":\"Categorization\",\"elements\":[{\"type\":\"Category\",\"label\":\"Personal Data\",\"elements\":[{\"type\":\"Control\",\"scope\":\"#/properties/firstName\"},{\"type\":\"Control\",\"scope\":\"#/properties/lastName\"},{\"type\":\"Control\",\"scope\":\"#/properties/age\"},{\"type\":\"Control\",\"scope\":\"#/properties/height\"},{\"type\":\"Control\",\"scope\":\"#/properties/dateOfBirth\"}]},{\"type\":\"Category\",\"label\":\"Dietary requirements\",\"elements\":[{\"type\":\"Control\",\"scope\":\"#/properties/diet\"},{\"type\":\"Control\",\"scope\":\"allergicToPeanuts\"}]}]}");

            var schema = EmbededResourceHelper.ReadJsonFormsSchema();

            bool isValid = uiSchema.IsValid(schema);
        }
    }
}