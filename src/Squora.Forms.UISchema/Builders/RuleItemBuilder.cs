using Squora.Forms.Common.Enums;
using Squora.Forms.UISchema.Models;

namespace Squora.Forms.UISchema.Builders
{
    internal class RuleItemBuilder
    {
        private RuleEffect _effect;
        private List<string> faults = new List<string>();
        private RuleCondition _ruleCondition;

        internal RuleItem Build()
        {
            if (ValidateBuild().Success)
            {
                var result = new RuleItem();
                result.Effect = _effect;
                result.Condition = _ruleCondition;
                return result;

            }
            throw new ArgumentException();
        }

        private ValidationResult ValidateBuild()
        {
            if (faults.Count > 0)
            {
                return ValidationResult.InvalidResult(string.Join("; ", faults));
            }
            return ValidationResult.ValidResult();
        }

        internal RuleItemBuilder WithRuleEffect(string effect)
        {
            if (string.IsNullOrWhiteSpace(effect))
            {
                faults.Add("effect cannot be empty.");
            }

            if (!IsValidEffect(effect))
            {
                string[] names = Enum.GetNames(typeof(RuleEffect));
                string enumList = string.Join(", ", names);
                faults.Add($"Effect must have one of the values: {enumList}");
            }

            _effect = (RuleEffect)Enum.Parse(typeof(RuleEffect), effect);
            return this;
        }

        internal RuleItemBuilder WithCondition(RuleCondition condition)
        {
            _ruleCondition = condition;
            return this;
        }

        private static bool IsValidEffect(string effect)
        {
            return Enum.IsDefined(typeof(RuleEffect), effect);
        }

    }
}
