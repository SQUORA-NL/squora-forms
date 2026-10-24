namespace Squora.Forms.JsonSchema.Constants;

public static class JsonSchemaConstants
{
    //Arrays
    public const string MaximumItems = "maxItems";
    public const string MinimumItems = "minItems";
    public const string UniqueItems = "uniqueItems";

    // Objects
    public const string Required = "required";
    public const string MinimumProperties = "minProperties";
    public const string MaximumProperties = "maxProperties";

    // For string
    public const string MaximumLength = "maxLength";
    public const string MinimumLength = "minLength";
    public const string Pattern = "pattern";
    public const string Format = "format";

    // For file uploads: an object (or array of objects) with this format holds { name, contentType, size, data (Base64) }
    public const string FileFormat = "squora-file";
    public const string Accept = "accept";
    public const string MaximumFileSize = "maxFileSize";

    // For integer and number
    public const string Minimum = "minimum";
    public const string Maximum = "maximum";
    public const string ExclusiveMaximum = "exclusiveMaximum";
    public const string ExclusiveMinimum = "exclusiveMinimum";
    public const string MultipleOf = "multipleOf";
}