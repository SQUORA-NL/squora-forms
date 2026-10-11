## razor component

```csharp
    <BDForm FormModel="@model" Components="@UserSettingsService.Components" Value="@data" ValueChanged="@update"></BDForm>
```

code behind
##
```csharp
   model = new DynamicFormModelParser().Deserialize(modelJson);
```