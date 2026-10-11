namespace Squora.Forms.UISchema.Models
{
    public class DetailOption
    {
        public string DetailType { get; }
        public IEnumerable<ILayoutDescriptionItem> LayoutDescriptions { get; }

        public DetailOption(string type, IEnumerable<ILayoutDescriptionItem> layoutDescriptions = default)
        {
            DetailType = type;
            LayoutDescriptions = layoutDescriptions ?? new List<ILayoutDescriptionItem>();
        }
    }
}