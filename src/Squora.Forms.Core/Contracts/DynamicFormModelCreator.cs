using Squora.Forms.Common.Enums;
using Squora.Forms.Core.Models;
using Squora.Forms.UISchema.Contracts;
using Squora.Forms.UISchema.Enums;
using Squora.Forms.UISchema.Models;
using Squora.Forms.Core.Models.ParameterModels;
using Newtonsoft.Json.Linq;

namespace Squora.Forms.Core.Contracts;

public class DynamicFormModelCreator : IDynamicFormModelCreator
{
    private readonly IUISchemaParser _schemaParser;

    public DynamicFormModelCreator(IUISchemaParser schemaParser)
    {
        _schemaParser = schemaParser;
    }

    public IEnumerable<DynamicFormModel> GenerateModels(JToken uiSchema)
    {
        var result = _schemaParser.ParseUISchema(uiSchema);

        var dynamicFormModels = new List<DynamicFormModel>();
        GenerateModelsInternal(result, dynamicFormModels);
        return dynamicFormModels;
    }

    private List<DynamicFormModel> GenerateModelsInternal(IEnumerable<ILayoutDescriptionItem> descriptionItems, List<DynamicFormModel> result)
    {
        foreach (var item in descriptionItems)
        {
            switch (item.Type)
            {
                case LayoutType.HorizontalLayout:
                    var currentItem = item as HorizontalLayoutDescriptionItem;

                    var horizontalDynamicFormModel = new DynamicFormModel
                    {
                        DynamicType = new ComponentSelectionKey(ComponentType.HorizontalLayout),
                        Elements = GenerateModelsInternal(currentItem.Elements, new List<DynamicFormModel>()),
                    };

                    if (currentItem.Rule != null)
                    {
                        horizontalDynamicFormModel.Rules.Add(new DynamicFormModelRule(currentItem.Rule.Effect,
                        new DynamicFormModelRuleCondition(currentItem.Rule.Condition.Scope, currentItem.Rule.Condition.Schema)));
                    }

                    result.Add(horizontalDynamicFormModel);
                    break;
                case LayoutType.VerticalLayout:
                    var verticalLayout = item as VerticalLayoutDescriptionItem;
                    var verticalDynamicFormModel = new DynamicFormModel
                    {
                        DynamicType = new ComponentSelectionKey(ComponentType.VerticalLayout),
                        Elements = GenerateModelsInternal(verticalLayout.Elements, new List<DynamicFormModel>()),
                        Rules = new List<DynamicFormModelRule> { new DynamicFormModelRule(verticalLayout.Rule.Effect,
                        new DynamicFormModelRuleCondition(verticalLayout.Rule.Condition.Scope, verticalLayout.Rule.Condition.Schema)) }
                    };
                    result.Add(verticalDynamicFormModel);

                    if (verticalLayout.Rule != null)
                    {
                        verticalDynamicFormModel.Rules.Add(new DynamicFormModelRule(verticalLayout.Rule.Effect,
                        new DynamicFormModelRuleCondition(verticalLayout.Rule.Condition.Scope, verticalLayout.Rule.Condition.Schema)));
                    }
                    break;
                case LayoutType.Control:
                    var controlItem = item as ControlDescriptionItem;
                    var controlOptions = ControlOptions.Default();
                    // controlItem.ScopeMetadata.Add("Label");

                    var dynamicFormModel = new DynamicFormModel
                    {
                        DynamicType = new ComponentSelectionKey(Enum.Parse<ComponentType>(controlItem.ScopeMetadata["Type"].ToString()), controlOptions.Format),
                        Parameters = new ParameterList(controlItem.ScopeMetadata),
                        DynamicFormModelOptions = new DynamicFormModelOptions()
                        {
                            Label = controlItem.ShowLabel == false ? string.Empty : controlItem.Label,
                            Readonly = controlOptions.Readonly,
                            ShowSortButtons = controlOptions.ShowSortButtons,
                            ElementLabelProperty = controlOptions.ElementLabelProperty
                        },

                    };

                    if (controlItem.Rule != null)
                    {
                        dynamicFormModel.Rules = new List<DynamicFormModelRule>{ new DynamicFormModelRule(controlItem.Rule.Effect,
                        new DynamicFormModelRuleCondition(controlItem.Rule.Condition.Scope, controlItem.Rule.Condition.Schema)) };
                    }

                    result.Add(dynamicFormModel);
                    break;
                case LayoutType.Categorization:
                    var categorization = item as CategorizationDescriptionItem;

                    var categorizationDynamic = new DynamicFormModel
                    {
                        DynamicType = new ComponentSelectionKey(ComponentType.Categorization),
                        Elements = GenerateModelsInternal(categorization.Elements, new List<DynamicFormModel>()),
                        Rules = new List<DynamicFormModelRule> { new DynamicFormModelRule(categorization.Rule.Effect,
                        new DynamicFormModelRuleCondition(categorization.Rule.Condition.Scope, categorization.Rule.Condition.Schema)) }
                    };
                    if (categorization.Rule != null)
                    {
                        categorizationDynamic.Rules.Add(new DynamicFormModelRule(categorization.Rule.Effect,
                        new DynamicFormModelRuleCondition(categorization.Rule.Condition.Scope, categorization.Rule.Condition.Schema)));
                    }

                    result.Add(categorizationDynamic);
                    break;
                case LayoutType.Category:
                    var category = item as CategoryDescriptionItem;
                    var categoryDynamicFormModel = new DynamicFormModel
                    {
                        DynamicType = new ComponentSelectionKey(ComponentType.Category),
                        Elements = GenerateModelsInternal(category.Elements, new List<DynamicFormModel>()),
                        DynamicFormModelOptions = new DynamicFormModelOptions()
                        {
                            Label = category.Label
                        }
                    };
                    if (category.Rule != null)
                    {
                        categoryDynamicFormModel.Rules.Add(new DynamicFormModelRule(category.Rule.Effect,
                        new DynamicFormModelRuleCondition(category.Rule.Condition.Scope, category.Rule.Condition.Schema)));
                    }
                    result.Add(categoryDynamicFormModel);

                    break;
                case LayoutType.Group:
                    var group = item as GroupDescriptionItem;

                    var groupDynamicFormModel = new DynamicFormModel
                    {
                        DynamicType = new ComponentSelectionKey(ComponentType.GroupLayout),
                        Elements = GenerateModelsInternal(group.Elements, new List<DynamicFormModel>()),
                        DynamicFormModelOptions = new DynamicFormModelOptions()
                        {
                            Label = group.Label
                        },
                        Rules = new List<DynamicFormModelRule>{ new DynamicFormModelRule(group.Rule.Effect,
                        new DynamicFormModelRuleCondition(group.Rule.Condition.Scope, group.Rule.Condition.Schema)) }
                    };

                    if (group.Rule != null)
                    {
                        groupDynamicFormModel.Rules.Add(new DynamicFormModelRule(group.Rule.Effect,
                        new DynamicFormModelRuleCondition(group.Rule.Condition.Scope, group.Rule.Condition.Schema)));
                    }

                    result.Add(groupDynamicFormModel);

                    break;
                default:
                    return result;
            }
        }
        return result;
    }
}