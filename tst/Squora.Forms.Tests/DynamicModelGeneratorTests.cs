using Squora.Forms.JsonSchema.Implementations;
using Squora.Forms.UISchema.Implementations;
using Squora.Forms.Core.Contracts;
using Newtonsoft.Json.Linq;

namespace Squora.Forms.Tests
{
    public class DynamicModelGeneratorTests
    {
        [Test]
        public void CanHandleSimpleSchema()
        {
            var uiSchemaText = File.ReadAllText("./DynamicFormSchemas/HorizontalLayout.json");
            var jsonSchemaText = File.ReadAllText("./DynamicFormSchemas/HorizontalLayoutSchema.json");

            var schema = JObject.Parse(jsonSchemaText);
            var sut = new DynamicFormModelCreator(
                new UISchemaParser(new JsonSchemaScopeProvider(new SchemaReader(), schema)));

            var uiSchema = JToken.Parse(uiSchemaText);
            var models = sut.GenerateModels(uiSchema);
        }
    }
}