using System;
using System.Linq;
using System.Collections.Generic;

namespace BrightBit
{

public static class Extensions
{
    public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
    {
        if (enumerable == null) return true;

        ICollection<T> collection = enumerable as ICollection<T>;

        if (collection != null) return collection.Count < 1;

        Array array = enumerable as Array;

        if (array != null) return array.Length < 1;

        return !enumerable.Any();
    }

    public static void Swap<T>(this IList<T> list, int a, int b)
    {
        T temp  = list[a];
        list[a] = list[b];
        list[b] = temp;
    }

    public static Value GetValueOrDefault<Key, Value>(this Dictionary<Key, Value> dict, Key key, Value defaultValue = default(Value))
    {
        if (dict == null) throw new ArgumentNullException("dict");
        if (key == null)  throw new ArgumentNullException("key");

        Value result;

        return dict.TryGetValue(key, out result) ? result : defaultValue;
    }
}

} // of namespace BrightBit
