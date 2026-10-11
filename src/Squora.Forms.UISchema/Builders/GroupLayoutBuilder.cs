using Squora.Forms.UISchema.Models;

namespace Squora.Forms.UISchema.Builders;

internal class GroupLayoutBuilder
{
    private IEnumerable<ILayoutDescriptionItem> _items;
    private string _label;
    private RuleItem? _ruleItem;
    internal GroupDescriptionItem Build()
    {
        if (ValidateBuild().Success)
        {
            var result = new GroupDescriptionItem();
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

    public GroupLayoutBuilder WithElements(IEnumerable<ILayoutDescriptionItem> items)
    {
        _items = items;
        return this;
    }
    
    internal GroupLayoutBuilder WithLabel(string label)
    {
        _label = label;
        return this;
    }

    internal GroupLayoutBuilder WithRule(RuleItem? ruleItem)
    {
        _ruleItem = ruleItem;
        return this;
    }
}