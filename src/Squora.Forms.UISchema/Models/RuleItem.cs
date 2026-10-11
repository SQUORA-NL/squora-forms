using Squora.Forms.Common.Enums;

namespace Squora.Forms.UISchema.Models;

public class RuleItem
{
    /// <summary>
    /// The effect property determines what should happen to the attached UI schema element once the condition is met
    /// Current effects include HIDE, SHOW, DISABLE, ENABLE
    /// </summary>
    public RuleEffect Effect { get; set; }

    public RuleCondition Condition { get; set; }
}
