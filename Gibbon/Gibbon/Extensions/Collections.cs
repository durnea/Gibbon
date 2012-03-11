using System.Collections.Generic;
using System.Collections.Specialized;

namespace Gibbon.Extensions
{
    internal static class CollectionsExtensions
    {
        internal static Dictionary<string, string> ToDictionary(this NameValueCollection collection)
        {
            var output = new Dictionary<string, string>();
            int count = collection.Count;
            int i = 0;
            while (i < count)
            {
                string key = collection.GetKey(i);

                if (!output.ContainsKey(key))
                    output.Add(key, collection.Get(i));
                else
                    output[key] = collection.Get(i);
                i++;
            }

            return output;
        }

        internal static void AddOrReplace<TKey, TValue>(this Dictionary<TKey, TValue> collection, TKey key, TValue value)
        {
            if (collection.ContainsKey(key))
                collection[key] = value;
            else
                collection.Add(key, value);
        }
    }
}
