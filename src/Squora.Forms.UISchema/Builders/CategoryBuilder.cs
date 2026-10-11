using Squora.Forms.UISchema.Models;

namespace Squora.Forms.UISchema.Builders;

internal class CategoryBuilder
{
    private IEnumerable<ILayoutDescriptionItem> _items;
    private string _label;
    private RuleItem _ruleItem;
    
    internal CategoryDescriptionItem Build()
    {
        if (ValidateBuild().Success)
        {
            var result = new CategoryDescriptionItem();
            result.Elements = _items.ToList();
            result.Label = _label;
            result.Rule = _ruleItem;
            return result;
        }
        throw new ArgumentException();
    }

    private ValidationResult ValidateBuild()
    {
        return ValidationResult.ValidResult();
    }

    public CategoryBuilder WithElements(IEnumerable<ILayoutDescriptionItem> items)
    {
        _items = items;
        return this;
    }
    
    internal CategoryBuilder WithLabel(string label)
    {
        _label = label;
        return this;
    }
    internal CategoryBuilder WithRule(RuleItem? ruleItem)
    {
        _ruleItem = ruleItem;
        return this;
    }
}