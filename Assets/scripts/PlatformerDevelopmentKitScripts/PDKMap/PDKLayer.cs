 using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

[Serializable]
public class PDKLayer
{
    [System.Serializable]
    public class PDKObjectIDHashSet : PDKSerializableHashSet<int> { }
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
    public PDKSerializable2DHashestOfInts dehydratedObjectIDMap;
    public PDKGameObjectsHashSet hydratedObjects;
    // Image Layer Attributes
    public Texture2D image;



    //The Initialize function sets up the tile layer, objects, and images
    public void InitializeLayer(SortedDictionary<string, UnityEngine.Object> objectsInMap)
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
        // TODO: COMMENT LATER
        PDKObjectIDHashSet hashSetToUse;

        // Go through each collumn in the given rect
        for (int currentColumnIndex = (int)rectToGet.xMin; currentColumnIndex < (int)rectToGet.xMax; currentColumnIndex++)
        {
            // Go through each row in this collumn
            for (int currentRowIndex = (int)rectToGet.yMin; currentRowIndex < (int)rectToGet.yMax; currentRowIndex++)
            {
                // TODO: COMMENT LATER
                hashSetToUse = dehydratedObjectIDMap.GetIDs(currentColumnIndex, currentRowIndex);
                // For each object in this slot
                foreach (int objectID in hashSetToUse)
                {
                    // Add this object to the objects to return
                    objectsInRect.Add(objects[objectID]);
                    // Remove this object from this slot in the object map
                    dehydratedObjectIDMap.RemoveID(currentColumnIndex, currentRowIndex, objectID);
                }
            }
        }
        return objectsInRect;
    }


    #region DehydratedObjectMap Manipulation
    // Places an object in the dehydrated objects map
    public void PutObjectInDehydratedMap(PDKObject objectToInsert, int tileWidth, int tileHeight)
    {
        // Add this object at this position, to the object map
        dehydratedObjectIDMap.AddID((int)objectToInsert.x, (int)objectToInsert.y, objectToInsert.id);
    }


    // Removes all objects in a given rect of the dehydrated objects map, and returns them in a hashset
    public PDKObjectIDHashSet TakeDehydratedObjectIDsInRect(Rect rectToTake)
    {
        // Create a new hash set to put all objects in the given rect into
        PDKObjectIDHashSet objectIDsInRect = GetDehydratedObjectIDsInRect(rectToTake);

        // For each object at this slot in the dehydrated objects map
        foreach (int objectID in objectIDsInRect)
        {
            // Remove the current object from the dehydrated objects map
            RemoveObjectFromDehydratedMap(objects[objectID]);
        }
        return objectIDsInRect;
    }


    // Returns a hahset with all dehydrated objects in a given rect
    public  PDKObjectIDHashSet GetDehydratedObjectIDsInRect(Rect rectToGet)
    {
        // Create a new hash set to put all objects in the given rect into
        PDKObjectIDHashSet objectsInIDsRect = new PDKObjectIDHashSet();
        // TODO: COMMENT LATER
        PDKObjectIDHashSet hashSetToUse;

        // Go through each collumn in the given rect
        for (int currentColumnIndex = (int)rectToGet.xMin; currentColumnIndex < (int)rectToGet.xMax; currentColumnIndex++)
        {
            // Go through each row in this collumn
            for (int currentRowIndex = (int)rectToGet.yMin; currentRowIndex < (int)rectToGet.yMax; currentRowIndex++)
            {
                // TODO: COMMENT LATER
                hashSetToUse = dehydratedObjectIDMap.GetIDs(currentColumnIndex, currentRowIndex);
                // For each object at this slot in the dehydrated objects map
                foreach (int objectID in hashSetToUse)
                {
                    if (objectID == 50) { }
                    // Add the current object to the objects to return
                    objectsInIDsRect.Add(objectID);
                }
            }
        }
        return objectsInIDsRect;
    }


    // Removes all instances of an object from the dehydrated objects map
    public void RemoveObjectFromDehydratedMap(PDKObject objectToRemove)
    {
        // Remove the current object from this slot in the dehydrated objects map
        dehydratedObjectIDMap.RemoveID((int)objectToRemove.x, (int)objectToRemove.y, objectToRemove.id);
    }
    #endregion


    #region HydratedObject Manipulation
    // This goes through each hydrated object, and destroys the ones that are outside of the given rect
    public void DehydrateExternalObjects(Rect rectToUse)
    {
        // This stores the objects to dehydrate
        List<GameObject> objectsToDehydrate = new List<GameObject>();

        // Find all the objects that are outside of the given rect
        foreach (GameObject objectToCheck in hydratedObjects)
        {
            // If this object is out of the rect use
            if (!rectToUse.Overlaps(new Rect(objectToCheck.transform.position.x, objectToCheck.transform.position.y, 16, 16)))
            {
                // Add this object to the list of objects to dehydrate
                objectsToDehydrate.Add(objectToCheck);
            }
        }
        // Dehydrate each of the objects in the list of objects to dehydrate
        for (int indexOfObjectToDehydrate = 0; indexOfObjectToDehydrate < objectsToDehydrate.Count; indexOfObjectToDehydrate++)
        {
            // Dehydrate this object
            DehydrateObject(objectsToDehydrate[indexOfObjectToDehydrate]);
            // Remove this object from the hydrated objects
            hydratedObjects.Remove(objectsToDehydrate[indexOfObjectToDehydrate]);
        }
    }

    // Creates a hydrated version of a dehydrated object
    public GameObject HydrateObject(int idOfObjectToHydrate)
    {
        // Stores the dehydrated object for the given ID
        PDKObject dehydratedObject = objects[idOfObjectToHydrate];
        // Will store the hydrated object to return
        GameObject hydratedObject;
        // Will store the hydrated object's properties
        PDKObjectProperties hydratedObjectProperties;
        
        hydratedObject = (GameObject)GameObject.Instantiate(dehydratedObject.prefab, new Vector3(dehydratedObject.x, dehydratedObject.y, 0), new Quaternion(0, 0, 0, 0));
        hydratedObjectProperties = hydratedObject.GetComponent<PDKObjectProperties>();
        // Set this object's ID and GID'
        hydratedObjectProperties.id = dehydratedObject.id;
        hydratedObjectProperties.gid = dehydratedObject.gid;
        // Copy the custom properties from the dehydrated objet to the hydrated object
        hydratedObjectProperties.objectProperties = dehydratedObject.properties;
        // Return the newly hydrated object
        return hydratedObject;
    }


    // Dehydrates a given object
    public PDKObject DehydrateObject(GameObject objectToDehydrate)
    {
        // Store the hydrated object's properties
        PDKObjectProperties hydratedObjectProperties = objectToDehydrate.GetComponent<PDKObjectProperties>();
        // Will sstore the dehydrated object to return
        PDKObject dehydratedObject = objects[hydratedObjectProperties.id];
        
        // If this object has custom properties, save them
        if (hydratedObjectProperties.objectProperties != null)
        {
            dehydratedObject.properties = hydratedObjectProperties.objectProperties;
        }
        // TODO: FILL THIS IN LATER
        dehydratedObject.x = objectToDehydrate.transform.position.x;
        dehydratedObject.x = objectToDehydrate.transform.position.x;
        // Destory the hydrated obejct
        GameObject.Destroy(objectToDehydrate);
        // Return the dehydrated object
        return dehydratedObject;
    }
    #endregion
}
