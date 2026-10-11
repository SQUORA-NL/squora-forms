using Squora.Forms.UISchema.Exceptions;
using Squora.Forms.UISchema.Models;

namespace Squora.Forms.UISchema.Builders;

internal class CategorizationBuilder
{
    private IEnumerable<CategoryDescriptionItem> _items;
    private RuleItem _ruleItem;
    internal CategorizationDescriptionItem Build()
    {
        var validation = ValidateBuild();
        if (validation.Success)
        {
            var result = new CategorizationDescriptionItem();
            result.Elements = _items.ToList();
            result.Rule = _ruleItem;
            return result;
        }
        throw BuilderValidationException.Create(validation);
    }

    private ValidationResult ValidateBuild()
    {
        var result = ValidationResult.ValidResult();
       if(_items == null)
        {
            result.InvalidateResult("No items are provided");
        }
       return result;
    }

    public CategorizationBuilder WithElements(IEnumerable<CategoryDescriptionItem> items)
    {
        _items = items;
        return this;
    }

    internal CategorizationBuilder WithRule(RuleItem? ruleItem)
    {
        _ruleItem = ruleItem;
        return this;
    }
}