using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

[Serializable]
public class PDKObject
{
    public string name;
    public string type;
    public int id;
    public int gid;
    public int x;
    public int y;
    public int width;
    public int height;
    public int rotation;
    public bool visible;
    public PDKMap.PDKCustomProperties properties;
    public UnityEngine.Object prefab;
}
