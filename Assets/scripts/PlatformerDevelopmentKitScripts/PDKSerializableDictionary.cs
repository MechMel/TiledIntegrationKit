/*
Sources: https://github.com/TheOddler/unity-helpers/blob/master/SerializableDictionary.cs
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PDKSerializableDictionay<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField, HideInInspector]
    private List<TKey> keys;
    [SerializeField, HideInInspector]
    private List<TValue> values;



    // When this dictionary is serialized
    public void OnBeforeSerialize()
    {
        // These are used to store the keys and values from this dictionary
        keys = new List<TKey>(this.Count);
        values = new List<TValue>(this.Count);
        // Go through each key value pair in this dictionary
        foreach (var keyValuePair in this)
        {
            // Store this key value pair in the lists
            keys.Add(keyValuePair.Key);
            values.Add(keyValuePair.Value);
        }
    }


    // When this dictionary is deserialized
    public void OnAfterDeserialize()
    {
        // Empty out anything that could be in this dictionary
        this.Clear();
        // Go thorugh each key value pair in this dictionary
        for (int indexOfCurrentKVP = 0; indexOfCurrentKVP < keys.Count; indexOfCurrentKVP++)
        {
            // Add this key value pair, from the lists, back into the dictionary
            this.Add(keys[indexOfCurrentKVP], values[indexOfCurrentKVP]);
        }
    }
}
