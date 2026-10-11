namespace Squora.Forms.Contracts;

public interface IJsonSchemaProvider
{
    public string ReadSchema(string fileName);
}