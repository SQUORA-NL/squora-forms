# UISchema Sample

This sample demonstrates rendering a form from a **UISchema** JSON definition and a **JSON Schema**. The `SquoraForm` component accepts both schemas and generates the form layout automatically — no `DynamicFormModel` is built in code.

- The **UISchema** defines the layout: a `HorizontalLayout` with two fields for `name` and `birthDate`
- The **Schema** defines the data types and constraints for each property

## Razor declarations

```html
<SquoraForm Components="@UserSettingsService.Components"
                    Data="@data"
                    Schema="@Schema"
                    UISchema="@UISchema"
                    ValueChanged="@Update">
</SquoraForm>
```

## Code

```csharp
private void Update()
{
    StateHasChanged();
}

JToken UISchema;
JObject Schema;
TestData data = new TestData() { name = "demo", birthDate = "none" };

protected override Task OnInitializedAsync()
{
    UISchema = JToken.Parse("{\"type\":\"HorizontalLayout\",\"elements\":[{\"type\":\"Control\",\"label\":\"Name\",\"scope\":\"#/properties/name\"},{\"type\":\"Control\",\"label\":\"Birth Date\",\"scope\":\"#/properties/birthDate\"}]}");
    Schema = JObject.Parse("{\"$schema\":\"http://json-schema.org/draft-04/schema#\",\"title\":\"Schema for a recording\",\"type\":\"object\",\"properties\":{\"name\":{\"type\":\"string\"},\"birthDate\":{\"type\":\"string\",\"format\":\"date-time\"}}}");
    return base.OnInitializedAsync();
}

public class TestData
{
    public string name { get; set; }
    public string birthDate { get; set; }
}
```

## Schema

```json
{
  "$schema": "http://json-schema.org/draft-04/schema#",
  "title": "Schema for a recording",
  "type": "object",
  "properties": {
    "name": {
      "type": "string"
    },
    "birthDate": {
      "type": "string",
      "format": "date-time"
    }
  }
}
```

## UISchema

```json
{
  "type": "HorizontalLayout",
  "elements": [
    {
      "type": "Control",
      "label": "Name",
      "scope": "#/properties/name"
    },
    {
      "type": "Control",
      "label": "Birth Date",
      "scope": "#/properties/birthDate"
    }
  ]
}
```
