using Squora.Forms.Common.Enums;
using Squora.Forms.JsonSchema.Implementations;
using Newtonsoft.Json.Linq;

namespace Squora.Forms.Tests
{
    public class JsonSchemaTests
    {
        [Test]
        public void CanHandleSimpleSchema()
        {
            var jsonSchema = File.ReadAllText("./Schemas/SimpleSchema.json");
            JObject schemaObject = JObject.Parse(jsonSchema);
            var schemaReader = new SchemaReader();

            var type = schemaReader.GetTypeFromPath(schemaObject, "#/properties/firstName");

            Assert.That(type.Type, Is.EqualTo(ComponentType.String));
        }

        [Test]
        public void CanHandleObjectJsonSchemaPath()
        {
            var jsonSchema = File.ReadAllText("./Schemas/SchemaWithObject.json");
            JObject schemaObject = JObject.Parse(jsonSchema);

            var schemaReader = new SchemaReader();

            var type = schemaReader.GetTypeFromPath(schemaObject, "#/properties/work/properties/id");
            var objectPath = schemaReader.GetTypeFromPath(schemaObject, "#/properties/work/properties/composer");
            var arrayPath = schemaReader.GetTypeFromPath(schemaObject, "#/properties/work/properties/composer/properties/functions");
            var stringArrayItemPath = schemaReader.GetTypeFromPath(schemaObject, "#/properties/work/properties/composer/properties/functions/items");

            var recordingArtistsName = schemaReader.GetTypeFromPath(schemaObject, "#/recording_artists/items/name");
            var recordingArtistsFunctionsItem =
                schemaReader.GetTypeFromPath(schemaObject, "#/properties/recording_artists/items/functions/items");
            var recordingArtistsFunctions =
                schemaReader.GetTypeFromPath(schemaObject, "#/properties/recording_artists/items/functions");

            Assert.That(type.Type, Is.EqualTo(ComponentType.Number));
            Assert.That(objectPath.Type, Is.EqualTo(ComponentType.Object));
            Assert.That(arrayPath.Type, Is.EqualTo(ComponentType.Array));
            Assert.That(stringArrayItemPath.Type, Is.EqualTo(ComponentType.String));


            Assert.That(recordingArtistsName.Type, Is.EqualTo(ComponentType.String));
            Assert.That(recordingArtistsFunctionsItem.Type, Is.EqualTo(ComponentType.String));
            Assert.That(recordingArtistsFunctions.Type, Is.EqualTo(ComponentType.Array));
        }
    }
}