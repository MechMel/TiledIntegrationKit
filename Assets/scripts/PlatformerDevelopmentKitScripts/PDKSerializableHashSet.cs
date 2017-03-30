/*
Sources: https://github.com/TheOddler/unity-helpers/blob/master/SerializableDictionary.cs
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PDKSerializableHashSet<TValue> : HashSet<TValue>, ISerializationCallbackReceiver
{
    [SerializeField, HideInInspector]
    private List<TValue> values;



    // When this hashset is serialized
    public void OnBeforeSerialize()
    {
        // These are used to store the  values from this HashSet
        values = new List<TValue>(this.Count);
        // Go through each value in this hashset
        foreach (var value in this)
        {
            // Store this value pair in the list
            values.Add(value);
        }
    }


    // When this hashset is deserialized
    public void OnAfterDeserialize()
    {
        // Empty out anything that could be in this hashset
        this.Clear();
        // Go through each value in this hashset
        foreach (var value in values)
        {
            // Add this value, from the list, back into the hashset
            this.Add(value);
        }
    }
}
