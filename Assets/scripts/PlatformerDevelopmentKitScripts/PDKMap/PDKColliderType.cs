using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PDKColliderType
{
    [System.Serializable]
    public class PDKSerializableHashSetOfIDs : PDKSerializableHashSet<int> { }
    
    public string name;
    public PDKSerializableHashSetOfIDs tilesWithThisCollider;
    public UnityEngine.Object gameObjectForThisCollider;

    public PDKColliderType()
    {
        tilesWithThisCollider = new PDKSerializableHashSetOfIDs();
    }
}
