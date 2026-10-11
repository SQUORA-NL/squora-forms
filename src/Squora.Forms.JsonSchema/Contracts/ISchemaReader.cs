using Squora.Forms.JsonSchema.Models;
using Newtonsoft.Json.Linq;

namespace Squora.Forms.JsonSchema.Contracts;

public interface ISchemaReader
{
    ISchemaItem GetTypeFromPath(JObject schema, string schemaPath);
}