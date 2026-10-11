using Squora.Forms.Contracts;
using Squora.Forms.UISchema.Builders;
using Squora.Forms.UISchema.Constants;
using Squora.Forms.UISchema.Contracts;
using Squora.Forms.UISchema.Converters;
using Squora.Forms.UISchema.Enums;
using Squora.Forms.UISchema.Models;
using Newtonsoft.Json.Linq;

namespace Squora.Forms.UISchema.Implementations
{
    public class UISchemaParser : IUISchemaParser
    {
        private readonly IScopeProvider _scopeProvider;
        private const string SchemaType = "type";
        private const string SchemaElements = "elements";

        public UISchemaParser(IScopeProvider scopeProvider)
        {
            _scopeProvider = scopeProvider;
        }
        
        public IEnumerable<ILayoutDescriptionItem> ParseUISchema(JToken UISchema)
        {
            var result = new List<ILayoutDescriptionItem>();
            ProcessUISchema(UISchema, result);
            return result;
        }

        private void ProcessUISchema(JToken schema, List<ILayoutDescriptionItem> items)
        {
            if (schema.Type == JTokenType.Array)
            {
                foreach (var arrayItem in schema.Children())
                {
                    ProcessUISchema(arrayItem, items);
                }
            }
            else
            {
                var itemType = Enum.Parse<LayoutType>(schema[SchemaType].ToString());
                switch (itemType)
                {
                    case LayoutType.Control:
                        items.Add(HandleControlItem(schema));
                        break;
                    case LayoutType.HorizontalLayout:
                        items.Add(HandleHorizontalLayoutItem(schema));
                        break;
                    case LayoutType.VerticalLayout:
                        items.Add(HandleVerticalLayoutItem(schema));
                        break;
                    case LayoutType.Group:
                        items.Add(HandleGroupItem(schema));
                        break;
                    case LayoutType.Categorization:
                        items.Add(HandleCategorizationItem(schema));
                        break;
                    case LayoutType.Category:
                        items.Add(HandleCategoryItem(schema));
                        break;
                    case LayoutType.ListWithDetail:
                        items.Add(new ControlDescriptionItem());
                        break;
                    default:
                        throw new Exception("Type is not supported");
                }
            }
        }

        private ControlDescriptionItem HandleControlItem(JToken token)
        {
            var label = FormUISchemaConverters.ConvertToControlLabel(token[UISchemaConstants.Label]);
            var scope = token[UISchemaConstants.Scope].ToString();

            var options = FormUISchemaConverters.ConvertControlOptions(token[UISchemaConstants.Options]);
            var optionsDetail = token.SelectToken(UISchemaConstants.Options + "." + UISchemaConstants.Detail);

            //var optionsDetail = token[UISchemaConstants.Options][UISchemaConstants.Detail];
            if (optionsDetail != null)
            {
                DetailOption detailOption = null;
                if (optionsDetail.Type == JTokenType.String)
                {
                    var detail = optionsDetail.ToString();
                    detailOption = new DetailOption(detail);
                }
                if (optionsDetail.Type == JTokenType.Object)
                {
                    var items = new List<ILayoutDescriptionItem>();
                    ProcessUISchema(optionsDetail, items);
                    detailOption = new DetailOption("InlinedUISchema", items);
                }
                options.WithDetail(detailOption);
            }


            var ruleItem = HandleRuleItem(token[UISchemaConstants.Rule]);
            var result = new ControlItemBuilder()
                .WithScope(scope)
                .WithLabel(label.Label, label.ShowLabel)
                .WithOptions(options)
                .WithScopeMetadata(_scopeProvider.GetDataFromScope(scope))
                .WithRule(ruleItem)
                ;
            return result.Build();
        }

        private HorizontalLayoutDescriptionItem HandleHorizontalLayoutItem(JToken token)
        {
            var items = new List<ILayoutDescriptionItem>();
            ProcessUISchema(token[SchemaElements], items);

            var ruleItem = HandleRuleItem(token[UISchemaConstants.Rule]);
            var result = new HorizontalLayoutBuilder()
                .WithElements(items)
                .WithRule(ruleItem);
            
            return result.Build();
        }
        
        private VerticalLayoutDescriptionItem HandleVerticalLayoutItem(JToken token)
        {
            var items = new List<ILayoutDescriptionItem>();
            ProcessUISchema(token[SchemaElements], items);

            var ruleItem = HandleRuleItem(token[UISchemaConstants.Rule]);
            var result = new VerticalLayoutBuilder()
                .WithElements(items)
                .WithRule(ruleItem);

            return result.Build();
        }
        
        private GroupDescriptionItem HandleGroupItem(JToken token)
        {
            var items = new List<ILayoutDescriptionItem>();
            ProcessUISchema(token[SchemaElements], items);

            var ruleItem = HandleRuleItem(token[UISchemaConstants.Rule]);
            var result = new GroupLayoutBuilder()
                .WithLabel(token[UISchemaConstants.Label].ToString())
                .WithElements(items)
                .WithRule(ruleItem)
                ;

            return result.Build();
        }

        private RuleItem? HandleRuleItem(JToken? token)
        {
            if (token == null) return null;
            
            var condition = token[UISchemaConstants.Condition].ToObject<RuleCondition>();

            var result = new RuleItemBuilder()
                .WithRuleEffect(token[UISchemaConstants.Effect].ToString())
                .WithCondition(condition)
                .Build();

            return result;
        }
        
        private CategorizationDescriptionItem HandleCategorizationItem(JToken token)
        {
            var items = new List<ILayoutDescriptionItem>();
            ProcessUISchema(token[SchemaElements], items);

            var ruleItem = HandleRuleItem(token[UISchemaConstants.Rule]);
            var castedItem = items.Cast<CategoryDescriptionItem>().ToList();
            var result = new CategorizationBuilder()
                .WithElements(castedItem)
                .WithRule(ruleItem)
                ;

            return result.Build();
        }
        
        private CategoryDescriptionItem HandleCategoryItem(JToken token)
        {
            var items = new List<ILayoutDescriptionItem>();
            ProcessUISchema(token[SchemaElements], items);

            var result = new CategoryBuilder()
                .WithLabel(token[UISchemaConstants.Label].ToString())
                .WithElements(items);
            
            return result.Build();
        }
    }
}