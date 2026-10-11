using System.Collections;

namespace Squora.Forms.Core.Models.ParameterModels
{
    public class ParameterList : IEnumerable<KeyValuePair<string, object>>
    {
        Dictionary<string, object> parameters = new Dictionary<string, object>();
        public ParameterList(Dictionary<string, object> parameters)
        {
            foreach (var item in parameters)
            {
                Add(item.Key, item.Value);
            }
        }

        public Dictionary<string, object> Entries
        {
            get { return parameters; }
        }

        public ParameterList() { }

        public ParameterList Add(string key, object value)
        {
            if (parameters.ContainsKey(key))
            {
                parameters[key] = value;
            }
            else
            {
                parameters.Add(key, value);
            }
            return this;
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return parameters.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void AddRange(ParameterList customParameters)
        {
            foreach (var item in customParameters)
            {
                parameters.Add(item.Key, item.Value);
            }
        }
    }
}
