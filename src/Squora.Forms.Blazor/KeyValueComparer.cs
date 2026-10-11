namespace Squora.Forms.Blazor
{
    public class KeyValueComparer : IEqualityComparer<KeyValuePair<string, object>>
    {
        public bool Equals(KeyValuePair<string, object> x, KeyValuePair<string, object> y)
        {
            return x.Key.Equals(y.Key);
        }

        public int GetHashCode(KeyValuePair<string, object> obj)
        {
            return obj.Key.GetHashCode();
        }
    }

}
