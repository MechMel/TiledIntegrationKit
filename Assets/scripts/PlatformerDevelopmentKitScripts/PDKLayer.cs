using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Xml;

[Serializable]
public class PDKLayer
{
    #region Layer Attributes
    public string name;
    public string type;
    public int height;
    public int width;
    public bool visible;
    public int opacity;
    public int x;
    public int y;

    public enum layerTypes { Tile, Object, Image };
    public layerTypes layerType;
    // Stores the custom properties for this map
    public PDKCustomProperty[] properties;
    #endregion

    #region Tile Layer Attributes
    public int[] data;
    #endregion

    #region Object Layer Attributes
    public string draworder;
    public PDKObject[] objects;
    #endregion

    #region Image Layer Attributes
    public string image;
    #endregion

    //The Initialize function sets up the tile layer, objects, and images
    public void InitializeLayer(Dictionary<string, UnityEngine.Object> objectsInMap)
    {
        if (type == "tilelayer")
        {
            layerType = layerTypes.Tile;
        }
        else if(type == "objectgroup")
        {
            layerType = layerTypes.Object;
            // Go through each object
            foreach (PDKObject thisObject in objects)
            {
                // Apply the appropriate prefab
                thisObject.prefab = objectsInMap[thisObject.type];
            }
        }
        else if(type == "imagelayer")
        {
            layerType = layerTypes.Image;
        }
    }

    /*
    // When this is called it creates a Layer from a given XmlNode for a layer
    public TIKLayer(XmlNode tileLayerXmlNode)
    {
        // Add all of the layer's attributes to this Layer
        name = tileLayerXmlNode.Attributes["name"].Value;
    }
    */
}
