using Squora.Forms.Core.Models;
using Newtonsoft.Json.Linq;

namespace Squora.Forms.Core.Contracts;

public interface IDynamicFormModelCreator
{
    public IEnumerable<DynamicFormModel> GenerateModels(JToken uiSchema);
}