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
                // For each object in this slot
                foreach (PDKObject currentObject in objectMap[currentCollumnIndex][currentRowIndex])
                {
                    // Add this object to the objects to return
                    objectsInRect.Add(currentObject);
                    // Remove this object from this slot in the object map
                    objectMap[currentCollumnIndex][currentRowIndex].Remove(currentObject);
                }
            }
        }
        return objectsInRect;
    }


    // Places an object in the object map
    public void PutObjectInMap(PDKObject objectToInsert, int tileWidth, int tileHeight)
    {
        // Go through each collumn of the map that this object exists in
        for (int currentColumnIndex = (int)objectToInsert.objectRect.x / tileWidth;
            currentColumnIndex < Math.Ceiling(objectToInsert.objectRect.xMax / tileHeight); currentColumnIndex++)
        {
            // Go through each row of the map that this object exists in
            for (int currentRowIndex = (int)objectToInsert.objectRect.y / tileWidth;
                currentRowIndex < Math.Ceiling(objectToInsert.objectRect.yMax / tileHeight); currentRowIndex++)
            {
                // Add this object at this position, to the object map
                objectMap[currentColumnIndex][currentRowIndex].Add(objectToInsert);
            }
        }
    }
}
