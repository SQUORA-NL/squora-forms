using Squora.Forms.UISchema.Models;

namespace Squora.Forms.UISchema.Builders;

internal class VerticalLayoutBuilder
{
    private IEnumerable<ILayoutDescriptionItem> _items;
    private RuleItem? _ruleItem;

    internal VerticalLayoutDescriptionItem Build()
    {
        if (ValidateBuild().Success)
        {
            var result = new VerticalLayoutDescriptionItem();
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

    public VerticalLayoutBuilder WithElements(IEnumerable<ILayoutDescriptionItem> items)
    {
        _items = items;
        return this;
    }

    public VerticalLayoutBuilder WithRule(RuleItem ruleItem)
    {
        _ruleItem = ruleItem;
        return this;
    }
}