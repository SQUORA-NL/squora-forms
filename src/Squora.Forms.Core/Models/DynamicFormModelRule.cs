using Squora.Forms.Common.Enums;

namespace Squora.Forms.Core.Models;

public class DynamicFormModelRule
{
    public RuleEffect Effect { get; }

    public DynamicFormModelRuleCondition Condition { get; }

    public DynamicFormModelRule(RuleEffect effect, DynamicFormModelRuleCondition condition)
    {
        Effect = effect;
        Condition = condition;
    }
}
