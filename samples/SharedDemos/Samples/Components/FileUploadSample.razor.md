# FileUpload

Lets the user pick one or more files. The files are read in the browser session and stored in the form data as
Base64, so the result is plain JSON.

### Parameters

- **Label** (`string`): label above the picker.
- **Multiple** (`bool`, default `false`): allow several files. The value is then a list of files instead of one file.
- **Accept** (`string`): file types the picker offers, like `.pdf,image/*`.
- **MaximumFileSize** (`long`, default 5 MB): largest allowed size of one file, in bytes. Larger files are rejected.
- **MaximumFiles** (`int`, default 0 = no limit): largest allowed number of files when `Multiple` is set.
- **Required** (`bool`): at least one file must be chosen.

### Resulting JSON

A file is `{ "name", "contentType", "size", "data" }` where `data` is the Base64 encoded content.

````json
{
  "cv": { "name": "cv.pdf", "contentType": "application/pdf", "size": 5, "data": "aGVsbG8=" },
  "attachments": [ { "name": "a.txt", "contentType": "text/plain", "size": 2, "data": "aGk=" } ]
}
````

### JSON Schema

````json
{
  "type": "object",
  "properties": {
    "cv": {
      "type": "object", "format": "squora-file", "accept": ".pdf", "maxFileSize": 102400,
      "properties": {
        "name": { "type": "string" }, "contentType": { "type": "string" }, "size": { "type": "integer" },
        "data": { "type": "string", "contentEncoding": "base64" }
      },
      "required": ["name", "data"]
    },
    "attachments": {
      "type": "array", "maxItems": 3,
      "items": { "type": "object", "format": "squora-file", "properties": { "name": { "type": "string" }, "data": { "type": "string", "contentEncoding": "base64" } } }
    }
  }
}
````

An object with `"format": "squora-file"` becomes a single-file upload, an array of such objects a multiple-file upload.

## Sample code

````csharp
<BDForm FormModel="@model" Components="@UserSettingsService.Components" Value="@data" ValueChanged="@update"></BDForm>

@code {
    public ExpandoObject data = new ExpandoObject();

    public DynamicFormModel model = FormFactory.VerticalLayout(
        FormFactory.FileUploadComponent("CV (one file)", "$.cv", b => b.WithAccept(".pdf,.txt").WithMaximumFileSize(100 * 1024)),
        FormFactory.FileUploadComponent("Attachments (several files)", "$.attachments",
            b => b.WithMultiple().WithMaximumFiles(3).WithMaximumFileSize(100 * 1024))
    );
}
````

With a typed model use `UploadedFile` (one file) or `List<UploadedFile>` (several) as the property type.
