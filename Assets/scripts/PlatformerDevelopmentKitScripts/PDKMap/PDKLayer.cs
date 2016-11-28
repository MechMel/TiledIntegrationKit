﻿using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class PDKLayer
{
    public enum layerTypes { Tile, Object, Image };
    public string name;
    public int height;
    public int width;
    public bool visible;
    public int opacity;
    public int horizontalOffset;
    public int verticalOffset;
    public layerTypes type;
    // Stores the custom properties for this map
    public Dictionary<string, string> properties;
    // Tile Layer Attributes
    public int[] tileMap;
    // Object Layer Attributes
    public PDKObject[] objects;
    // Image Layer Attributes
    public Texture2D image;



    //The Initialize function sets up the tile layer, objects, and images
    public void InitializeLayer(Dictionary<string, UnityEngine.Object> objectsInMap)
    {
        if (type == layerTypes.Object)
        {
            // Go through each object
            foreach (PDKObject thisObject in objects)
            {
                // Apply the appropriate prefab
                thisObject.prefab = objectsInMap[thisObject.type];
            }
        }
    }
}
