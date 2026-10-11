using Squora.Forms.Core.Helpers;
using Squora.Forms.Blazor.Components;

namespace Squora.Forms.Blazor
{
    public partial class NotFoundComponent : DisplayFormComponent
    {
        public override string ValidationString => TokenReplacer.ReplaceTokens(InvalidMessage, this);

    }
}
