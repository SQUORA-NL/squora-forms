using Newtonsoft.Json.Linq;

namespace Squora.Forms.Core.Models;

public class DynamicFormModelRuleCondition
{
    public string Scope { get; set; }

    public JToken Schema { get; set; }

    public DynamicFormModelRuleCondition(string scope, JToken schema)
    {
        Scope = scope;
        Schema = schema;
    }
}