using UnityEngine;
using System.Collections.Generic;

namespace BrightBit
{

[System.Serializable]
public class SerializableDictionary<K, V> : Dictionary<K, V>, ISerializationCallbackReceiver
{
	[SerializeField, HideInInspector] List<K> keys;
	[SerializeField, HideInInspector] List<V> values;

	void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
		keys = new List<K>(this.Count);
		values = new List<V>(this.Count);

		foreach (KeyValuePair<K, V> entry in this)
        {
			keys.Add(entry.Key);
			values.Add(entry.Value);
		}
	}
	
	void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
		this.Clear();

        if (keys.Count != values.Count) Debug.LogError("Something went wrong! The amount of keys doesn't match the amount of values!");

        int count = Mathf.Min(keys.Count, values.Count);

		for (int i = 0; i < count; ++i)
        {
			this.Add(keys[i], values[i]);
		}
	}
}

} // of namespace BrightBit
