using System.Collections;
using Newtonsoft.Json.Linq;
using Squora.Forms.Core.Models;

namespace Squora.Forms.Core.Helpers;

/// <summary>
/// Reads the value of a FileUpload field back into <see cref="UploadedFile"/>s. The value can be a typed
/// <see cref="UploadedFile"/> (or a list of them), or whatever JSON loading produced for it: an ExpandoObject, a
/// dictionary or a JObject. A single file and a list of files are both accepted, whatever the field's Multiple setting is.
/// </summary>
public static class UploadedFileConverter
{
    public static List<UploadedFile> ToFiles(object? value)
    {
        var files = new List<UploadedFile>();
        if (value is null or string) return files;

        if (value is IEnumerable items && value is not IDictionary<string, object> && value is not JObject)
        {
            foreach (var item in items)
            {
                var file = ToFile(item);
                if (file != null) files.Add(file);
            }
        }
        else
        {
            var single = ToFile(value);
            if (single != null) files.Add(single);
        }
        return files;
    }

    public static UploadedFile? ToFile(object? value)
    {
        switch (value)
        {
            case UploadedFile file:
                return file;
            case JObject jObject:
                return jObject.ToObject<UploadedFile>();
            case IDictionary<string, object> dictionary:
                return new UploadedFile
                {
                    Name = Text(dictionary, "name"),
                    ContentType = Text(dictionary, "contentType"),
                    Size = dictionary.TryGetValue("size", out var size) && size != null ? Convert.ToInt64(size) : 0,
                    Data = Text(dictionary, "data")
                };
            default:
                return null;
        }
    }

    private static string Text(IDictionary<string, object> dictionary, string key) =>
        dictionary.TryGetValue(key, out var value) ? value?.ToString() ?? string.Empty : string.Empty;
}
