 using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

[Serializable]
public class PDKLayer
{
    [System.Serializable]
    public class PDKDehydratedObjectsHashSet : PDKSerializableHashSet<PDKObject> { }
    [System.Serializable]
    public class PDKGameObjectsHashSet : PDKSerializableHashSet<GameObject> { }
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
    public PDKSerializable2DArray dehydratedObjectMap;
    public PDKGameObjectsHashSet hydratedObjects;
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
                if (thisObject.prefab == null)
                {
                    Debug.Log("null");
                }
                else
                {
                    Debug.Log("not null");
                }
            }
        }
    }
    

    // Returns a refrence to all objects in a rect of the object map
    public HashSet<PDKObject> GetObjectsInRect(Rect rectToGet)
    {
        // Create a new hash set to put all objects in the given rect into
        HashSet<PDKObject> objectsInRect = new HashSet<PDKObject>();
        // TODO: COMMENT LATER
        PDKDehydratedObjectsHashSet hashSetToUse;

        // Go through each collumn in the given rect
        for (int currentColumnIndex = (int)rectToGet.xMin; currentColumnIndex < (int)rectToGet.xMax; currentColumnIndex++)
        {
            // Go through each row in this collumn
            for (int currentRowIndex = (int)rectToGet.yMin; currentRowIndex < (int)rectToGet.yMax; currentRowIndex++)
            {
                // TODO: COMMENT LATER
                hashSetToUse = dehydratedObjectMap.GetItem(currentColumnIndex, currentRowIndex);
                // For each object in this slot
                foreach (PDKObject currentObject in hashSetToUse)
                {
                    // Add this object to the objects to return
                    objectsInRect.Add(currentObject);
                    // Remove this object from this slot in the object map
                    hashSetToUse.Remove(currentObject);
                }
            }
        }
        return objectsInRect;
    }


    #region DehydratedObjectMap Manipulation
    // Places an object in the dehydrated objects map
    public void PutObjectInDehydratedMap(PDKObject objectToInsert, int tileWidth, int tileHeight)
    {
        // TODO: COMMENT LATER
        PDKDehydratedObjectsHashSet hashSetToUse = dehydratedObjectMap.GetItem(objectToInsert.x, objectToInsert.y);
        // Add this object at this position, to the object map
        hashSetToUse.Add(objectToInsert);
    }


    // Removes all objects in a given rect of the dehydrated objects map, and returns them in a hashset
    public HashSet<PDKObject> TakeDehydratedObjectsFromRect(Rect rectToTake)
    {
        // Create a new hash set to put all objects in the given rect into
        HashSet<PDKObject> objectsInRect = GetDehydratedObjectsInRect(rectToTake);

        // For each object at this slot in the dehydrated objects map
        foreach (PDKObject currentObject in objectsInRect)
        {
            // Remove the current object from the dehydrated objects map
            RemoveObjectFromDehydratedMap(currentObject);
        }
        return objectsInRect;
    }


    // Returns a hahset with all dehydrated objects in a given rect
    public  HashSet<PDKObject> GetDehydratedObjectsInRect(Rect rectToGet)
    {
        // Create a new hash set to put all objects in the given rect into
        HashSet<PDKObject> objectsInRect = new HashSet<PDKObject>();
        // TODO: COMMENT LATER
        PDKDehydratedObjectsHashSet hashSetToUse;

        // Go through each collumn in the given rect
        for (int currentColumnIndex = (int)rectToGet.xMin; currentColumnIndex < (int)rectToGet.xMax; currentColumnIndex++)
        {
            // Go through each row in this collumn
            for (int currentRowIndex = (int)rectToGet.yMin; currentRowIndex < (int)rectToGet.yMax; currentRowIndex++)
            {
                // TODO: COMMENT LATER
                hashSetToUse = dehydratedObjectMap.GetItem(currentColumnIndex, currentRowIndex);
                // For each object at this slot in the dehydrated objects map
                foreach (PDKObject currentObject in hashSetToUse)
                {
                    // Add the current object to the objects to return
                    objectsInRect.Add(currentObject);
                }
            }
        }
        return objectsInRect;
    }


    // Removes all instances of an object from the dehydrated objects map
    public void RemoveObjectFromDehydratedMap(PDKObject objectToRemove)
    {
        // TODO: COMMENT LATER
        PDKDehydratedObjectsHashSet hashSetToUse;
        // TODO: COMMENT LATER
        hashSetToUse = dehydratedObjectMap.GetItem(objectToRemove.x, objectToRemove.y);
        // Remove the current object from this slot in the dehydrated objects map
        hashSetToUse.Remove(objectToRemove);
    }
    #endregion


    #region HydratedObject Manipulation
    // This goes through each hydrated object, and destroys the ones that are outside of the given rect
    public void DehydrateExternalObjects(Rect rectToUse)
    {
        // Go through each hydrated object
        for (int thisObjectIndex = hydratedObjects.Count - 1; thisObjectIndex >= 0; thisObjectIndex--)
        {
            // If this object is out of the rect use
            if (!rectToUse.Overlaps(new Rect(hydratedObjects.First().transform.position.x, hydratedObjects.First().transform.position.y, 16, 16)))
            {
                // Remove this object from the hydrated objects
                hydratedObjects.Remove(hydratedObjects.First());
                // Dehydrate this object
                DehydrateObject(hydratedObjects.First());
            }
        }
    }

    // Creates a hydrated version of a dehydrated object
    public GameObject HydrateObject(PDKObject objectToHydrate)
    {
        // Will store the hydrated object to return
        GameObject hydratedObject;
        // Will store the hydrated object's properties
        PDKObjectProperties hydratedObjectProperties;

        // Instantiate this object
        if (objectToHydrate.prefab == null)
        {
            hydratedObject = (GameObject)GameObject.Instantiate(objectToHydrate.prefab, new Vector3(objectToHydrate.x, objectToHydrate.y, 0), new Quaternion(0, 0, 0, 0));
            hydratedObjectProperties = hydratedObject.GetComponent<PDKObjectProperties>();
        }
        hydratedObject = (GameObject)GameObject.Instantiate(objectToHydrate.prefab, new Vector3(objectToHydrate.x, objectToHydrate.y, 0), new Quaternion(0, 0, 0, 0));
        hydratedObjectProperties = hydratedObject.GetComponent<PDKObjectProperties>();
        // Copy each property from the dehydrated objet to the hydrated object
        foreach (string propertyName in objectToHydrate.properties.Keys)
        {
            hydratedObjectProperties.objectProperties.Add(propertyName, objectToHydrate.properties[propertyName]);
        }
        // Return the newly hydrated object
        return hydratedObject;
    }


    // Dehydrates a given object
    public PDKObject DehydrateObject(GameObject objectToDehydrate)
    {
        // Will sstore the dehydrated object to return
        PDKObject dehydratedObject = new PDKObject();
        // Will store the hydrated object's properties
        PDKObjectProperties hydratedObjectProperties;

        // 
        hydratedObjectProperties = objectToDehydrate.GetComponent<PDKObjectProperties>();
        // Copy each property from the hydrated objet to the dehydrated object
        foreach (string propertyName in hydratedObjectProperties.objectProperties.Keys)
        {
            dehydratedObject.properties.Add(propertyName, hydratedObjectProperties.objectProperties[propertyName]);
        }
        // Destory the hydrated obejct
        GameObject.Destroy(objectToDehydrate);
        // Return the dehydrated object
        return dehydratedObject;
    }
    #endregion
}
