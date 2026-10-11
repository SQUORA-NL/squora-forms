using Squora.Forms.UISchema.Models;

namespace Squora.Forms.UISchema.Builders
{
    internal class ControlItemBuilder
    {
        private ControlOptions _options;
        private string? _scope;
        private string? _label;
        private bool _showLabel;
        private Dictionary<string, object> _scopeMetadata;
        private RuleItem? _ruleItem;
        
        internal ControlDescriptionItem Build()
        {

            if (ValidateBuild().Success)
            {
                var result = new ControlDescriptionItem();
                result.Scope = _scope;
                result.Options = _options;
                result.Label = _label;
                result.ShowLabel = _showLabel;
                result.ScopeMetadata = _scopeMetadata;
                result.Rule = _ruleItem;
                return result;

            }
            throw new ArgumentException();
        }

        private ValidationResult ValidateBuild()
        {
            return ValidationResult.ValidResult();
        }


        public ControlItemBuilder WithDefaultOptions()
        {
            _options = ControlOptions.Default();
            return this;
        }

        internal ControlItemBuilder WithScope(string scope)
        {
            _scope = scope;
            return this;
        }
        
        internal ControlItemBuilder WithLabel(string? label, bool showLabel = true)
        {
            _label = label;
            _showLabel = showLabel;
            return this;
        }

        internal ControlItemBuilder WithOptions(ControlOptions? options)
        {
            _options = options;
            return this;
        }
        
        internal ControlItemBuilder WithScopeMetadata(Dictionary<string,object> scopeMetadata)
        {
            _scopeMetadata = scopeMetadata;
            return this;
        }

        internal ControlItemBuilder WithRule(RuleItem? ruleItem)
        {
            _ruleItem = ruleItem;
            return this;
        }
    }
}
