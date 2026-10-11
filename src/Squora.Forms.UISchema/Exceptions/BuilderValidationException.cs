using Squora.Forms.UISchema.Builders;

namespace Squora.Forms.UISchema.Exceptions
{
    internal class BuilderValidationException : ArgumentException
    {
        internal BuilderValidationException(string message) : base(message)
        {

        }

        internal static BuilderValidationException Create(ValidationResult validationResult)
        {
            return new BuilderValidationException(string.Join($"{Environment.NewLine} ", validationResult.Errors));
        }
    }
}
