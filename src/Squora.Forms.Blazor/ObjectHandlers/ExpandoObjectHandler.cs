using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Squora.Forms.Blazor.ObjectHandlers
{
    public class ExpandoObjectHandler : IValueHandler
    {
        public void AddValue(string path, object obj, object defaultValue)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));

            if (!(obj is IDictionary<string, object> expando))
                throw new ArgumentException("Object must be of type ExpandoObject", nameof(obj));

            var defaultExpando = ConvertToExpando(defaultValue);

            path = path.TrimStart('$').TrimStart('.');
            var segments = path.Split('.');

            for (int i = 0; i < segments.Length; i++)
            {
                var segment = segments[i];

                if (i == segments.Length - 1)
                {
                    if (expando.TryGetValue(segment, out var currentValue))
                    {
                        // Handle List<object>
                        if (currentValue is List<object> currentList)
                        {
                            currentList.Add(defaultExpando);
                        }
                        // Handle Array
                        else if (currentValue.GetType().IsArray)
                        {
                            var array = (Array)currentValue;
                            var elementType = array.GetType().GetElementType();
                            var newArray = Array.CreateInstance(elementType, array.Length + 1);
                            Array.Copy(array, newArray, array.Length);
                            newArray.SetValue(defaultExpando, array.Length);
                            expando[segment] = newArray; // Replace old array with new array
                        }
                        else
                        {
                            // Not a list or array
                            throw new InvalidOperationException($"Expected a list or array at path '{path}', but found a different type.");
                        }
                    }
                    else
                    {
                        // Segment not found, create new list
                        var newList = new List<object> { defaultExpando };
                        expando[segment] = newList;
                    }
                }

                else
                {
                    GetNextSegment(ref expando, segment);
                }
            }
        }

        private static object GetNextSegment(ref IDictionary<string, object> expando, string segment)
        {
            object obj;
            // Check if segment has an array index notation, e.g., "cars[0]"
            var isArrayIndex = segment.EndsWith("]") && segment.Contains("[");
            var arrayIndex = -1;
            string arrayName = null;

            if (isArrayIndex)
            {
                // Extract array name and index from segment
                arrayName = segment.Substring(0, segment.IndexOf("["));
                var indexStr = segment.Substring(segment.IndexOf("[") + 1, segment.Length - arrayName.Length - 2);
                if (!int.TryParse(indexStr, out arrayIndex))
                {
                    throw new ArgumentException("Invalid array index in path.", nameof(segment));
                }
            }

            if (isArrayIndex)
            {
                // Process segment with array index
                if (!expando.TryGetValue(arrayName, out obj) || !(obj is IList<object> currentList))
                {
                    currentList = new List<object>();
                    expando[arrayName] = currentList;
                }

                while (currentList.Count <= arrayIndex)
                {
                    currentList.Add(new ExpandoObject());
                }

                obj = currentList[arrayIndex];
                if (!(obj is IDictionary<string, object> nextExpando))
                {
                    nextExpando = new ExpandoObject();
                    currentList[arrayIndex] = nextExpando;
                }
                expando = nextExpando;
            }
            else
            {
                // Process regular segment
                if (!expando.TryGetValue(segment, out obj) || !(obj is IDictionary<string, object> nextExpando))
                {
                    nextExpando = new ExpandoObject();
                    expando[segment] = nextExpando;
                }
                expando = nextExpando;
            }

            return obj;
        }

        public ExpandoObject ConvertToExpando(object obj)
        {
            if (obj is ExpandoObject expando)
            {
                // The object is already an ExpandoObject
                return expando;
            }
            else
            {
                // Convert to ExpandoObject
                var expandoObject = new ExpandoObject();
                var dictionary = (IDictionary<string, object>)expandoObject;
                if (obj != null)
                {
                    foreach (PropertyInfo prop in obj.GetType().GetProperties())
                    {
                        if (prop.CanRead && prop.GetIndexParameters().Length == 0)
                        {
                            dictionary[prop.Name] = prop.GetValue(obj);
                        }
                    }
                }
                return expandoObject;
            }
        }

        public int GetCounterValue(string path, object obj)
        {
            var pathValue = GetValue(path, obj);
            if(pathValue == null) { return 0; }
            // Check if the value is a collection and return its count
            if (pathValue is List<object> currentList)
            {
                return currentList.Count();
            }
            // Handle Array
            else if (pathValue.GetType().IsArray)
            {
                return ((Array)pathValue).Length;
            }
            return 0;
        }

        public object GetValue(string path, object obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));

            if (!(obj is IDictionary<string, object> expando))
                throw new ArgumentException("Object must be of type ExpandoObject", nameof(obj));

            path = path.TrimStart('$');
            path = path.TrimStart('.');

            var segments = path.Split('.');

            foreach (var originalSegment in segments)
            {
                // Use a separate variable for the potentially modified segment
                var segment = originalSegment;

                // Check if the segment has an array indexer
                var arrayIndex = -1; // Default to -1 to indicate no array index
                var arrayPattern = new Regex(@"(\w+)\[(\d+)\]"); // Pattern to match 'name[index]'
                var match = arrayPattern.Match(segment);
                if (match.Success)
                {
                    segment = match.Groups[1].Value; // The name part of the segment
                    arrayIndex = int.Parse(match.Groups[2].Value); // The index part of the segment
                }

                if (!expando.TryGetValue(segment, out obj))
                {
                    obj = null;
                    break;
                }

                // If the segment has an array indexer, attempt to get the specified element
                if (arrayIndex >= 0)
                {
                    if (obj is IList<object> list && arrayIndex < list.Count)
                    {
                        obj = list[arrayIndex];
                    }
                    else
                    {
                        obj = null;
                        break;
                    }
                }

                // If the value is another ExpandoObject, update our expando reference
                if (obj is IDictionary<string, object> nextExpando)
                {
                    expando = nextExpando;
                }
            }

            return obj;
        }



        public void RemoveValue(string path, object obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));

            if (!(obj is IDictionary<string, object> expando))
                throw new ArgumentException("Object must be of type ExpandoObject", nameof(obj));

            path = path.TrimStart('$').TrimStart('.');
            var segments = path.Split('.');
            for (int i = 0; i < segments.Length; i++)
            {
                var segment = segments[i];
                var arrayPattern = new Regex(@"(\w+)\[(\d+)\]");
                var match = arrayPattern.Match(segment);

                if (i == segments.Length - 1)
                {
                    if (match.Success)
                    {
                        // Handle array/list removal
                        segment = match.Groups[1].Value;
                        int arrayIndex = int.Parse(match.Groups[2].Value);

                        if (expando.TryGetValue(segment, out var listObj) && listObj is IList<object> list)
                        {
                            if (arrayIndex >= 0 && arrayIndex < list.Count)
                            {
                                list.RemoveAt(arrayIndex);
                            }
                            else
                            {
                                throw new IndexOutOfRangeException($"Index {arrayIndex} is out of range for list at '{path}'.");
                            }
                        }
                        else
                        {
                            throw new KeyNotFoundException($"List key '{segment}' not found in ExpandoObject.");
                        }
                    }
                    else
                    {
                        // Remove the key for non-list/array segment
                        if (expando.ContainsKey(segment))
                        {
                            expando.Remove(segment);
                        }
                        else
                        {
                            throw new KeyNotFoundException($"Key '{segment}' not found in ExpandoObject.");
                        }
                    }
                }
                else
                {
                    GetNextSegment(ref expando, segment);
                }
            }
        }


        public void SetValue(string path, object obj, object value)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));

            if (!(obj is IDictionary<string, object> expando))
                throw new ArgumentException("Object must be of type ExpandoObject", nameof(obj));

            path = path.TrimStart('$');
            path = path.TrimStart('.');

            var segments = path.Split('.');
            for (int i = 0; i < segments.Length; i++)
            {
                var segment = segments[i];

                // Check for array indexers
                var arrayIndex = -1;
                var arrayPattern = new Regex(@"(\w+)\[(\d+)\]");
                var match = arrayPattern.Match(segment);
                if (match.Success)
                {
                    segment = match.Groups[1].Value;
                    arrayIndex = int.Parse(match.Groups[2].Value);
                }

                if (i == segments.Length - 1)
                {
                    // Handle setting value for an array element
                    if (arrayIndex >= 0 && expando[segment] is IList<object> list && arrayIndex < list.Count)
                    {
                        list[arrayIndex] = value;
                    }
                    else
                    {
                        expando[segment] = value;
                    }
                }
                else
                {
                    if (!expando.TryGetValue(segment, out obj))
                    {
                        var nextExpando = new ExpandoObject();
                        expando[segment] = nextExpando;
                        obj = nextExpando;
                    }

                    if (arrayIndex >= 0)
                    {
                        // Navigate into the array
                        if (obj is IList<object> list && arrayIndex < list.Count)
                        {
                            obj = list[arrayIndex];
                        }
                        else
                        {
                            // Handle case where the array or index doesn't exist
                            var nextExpando = new ExpandoObject();
                            if (obj is IList<object> existingList && arrayIndex == existingList.Count)
                            {
                                // Append new expando to the list
                                existingList.Add(nextExpando);
                            }
                            else
                            {
                                // Replace or create a new list with the expando
                                var newList = new List<object> { nextExpando };
                                expando[segment] = newList;
                            }
                            obj = nextExpando;
                        }
                    }

                    if (!(obj is IDictionary<string, object> nextExpandoDict))
                    {
                        nextExpandoDict = new ExpandoObject();
                        expando[segment] = nextExpandoDict;
                    }
                    expando = nextExpandoDict;
                }
            }
        }


    }
}
