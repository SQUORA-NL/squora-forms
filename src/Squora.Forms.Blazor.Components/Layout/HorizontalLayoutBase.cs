using Squora.Forms.Core.Helpers;

namespace Squora.Forms.Blazor.Components.Layout
{
    public class HorizontalLayoutBase : LayoutFormComponent
    {
        public override string ValidationString => TokenReplacer.ReplaceTokens(InvalidMessage, this);
    }
}
