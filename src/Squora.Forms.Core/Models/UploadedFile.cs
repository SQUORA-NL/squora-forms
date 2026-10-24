using Newtonsoft.Json;

namespace Squora.Forms.Core.Models;

/// <summary>
/// A file chosen in a FileUpload component. This is the shape written to the form's JSON:
/// <c>{ "name": "a.pdf", "contentType": "application/pdf", "size": 1234, "data": "&lt;base64&gt;" }</c>.
/// </summary>
public class UploadedFile
{
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("contentType")]
    public string ContentType { get; set; } = string.Empty;

    [JsonProperty("size")]
    public long Size { get; set; }

    /// <summary>The file content, Base64 encoded.</summary>
    [JsonProperty("data")]
    public string Data { get; set; } = string.Empty;

    public byte[] GetBytes() => Convert.FromBase64String(Data);

    public static UploadedFile FromBytes(string name, string contentType, byte[] content) => new()
    {
        Name = name,
        ContentType = contentType,
        Size = content.LongLength,
        Data = Convert.ToBase64String(content)
    };
}
