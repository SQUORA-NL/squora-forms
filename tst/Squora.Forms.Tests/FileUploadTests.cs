using System.Dynamic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Squora.Forms.Blazor.Components.Input;
using Squora.Forms.Common.Enums;
using Squora.Forms.Core.Helpers;
using Squora.Forms.Core.Models;
using Squora.Forms.JsonSchema.Implementations;
using Squora.Forms.JsonSchema.Models;

namespace Squora.Forms.Tests;

public class FileUploadTests
{
    private const string Schema = """
    {
      "type": "object",
      "properties": {
        "cv": {
          "type": "object", "format": "squora-file", "accept": ".pdf", "maxFileSize": 1024,
          "properties": { "name": { "type": "string" }, "contentType": { "type": "string" }, "size": { "type": "integer" },
                          "data": { "type": "string", "contentEncoding": "base64" } },
          "required": ["name", "data"]
        },
        "attachments": {
          "type": "array", "maxItems": 3,
          "items": { "type": "object", "format": "squora-file",
                     "properties": { "name": { "type": "string" }, "data": { "type": "string", "contentEncoding": "base64" } } }
        },
        "tags": { "type": "array", "items": { "type": "string" } }
      }
    }
    """;

    private sealed class TestUpload : FileUploadComponentBase
    {
        // Blazor calls this when the parameters arrive; there is no renderer here.
        public void Refresh() => OnParametersSet();
    }

    private static FileSource Source(string name, string text) =>
        new(name, "text/plain", Encoding.UTF8.GetByteCount(text), _ => new MemoryStream(Encoding.UTF8.GetBytes(text)));

    [Test]
    public void An_object_with_the_file_format_is_a_single_file_upload()
    {
        var item = new SchemaReader().GetTypeFromPath(JObject.Parse(Schema), "#/properties/cv") as FileSchemaItem;

        Assert.That(item, Is.Not.Null);
        Assert.That(item!.Type, Is.EqualTo(ComponentType.FileUpload));
        Assert.That(item.Multiple, Is.False);
        Assert.That(item.Accept, Is.EqualTo(".pdf"));
        Assert.That(item.MaximumFileSize, Is.EqualTo(1024));
        Assert.That(item.ItemMetadata["Multiple"], Is.EqualTo(false));
    }

    [Test]
    public void An_array_of_file_objects_is_a_multiple_file_upload_and_other_arrays_are_unchanged()
    {
        var reader = new SchemaReader();
        var schema = JObject.Parse(Schema);

        var files = reader.GetTypeFromPath(schema, "#/properties/attachments") as FileSchemaItem;
        var tags = reader.GetTypeFromPath(schema, "#/properties/tags");

        Assert.That(files, Is.Not.Null);
        Assert.That(files!.Multiple, Is.True);
        Assert.That(files.MaximumFiles, Is.EqualTo(3));
        Assert.That(tags.Type, Is.EqualTo(ComponentType.Array));
    }

    [Test]
    public void Files_are_written_to_json_as_base64_with_camel_case_names()
    {
        var file = UploadedFile.FromBytes("a.txt", "text/plain", Encoding.UTF8.GetBytes("hello"));

        var json = JObject.Parse(JsonConvert.SerializeObject(new { cv = file }));

        Assert.That((string?)json["cv"]!["name"], Is.EqualTo("a.txt"));
        Assert.That((long?)json["cv"]!["size"], Is.EqualTo(5));
        Assert.That((string?)json["cv"]!["data"], Is.EqualTo("aGVsbG8="));
        Assert.That(file.GetBytes(), Is.EqualTo(Encoding.UTF8.GetBytes("hello")));
    }

    [Test]
    public void Files_are_read_back_from_loaded_json_for_one_file_and_for_a_list()
    {
        var one = ParameterListConverter_Expando("""{ "cv": { "name": "a.txt", "contentType": "text/plain", "size": 5, "data": "aGVsbG8=" } }""");
        var many = ParameterListConverter_Expando("""{ "docs": [ { "name": "a.txt", "data": "aGVsbG8=" }, { "name": "b.txt", "data": "aGk=" } ] }""");

        var single = UploadedFileConverter.ToFiles(((IDictionary<string, object>)one)["cv"]);
        var list = UploadedFileConverter.ToFiles(((IDictionary<string, object>)many)["docs"]);

        Assert.That(single.Select(f => f.Name), Is.EqualTo(new[] { "a.txt" }));
        Assert.That(single[0].Size, Is.EqualTo(5));
        Assert.That(list.Select(f => f.Name), Is.EqualTo(new[] { "a.txt", "b.txt" }));
        Assert.That(UploadedFileConverter.ToFiles(null), Is.Empty);
    }

