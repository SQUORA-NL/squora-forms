namespace Squora.Forms.Core.Models;

public class DynamicFormModelOptions
{
    public string Label { get; set; } = default!;

    /// <summary>
    /// defines the way the items in the array are gonna be presented as Radio, 
    /// </summary>
    public bool RadioGroup { get; set; } = default!;

    public bool Readonly { get; set; }

    /// <summary>
    /// toggle additional buttons that allow chaging the order of the items in the array
    /// </summary>
    public bool ShowSortButtons { get; set; }

    /// <summary>
    /// when the itme is collapesed this will indicate with property is going to be used as the description for that element
    /// </summary>
    public string ElementLabelProperty { get; set; }
}