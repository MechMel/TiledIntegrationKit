 using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class PDKLayer
{
    public enum layerTypes { Tile, Object, Image };
    public string name;
    public layerTypes type;
    public int height;
    public int width;
    public bool visible;
    public int opacity;
    public int horizontalOffset;
    public int verticalOffset;
    public PDKMap.PDKCustomProperties properties;
    // Tile Layer Attributes
    public int[] tileMap;
    // Object Layer Attributes
    public PDKObject[] objects;
    public HashSet<PDKObject>[][] objectMap;
    // Image Layer Attributes
    public Texture2D image;



    //The Initialize function sets up the tile layer, objects, and images
    public void InitializeLayer(Dictionary<string, UnityEngine.Object> objectsInMap)
    {
        // If this is an object layer
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
    

    // Returns a refrence to all objects in a rect of the object map
    public HashSet<PDKObject> GetObjectsInRect(Rect rectToGet)
    {
        // Create a new hash set to put all objects in the given rect into
        HashSet<PDKObject> objectsInRect = new HashSet<PDKObject>();

        // Go through each collumn in the given rect
        for (int currentCollumnIndex = (int)rectToGet.xMin; currentCollumnIndex < (int)rectToGet.xMax; currentCollumnIndex++)
        {
            // Go through each row in this collumn
            for (int currentRowIndex = (int)rectToGet.yMin; currentRowIndex < (int)rectToGet.yMax; currentRowIndex++)
            {
                // Add each object from this slot in the object map
                foreach (PDKObject currentObject in objectMap[currentCollumnIndex][currentRowIndex])
                {
                    objectsInRect.Add(currentObject);
                }
            }
        }
        return objectsInRect;
    }
}
