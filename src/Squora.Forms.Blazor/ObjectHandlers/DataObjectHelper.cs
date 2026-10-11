using System.Dynamic;

namespace Squora.Forms.Blazor.ObjectHandlers
{
    public class DataObjectHelper
    {
        private static IValueHandler GetHandler(object obj)
        {
            if (obj is ExpandoObject)
                return new ExpandoObjectHandler();
            else
                return new RegularObjectHandler();
        }

        public static object GetValue(string path, object obj)
        {
            if (string.IsNullOrEmpty(path)) return obj;
            var handler = GetHandler(obj);
            return handler.GetValue(path, obj);
        }

        public static void SetValue(string path, object obj, object value)
        {
            if (string.IsNullOrEmpty(path)) return;
            var handler = GetHandler(obj);
            handler.SetValue(path, obj, value);
        }

        public static void Remove(string path, object obj)
        {
            var handler = GetHandler(obj);
            handler.RemoveValue(path, obj);
        }

        public static void Add(string path, object obj, object defaultValue)
        {
            var handler = GetHandler(obj);
            handler.AddValue(path, obj, defaultValue);
        }

        public static int GetCounterValue(string path, object obj)
        {
            var handler = GetHandler(obj);
            return handler.GetCounterValue(path, obj);
        }
    }
}
