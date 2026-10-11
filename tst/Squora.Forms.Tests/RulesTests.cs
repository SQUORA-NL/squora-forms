using Squora.Forms.Common.Enums;
using Squora.Forms.Contracts;
using Squora.Forms.UISchema.Implementations;
using Squora.Forms.UISchema.Models;
using Newtonsoft.Json.Linq;
using NSubstitute;

namespace Squora.Forms.Tests
{
    [TestFixture]
    public class RulesTests
    {
        [Test]
        public void TestHorizontalLayout()
        {
            // Arrange
            var _scopeProviderMock = Substitute.For<IScopeProvider>();

            var uiSchema = File.ReadAllText("./DynamicFormSchemas/Rules/HL_With_Rule.json");
            var uiSchemaParsed = JToken.Parse(uiSchema);

            // Act
            var result = new UISchemaParser(_scopeProviderMock).ParseUISchema(uiSchemaParsed).ToList();

            // Assert
            var item = result[0] as HorizontalLayoutDescriptionItem;

            Assert.That(item.Elements.Count, Is.EqualTo(1));
            Assert.That(item.Rule, Is.Not.Null);
            Assert.That(item.Rule.Effect, Is.EqualTo(RuleEffect.HIDE));
        }

        [Test]
        public void TestVerticalLayout()
        {
            // Arrange
            var _scopeProviderMock = Substitute.For<IScopeProvider>();

            var uiSchema = File.ReadAllText("./DynamicFormSchemas/Rules/VL_With_Rule.json");
            var uiSchemaParsed = JToken.Parse(uiSchema);

            // Act
            var result = new UISchemaParser(_scopeProviderMock).ParseUISchema(uiSchemaParsed).ToList();

            // Assert
            var item = result[0] as VerticalLayoutDescriptionItem;

            Assert.That(item.Rule, Is.Not.Null);
            Assert.That(item.Rule.Effect, Is.EqualTo(RuleEffect.HIDE));
        }

        [Test]
        public void TestControl()
        {
            // Arrange
            var _scopeProviderMock = Substitute.For<IScopeProvider>();

            var uiSchema = File.ReadAllText("./DynamicFormSchemas/Rules/C_With_Rule.json");
            var uiSchemaParsed = JToken.Parse(uiSchema);

            // Act
            var result = new UISchemaParser(_scopeProviderMock).ParseUISchema(uiSchemaParsed).ToList();

            // Assert
            var item = result[0] as VerticalLayoutDescriptionItem;

        }
    }
}