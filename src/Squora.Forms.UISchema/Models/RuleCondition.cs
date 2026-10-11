using Newtonsoft.Json.Linq;

namespace Squora.Forms.UISchema.Models;

public class RuleCondition
{
    public string Scope { get; set; }

    /// <summary>
    /// The schema property is a standard JSON schema object. 
    /// This means, everything that can be specified using JSON schema can be used in the rule condition.
    /// The schema is validated against the data specified in the scope property.
    /// </summary>
    public JToken Schema { get; set; }
}
