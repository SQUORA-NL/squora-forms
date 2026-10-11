using Squora.Forms.Common.Enums;
using Squora.Forms.Core.Models.ParameterModels;
using Newtonsoft.Json;

namespace Squora.Forms.Core.Models;

public class DynamicFormModel
{
    private DynamicFormModelOptions? _dynamicFormModelOptions;

    public DynamicFormModel()
    {

    }

    public DynamicFormModel(ComponentType componentType)
    {
        DynamicType = new ComponentSelectionKey(componentType);
    }
    [JsonConstructor]
    public DynamicFormModel(ComponentType componentType, string typeDefinionName)
    {
        DynamicType = new ComponentSelectionKey(componentType, typeDefinionName);
    }
    public DynamicFormModel(ComponentSelectionKey componentSelectionKey)
    {
        DynamicType = componentSelectionKey;
    }

    [JsonProperty("DynamicType")]
    public ComponentSelectionKey DynamicType { get; set; } = new ComponentSelectionKey(ComponentType.Default);

    public ParameterList Parameters { get; set; } = new();

    public List<DynamicFormModel> Elements { get; set; } = new();

    public DynamicFormModelOptions DynamicFormModelOptions
    {
        get => _dynamicFormModelOptions;
        set
        {
            _dynamicFormModelOptions = value;
        }
    }

    public List<DynamicFormModelRule>? Rules { get; set; }
}