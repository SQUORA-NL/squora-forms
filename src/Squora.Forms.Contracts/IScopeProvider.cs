namespace Squora.Forms.Contracts;

public interface IScopeProvider
{
    public Dictionary<string, object> GetDataFromScope(string scope);
}