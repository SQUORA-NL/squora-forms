namespace Squora.Forms.UISchema.Models
{
    public class ControlOptions
    {
        public DetailOption Detail { get; private set; }

        public bool ShowSortButtons { get; private set; } = false;

        public string ElementLabelProperty { get; private set; } = string.Empty;

        public string Format { get; private set; } = string.Empty;

        public bool Readonly { get; private set; }

        public static ControlOptions Default()
        {
            return new ControlOptions();
        }

        internal ControlOptions WithDetail(DetailOption detail)
        {
            Detail = detail;
            return this;
        }

        internal ControlOptions WithReadonly(bool isReadonly)
        {
            Readonly = isReadonly;
            return this;
        }

        internal ControlOptions WithShowSortButtons(bool showSortButtons)
        {
            ShowSortButtons = showSortButtons;
            return this;
        }
        internal ControlOptions WithFormat(string format)
        {
            Format = format;
            return this;
        }
        internal ControlOptions WithElementLabelProperty(string elementLabelProperty)
        {
            ElementLabelProperty = elementLabelProperty;
            return this;
        }
    }
}