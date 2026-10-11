namespace Squora.Forms.UISchema.Builders
{
    internal class ValidationResult
    {
        internal bool Success { get; private set; }
        internal List<string> Errors { get; private set; }
      
        private ValidationResult(bool success, List<string> errors)
        {
            Success = success;
            Errors = errors;
        }

        internal static ValidationResult ValidResult()
        {
            return new ValidationResult(true, new List<string>());
        }

        internal static ValidationResult InvalidResult(string message)
        {
            return new ValidationResult(false, new List<string> { message});
        }

        internal ValidationResult InvalidateResult(string message)
        {
            Success = false;
            Errors.Add(message);
            return this;
        }
    }
}
