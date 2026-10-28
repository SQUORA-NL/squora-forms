using Squora.Forms.Core.Helpers;
using Squora.Forms.Blazor.Services;
using Microsoft.AspNetCore.Components;

namespace Squora.Forms.Blazor
{
    public partial class BDForm : Components.LayoutFormComponent
    {
        /// <summary>Extra classes on the stage, the area the form is on. An editor adds the look of its canvas with it.</summary>
        [Parameter] public string? StageClass { get; set; }

        /// <summary>Extra classes on the sheet, the form's own page.</summary>
        [Parameter] public string? SheetClass { get; set; }

        private ValidationStateService validationStateService = new ValidationStateService();

        public override string ValidationString => TokenReplacer.ReplaceTokens(InvalidMessage, this);

        private static string Join(string first, string? second) => string.IsNullOrWhiteSpace(second) ? first : $"{first} {second}";
    }
}
