using Squora.Forms.Core.Helpers;

namespace Squora.Forms.Blazor.Components.Layout
{
    public class VerticalLayoutBase : InputFormComponent
    {
        public override string ValidationString => TokenReplacer.ReplaceTokens(InvalidMessage, this);
    }
}