    [Test]
    public async Task Picking_files_in_a_multiple_upload_adds_base64_files_to_the_data()
    {
        var data = new ExpandoObject();
        var upload = new TestUpload { Path = "$.docs", Value = data, Multiple = true };

        await upload.AddFilesAsync(new[] { Source("a.txt", "hello"), Source("b.txt", "hi") });
        await upload.AddFilesAsync(new[] { Source("c.txt", "yo") });

        var stored = (List<UploadedFile>)((IDictionary<string, object>)data)["docs"];
        Assert.That(stored.Select(f => f.Name), Is.EqualTo(new[] { "a.txt", "b.txt", "c.txt" }));
        Assert.That(stored[0].Data, Is.EqualTo("aGVsbG8="));

        upload.RemoveFile(1);
        Assert.That(((List<UploadedFile>)((IDictionary<string, object>)data)["docs"]).Select(f => f.Name), Is.EqualTo(new[] { "a.txt", "c.txt" }));
    }

    [Test]
    public async Task A_single_upload_keeps_only_the_last_file_and_clears_to_null()
    {
        var data = new ExpandoObject();
        var upload = new TestUpload { Path = "$.cv", Value = data };

        await upload.AddFilesAsync(new[] { Source("a.txt", "hello") });
        await upload.AddFilesAsync(new[] { Source("b.txt", "hi") });

        Assert.That(((UploadedFile)((IDictionary<string, object>)data)["cv"]).Name, Is.EqualTo("b.txt"));

        upload.Clear();
        Assert.That(((IDictionary<string, object>)data)["cv"], Is.Null);
    }

    [Test]
    public async Task Files_over_the_limits_are_turned_away_with_a_notice_and_do_not_block_the_form()
    {
        var data = new ExpandoObject();
        var upload = new TestUpload { Path = "$.docs", Value = data, Multiple = true, MaximumFileSize = 4, MaximumFiles = 1 };

        await upload.AddFilesAsync(new[] { Source("big.txt", "hello") });

        Assert.That(upload.Files, Is.Empty);
        Assert.That(upload.Notice, Does.Contain("big.txt"));
        Assert.That(upload.IsValid, Is.True);

        await upload.AddFilesAsync(new[] { Source("a.txt", "hi"), Source("b.txt", "yo") });

        Assert.That(upload.Files.Select(f => f.Name), Is.EqualTo(new[] { "a.txt" }));
        Assert.That(upload.Notice, Does.Contain("At most 1"));
        upload.Validate();
        Assert.That(upload.IsValid, Is.True);

        upload.RemoveFile(0);
        Assert.That(upload.Notice, Is.Null);
    }

    [Test]
    public void Data_that_is_already_over_the_limits_is_invalid_until_it_is_fixed()
    {
        var data = new ExpandoObject();
        var big = new List<UploadedFile> { UploadedFile.FromBytes("a", "t", new byte[2]), UploadedFile.FromBytes("b", "t", new byte[2]) };
        ((IDictionary<string, object>)data)["docs"] = big;
        var upload = new TestUpload { Path = "$.docs", Value = data, Multiple = true, MaximumFiles = 1 };
        upload.Refresh();

        upload.Validate();
        Assert.That(upload.IsValid, Is.False);
        Assert.That(upload.ValidationString, Does.Contain("At most 1"));

        big.RemoveAt(1);
        upload.Refresh();
        upload.Validate();
        Assert.That(upload.IsValid, Is.True);
    }

    [Test]
    public void A_required_upload_without_a_file_is_invalid()
    {
        var upload = new TestUpload { Required = true };

        upload.Validate();

        Assert.That(upload.IsValid, Is.False);
    }

    private static ExpandoObject ParameterListConverter_Expando(string json) =>
        Squora.Forms.Core.Parser.ParameterListConverter.ConvertJTokenToExpando(JObject.Parse(json));
}
