using Squora.Forms.Common.Enums;

namespace Squora.Forms.JsonSchema.Models;

/// <summary>
/// A file upload field. In the schema it is an object with <c>"format": "squora-file"</c> (one file) or an array of such
/// objects (several files). The property names here match the FileUpload component's parameters.
/// </summary>
public class FileSchemaItem : SchemaItem
{
    public bool Multiple { get; set; }
    /// <summary>0 means no limit.</summary>
    public int MaximumFiles { get; set; }
    public int? MinimumFiles { get; set; }
    public string? Accept { get; set; }
    public long MaximumFileSize { get; set; } = 5 * 1024 * 1024;

    public FileSchemaItem(string path) : base(ComponentType.FileUpload, path)
    {
    }
}
