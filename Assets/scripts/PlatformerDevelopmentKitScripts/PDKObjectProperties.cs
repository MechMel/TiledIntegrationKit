using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PDKObjectProperties : MonoBehaviour
{
    [System.Serializable]
    public class PDKSerializableObjectProperties : PDKSerializableDictionay<string, string> { }

    // Holds this object's properties
    public PDKSerializableObjectProperties objectProperties;
}
