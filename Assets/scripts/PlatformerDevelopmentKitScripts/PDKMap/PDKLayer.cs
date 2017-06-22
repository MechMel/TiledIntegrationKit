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
    public PDKSerializable2DHashsetOfPDKObjects dehydratedObjectMap;
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
                // Put this object in the object map
                dehydratedObjectMap.GetItem((int)thisObject.x, -(int)thisObject.y).Add(thisObject);
            }
        }
    }


    #region Object Dehydration
    // This goes through each hydrated object, and destroys the ones that are outside of the given rect
    public void DehydrateExternalObjects(Rect rectToUse)
    {
        // This stores the objects to dehydrate
        List<GameObject> objectsToDehydrate = new List<GameObject>();

        // Find all the objects that are outside of the given rect
        foreach (GameObject objectToCheck in hydratedObjects)
        {
            // If this object is out of the rect use
            if (!rectToUse.Overlaps(new Rect(objectToCheck.transform.position.x, objectToCheck.transform.position.y, 1, -1)))
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
        }
    }


    // Dehydrates a given object
    public void DehydrateObject(GameObject objectToDehydrate)
    {
        // Store the hydrated object's properties
        PDKObjectProperties hydratedObjectProperties = objectToDehydrate.GetComponent<PDKObjectProperties>();
        // Will sstore the dehydrated object to return
        PDKObject dehydratedObject = objects[hydratedObjectProperties.id];

        // If this object has custom properties, save them
        if (hydratedObjectProperties.objectProperties != null)
        {
            // TODO: COMMENT THIS LATER
            foreach (string key in hydratedObjectProperties.objectProperties.Keys)
            {
                if (dehydratedObject.properties.Keys.Contains(key))
                {
                    dehydratedObject.properties[key] = hydratedObjectProperties.objectProperties[key];
                }
                else
                {
                    dehydratedObject.properties.Add(key, hydratedObjectProperties.objectProperties[key]);
                }
            }
        }
        // Store the hydrated object's x and y position
        dehydratedObject.x = objectToDehydrate.transform.position.x;
        dehydratedObject.y = objectToDehydrate.transform.position.y;
        // Remove this object from the hydrated object map
        hydratedObjects.Remove(objectToDehydrate);
        // Destory the hydrated obejct
        GameObject.Destroy(objectToDehydrate);
        // Put this object in the dehydrated object map
        PutObjectInDehydratedMap(dehydratedObject);
    }


    // Places an object in the dehydrated objects map
    public void PutObjectInDehydratedMap(PDKObject objectToPlace)
    {
        if (objectToPlace.x < dehydratedObjectMap.Width && -objectToPlace.y < dehydratedObjectMap.Height)
        {
            dehydratedObjectMap.GetItem((int)objectToPlace.x, -(int)objectToPlace.y).Add(objectToPlace);
        }
    }
    #endregion

    #region Object Hydration
    //
    public void HydrateInternalObjects(Rect rectToHydrate)
    {
        // Create a new hash set to put all objects in the given rect into
        List<PDKObject> objectsToHydrate = new List<PDKObject>();

        // Go through each slot in this rect and add all dehydrated objects, at that slot, to the objects to hydrate
        for (int x = (int)rectToHydrate.xMin; x < (int)rectToHydrate.xMax; x++)
        {
            // Go through each row in this collumn
            for (int y = (int)rectToHydrate.yMin; y < (int)rectToHydrate.yMax; y++)
            {
                foreach (PDKObject objectToAdd in dehydratedObjectMap.GetItem(x, y))
                {
                    // Add this object to the objects to hydrate
                    objectsToHydrate.Add(objectToAdd);
                }
            }
        }
        // Hydrate each of the objects in the list of objects to dehydrate
        for (int indexOfObjectToHydrate = 0; indexOfObjectToHydrate < objectsToHydrate.Count; indexOfObjectToHydrate++)
        {
            HydrateObject(objectsToHydrate[indexOfObjectToHydrate]);
        }
    }

    // Creates a hydrated version of a dehydrated object
    public void HydrateObject(PDKObject objectToHydrate)
    {
        // Will store the hydrated object to return
        GameObject hydratedObject;
        // Will store the hydrated object's properties
        PDKObjectProperties hydratedObjectProperties;

        //
        hydratedObject = (GameObject)GameObject.Instantiate(objectToHydrate.prefab, new Vector3(objectToHydrate.x, objectToHydrate.y, 0), new Quaternion(0, 0, 0, 0));
        hydratedObjectProperties = hydratedObject.GetComponent<PDKObjectProperties>();
        // Set this object's ID and GID'
        hydratedObjectProperties.id = objectToHydrate.id;
        hydratedObjectProperties.gid = objectToHydrate.gid;
        // Tell this object which layer it is in
        hydratedObjectProperties.layerThisObjectIsIn = this;
        // Copy the custom properties from the dehydrated objet to the hydrated object
        if (objectToHydrate.properties != null)
        {
            // TODO:COMMENTLATER
            foreach (string key in objectToHydrate.properties.Keys)
            {
                if (hydratedObjectProperties.objectProperties.Keys.Contains(key))
                {
                    hydratedObjectProperties.objectProperties[key] = objectToHydrate.properties[key];
                }
                else
                {
                    hydratedObjectProperties.objectProperties.Add(key, objectToHydrate.properties[key]);
                }
            }
        }
        // Remvoe this object from the dehydrated object map
        RemoveDehydratedObject(objectToHydrate);
        // Add this object to the hydrated object map
        hydratedObjects.Add(hydratedObject);
    }

    // Remvoes a object from the dehydrated object map
    public void RemoveDehydratedObject(PDKObject objectToTake)
    {
        // Remove this object from the dehydrated object map
        dehydratedObjectMap.GetItem((int)objectToTake.x, -(int)objectToTake.y).Remove(objectToTake);
    }
    #endregion

    #region Object Map Manipulation
    /* When a hydrated object is destroyed it calls this function to tell this layer to
     * remove this object from the hydrated object map */
    public void HydratedObectDestoryed(GameObject destoyedObject)
    {
        // Remove this object from the hydrated object map
        hydratedObjects.Remove(destoyedObject);
    }
    #endregion
}
    /*// Returns a refrence to all objects in a rect of the object map
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
            if (!rectToUse.Overlaps(new Rect(objectToCheck.transform.position.x, -objectToCheck.transform.position.y, 1, 1)))
            {
                // Add this object to the list of objects to dehydrate
                objectsToDehydrate.Add(objectToCheck);
            }
        }
        // Dehydrate each of the objects in the list of objects to dehydrate
        for (int indexOfObjectToDehydrate = 0; indexOfObjectToDehydrate < objectsToDehydrate.Count; indexOfObjectToDehydrate++)
        {
            // Remove this object from the hydrated objects
            hydratedObjects.Remove(objectsToDehydrate[indexOfObjectToDehydrate]);
            // Dehydrate this object
            PDKObject dehydratedObject = DehydrateObject(objectsToDehydrate[indexOfObjectToDehydrate]);
            // Put this object in the dehydrated object map
            PutObjectInDehydratedMap(dehydratedObject, 16, 16);
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
        if (dehydratedObject.properties != null)
        {
            foreach (string key in dehydratedObject.properties.Keys)
            {
                if (hydratedObjectProperties.objectProperties.Keys.Contains(key))
                {
                    hydratedObjectProperties.objectProperties[key] = dehydratedObject.properties[key];
                }
                else
                {
                    hydratedObjectProperties.objectProperties.Add(key, dehydratedObject.properties[key]);
                }
            }
        }
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
            foreach (string key in hydratedObjectProperties.objectProperties.Keys)
            {
                if (dehydratedObject.properties.Keys.Contains(key))
                {
                    dehydratedObject.properties[key] = hydratedObjectProperties.objectProperties[key];
                }
                else
                {
                    dehydratedObject.properties.Add(key, hydratedObjectProperties.objectProperties[key]);
                }
            }
        }
        // TODO: FILL THIS IN LATER
        dehydratedObject.x = objectToDehydrate.transform.position.x;
        dehydratedObject.x = objectToDehydrate.transform.position.x;
        // Destory the hydrated obejct
        GameObject.Destroy(objectToDehydrate);
        // Return the dehydrated object
        return dehydratedObject;
    }
    #endregion*/
