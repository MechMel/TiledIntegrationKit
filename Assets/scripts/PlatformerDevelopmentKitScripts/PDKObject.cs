using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

[Serializable]
public class PDKObject
{
    // Object Attributes
    public string name;
    public string type;
    public int id;
    public int gid;
    public int x;
    public int y;
    public int height;
    public int width;
    public int rotation;
    public bool visible;
    // Stores the prefab for this object
    //public Object prefab;
}
