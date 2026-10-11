using Microsoft.AspNetCore.Components;
using System.Collections;
using System.Reflection;

namespace Squora.Forms.Blazor.ObjectHandlers
{
    public class RegularObjectHandler : IValueHandler
    {
        public object GetValue(string path, object obj)
        {
            ValidateInput(path, obj);

            var segments = NormalizePath(path).Split('.');

            foreach (var segment in segments)
            {
                if (IsIndexedProperty(segment))
                {
                    obj = GetIndexedPropertyValue(obj, segment);
                }
                else
                {
                    obj = GetPropertyValue(obj, segment);
                }

                if (obj == null) return null;
            }

            return obj;
        }

        private static void ValidateInput(string path, object obj)
        {
            if (obj == null) { return;  }
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
        }

        private static string NormalizePath(string path)
        {
            return path.TrimStart('$', '.');
        }

        private static bool IsIndexedProperty(string segment)
        {
            return segment.EndsWith("]") && segment.Contains("[");
        }

        private static object GetIndexedPropertyValue(object obj, string segment)
        {
            var (propertyName, index) = ExtractPropertyAndIndex(segment);

            var listObj = GetPropertyValue(obj, propertyName) as IList;

            ValidateListAndIndex(listObj, propertyName, index, segment);
            if (index > listObj.Count - 1 || index < 0)
            {
                return null; //TODO, THE RENDERING IS FAULTY IT WILL RUN MULTIPLE TIMES ALSO WITH OLDER DATA
            }
            return listObj[index];
        }

        private static (string PropertyName, int Index) ExtractPropertyAndIndex(string segment)
        {
            var indexStartPos = segment.IndexOf('[');
            var propertyName = segment.Substring(0, indexStartPos);
            var indexPart = segment.Substring(indexStartPos + 1, segment.Length - indexStartPos - 2);

            if (!int.TryParse(indexPart, out int index))
            {
                throw new Exception($"Invalid index format in segment {segment}");
            }

            return (propertyName, index);
        }

        private static void ValidateListAndIndex(IList list, string propertyName, int index, string segment)
        {
            if (list == null || index >= list.Count)
            {
                //TODO, throw new Exception($"Property {propertyName} is not a list or index is out of range in segment {segment}.");
            }
        }

        private static object GetPropertyValue(object obj, string propertyName)
        {
            var propInfo = obj.GetType().GetProperty(propertyName);

            if (propInfo == null)
            {
                //todo use logger : throw new Exception($"Property {propertyName} not found on {obj.GetType().Name}");
                return null;
            }

            return propInfo.GetValue(obj);
        }

        public void SetValue(string path, object obj, object value)
        {
            ValidateInput(path, obj);

            var segments = NormalizePath(path).Split('.');

            for (int i = 0; i < segments.Length; i++)
            {
                var segment = segments[i];

                if (IsLastSegment(segments, i))
                {
                    if (IsIndexedProperty(segment))
                    {
                        SetIndexedPropertyValue(obj, segment, value);
                    }
                    else
                    {
                        SetPropertyValue(obj, segment, value);
                    }
                }
                else
                {
                    if (IsIndexedProperty(segment))
                    {
                        obj = GetIndexedPropertyValue(obj, segment);
                    }
                    else
                    {
                        obj = GetPropertyValue(obj, segment);
                    }
                }
            }
        }

        private static bool IsLastSegment(string[] segments, int currentIndex)
        {
            return currentIndex == segments.Length - 1;
        }

        private static void SetPropertyValue(object obj, string propertyName, object value)
        {
            var propInfo = GetPropertyInfo(obj, propertyName);
            if (propInfo != null)
            {
                propInfo.SetValue(obj, value);
            }
        }

        private static void SetIndexedPropertyValue(object obj, string segment, object value)
        {
            var (propertyName, index) = ExtractPropertyAndIndex(segment);
            var listObj = GetPropertyValue(obj, propertyName) as IList;

            ValidateListAndIndex(listObj, propertyName, index, segment);

            listObj[index] = value;
        }

        private static PropertyInfo GetPropertyInfo(object obj, string propertyName)
        {
            if (obj == null) { return null; }
            var propInfo = obj.GetType().GetProperty(propertyName);
            if (propInfo == null)
            {
                return null;
                // todo use logger throw new Exception($"Property {propertyName} not found on {obj.GetType().Name}");
            }
            return propInfo;
        }

        /// <summary>
        /// Parameter values come from JSON, where <c>1</c> is read as a 64-bit integer and <c>1.5</c> as a double. A
        /// component property typed <c>double</c> (Minimum, Maximum, Step) or <c>int</c> cannot be set from the other, and
        /// Blazor throws while setting it, which ends the whole circuit. Numbers are converted to the property's own type;
        /// anything else, or a value that does not fit, is passed through unchanged.
        /// </summary>
        internal static object CoerceToProperty(Type componentType, string propertyName, object value)
        {
            if (value == null) { return value; }
            var property = componentType.GetProperty(propertyName);
            if (property == null) { return value; }

