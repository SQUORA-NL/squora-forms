using System.Dynamic;
using System.Reflection;

namespace Squora.Forms.Blazor
{
    public static class ExpandoObjectExtensions
    {
        public static ExpandoObject ToExpando(this object source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            var expando = new ExpandoObject();
            var expandoDict = expando as IDictionary<string, object>;

            foreach (PropertyInfo property in source.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (property.CanRead)
                {
                    var value = property.GetValue(source);

                    if (value != null &&
                        !property.PropertyType.IsPrimitive &&
                        property.PropertyType != typeof(string) &&
                        property.PropertyType != typeof(DateTime) && // This line and the lines below can be extended based on types you consider as "primitive"
                        property.PropertyType != typeof(decimal) &&
                        !IsGenericList(property.PropertyType) &&
                        !IsGenericDictionary(property.PropertyType) &&
                        !property.PropertyType.IsEnum)
                    {
                        // Recurse into the property if it's a custom class type
                        expandoDict[property.Name] = value.ToExpando();
                    }
                    else
                    {
                        expandoDict[property.Name] = value;
                    }
                }
            }

            return expando;
        }

        private static bool IsGenericList(Type type)
        {
            return type.IsGenericType &&
                   (type.GetGenericTypeDefinition() == typeof(List<>) ||
                    typeof(IList<>).IsAssignableFrom(type.GetGenericTypeDefinition()) ||
                    typeof(ICollection<>).IsAssignableFrom(type.GetGenericTypeDefinition()) ||
                    typeof(IEnumerable<>).IsAssignableFrom(type.GetGenericTypeDefinition()));
        }

        private static bool IsGenericDictionary(Type type)
        {
            return type.IsGenericType &&
                   (type.GetGenericTypeDefinition() == typeof(Dictionary<,>) ||
                    typeof(IDictionary<,>).IsAssignableFrom(type.GetGenericTypeDefinition()));
        }
    }
}
