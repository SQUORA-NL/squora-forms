using Squora.Forms.UISchema.Models;
using Newtonsoft.Json.Linq;

namespace Squora.Forms.UISchema.Contracts;

public interface IUISchemaParser
{
    public IEnumerable<ILayoutDescriptionItem> ParseUISchema(JToken UISchema);
}