            var target = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            if (target.IsInstanceOfType(value) || !IsNumeric(target) || !IsNumeric(value.GetType())) { return value; }

            try
            {
                return Convert.ChangeType(value, target, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception ex) when (ex is OverflowException or InvalidCastException or FormatException)
            {
                return value;
            }
        }

        private static bool IsNumeric(Type type) =>
            Type.GetTypeCode(type) is TypeCode.SByte or TypeCode.Byte or TypeCode.Int16 or TypeCode.UInt16 or TypeCode.Int32
                or TypeCode.UInt32 or TypeCode.Int64 or TypeCode.UInt64 or TypeCode.Single or TypeCode.Double or TypeCode.Decimal;

        public static List<string> GetPropertiesWithParameterAnnotation(Type componentType)
        {
            return componentType.GetProperties()
                .Where(p => p.GetCustomAttribute<ParameterAttribute>() != null)
                .Select(p => p.Name)
                .ToList();
        }

        public static Dictionary<string, object> GetParameters(Type componentType, Dictionary<string, object> parameters, List<KeyValuePair<string, object>> completeListOfParameters)
        {
            var validParameters = new Dictionary<string, object>();
            IEnumerable<string> componentProperties = GetPropertiesWithParameterAnnotation(componentType);

            completeListOfParameters.ForEach(p =>
            {
                if (componentProperties.Contains(p.Key))
                {
                    validParameters.Add(p.Key, CoerceToProperty(componentType, p.Key, p.Value));
                }
            }
            );


            return validParameters;
        }

        public void RemoveValue(string path, object obj)
        {
            ValidateInput(path, obj);

            var segments = NormalizePath(path).Split('.');

            for (int i = 0; i < segments.Length; i++)
            {
                var segment = segments[i];

                if (IsLastSegment(segments, i))
                {
                    if (IsIndexedProperty(segment))
                    {
                        RemoveIndexedValue(obj, segment);
                    }
                    else
                    {
                        // Handle non-indexed properties if needed
                        throw new Exception($"Cannot remove value for non-indexed property {segment}");
                    }
                }
                else
                {
                    obj = IsIndexedProperty(segment) ? GetIndexedPropertyValue(obj, segment) : GetPropertyValue(obj, segment);
                    if (obj == null) throw new Exception($"Property {segment} was null and could not traverse to {segments[i + 1]}");
                }
            }
        }

        private static void RemoveIndexedValue(object obj, string segment)
        {
            var (propertyName, index) = ExtractPropertyAndIndex(segment);
            var listObj = GetPropertyValue(obj, propertyName) as IList;

            ValidateListAndIndex(listObj, propertyName, index, segment);

            listObj.RemoveAt(index);
        }

        public void AddValue(string path, object obj, object defaultValue)
        {
            ValidateInput(path, obj);

            var segments = NormalizePath(path).Split('.');

            for (int i = 0; i < segments.Length; i++)
            {
                var segment = segments[i];

                if (IsLastSegment(segments, i))
                {
                    // Get the property or indexed value
                    var targetObj = IsIndexedProperty(segment) ? GetIndexedPropertyValue(obj, segment) : GetPropertyValue(obj, segment);

                    // Add to list or array
                    if (targetObj is IList list)
                    {
                        list.Add(defaultValue);
                    }
                    else if (targetObj.GetType().IsArray)
                    {
                        var array = (Array)targetObj;
                        var elementType = array.GetType().GetElementType();
                        var newArray = Array.CreateInstance(elementType, array.Length + 1);
                        Array.Copy(array, newArray, array.Length);
                        newArray.SetValue(defaultValue, array.Length);
                        SetPropertyValue(obj, segment, newArray); // Replace the old array with the new array
                    }
                    else
                    {
                        throw new InvalidOperationException($"Expected a list or array at path '{path}', but found a different type.");
                    }
                }
                else
                {
                    // Navigate to the next segment
                    obj = IsIndexedProperty(segment) ? GetIndexedPropertyValue(obj, segment) : GetPropertyValue(obj, segment);
                    if (obj == null)
                    {
                        throw new NullReferenceException($"Property {segment} was null and could not traverse to {segments[i + 1]}");
                    }
                }
            }
        }

        public int GetCounterValue(string path, object valueObject)
        {
            var obj = GetValue(path, valueObject);  
            
                // Check if obj is null
                if (obj == null)
                {
                    return 0;
                }

                // Check if the object is a collection
                if (obj is IEnumerable && !(obj is string))
                {
                    int count = 0;

                    // Count the number of items in the collection
                    foreach (var item in (IEnumerable)obj)
                    {
                        count++;
                    }

                    return count;
                }

                // If obj is not a collection, return 0 or some other default value
                return 0;
            
        }
    }
}
