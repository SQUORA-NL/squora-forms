namespace Squora.Forms.Blazor.ObjectHandlers
{
    public interface IValueHandler
    {
        void AddValue(string path, object obj, object defaultValue);
        int GetCounterValue(string path, object obj);
        object GetValue(string path, object obj);
        void RemoveValue(string path, object value);
        void SetValue(string path, object obj, object value);
    }
}
