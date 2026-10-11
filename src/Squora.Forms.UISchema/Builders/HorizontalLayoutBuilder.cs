using Squora.Forms.UISchema.Models;

namespace Squora.Forms.UISchema.Builders;

internal class HorizontalLayoutBuilder
{
    private IEnumerable<ILayoutDescriptionItem> _items;
    private RuleItem? _ruleItem;
    
    internal HorizontalLayoutDescriptionItem Build()
    {
        if (ValidateBuild().Success)
        {
            var result = new HorizontalLayoutDescriptionItem();
            result.Elements = _items.ToList();
            result.Rule = _ruleItem;
            return result;
        }
        throw new ArgumentException();
    }

    private ValidationResult ValidateBuild()
    {
        return ValidationResult.ValidResult();
    }

    public HorizontalLayoutBuilder WithElements(IEnumerable<ILayoutDescriptionItem> items)
    {
        _items = items;
        return this;
    }

    public HorizontalLayoutBuilder WithRule(RuleItem ruleItem)
    {
        _ruleItem = ruleItem;
        return this;
    }
}