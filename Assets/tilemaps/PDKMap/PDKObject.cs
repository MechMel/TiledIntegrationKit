﻿using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

[Serializable]
public class PDKObject : MonoBehaviour
{
    public Vector2 offsetFromTopLeft;
    [HideInInspector]
    public PDKMap.PDKCustomProperties properties;
    [HideInInspector]
    public string objectName;
    [HideInInspector]
    public string type;
    [HideInInspector]
    public int id;
    [HideInInspector]
    public Vector3 position;
    [HideInInspector]
    public int rotation;
    [HideInInspector]
    public UnityEngine.Object prefab;
    [HideInInspector]
    public PDKLayer layerThisObjectIsIn;

    
    private void OnDestroy()
    {
        // Tell this object's layer this object has been destroyed
        layerThisObjectIsIn.HydratedObectHasBeenDestoryed(gameObject);
    }

    // TODO: COMMENT THIS LATER
    public PDKObject()
    {
        properties = new PDKMap.PDKCustomProperties();
    }

    // TODO: COMMENT THIS LATER
    public PDKObject(PDKObject objectToCopy)
    {
        Copy(objectToCopy);
    }

    // This copies the properties from a given object
    public void Copy(PDKObject objectToCopy)
    {
        // Copy each property
        this.offsetFromTopLeft = objectToCopy.offsetFromTopLeft;
        this.objectName = objectToCopy.objectName;
        this.id = objectToCopy.id;
        this.type = objectToCopy.type;
        this.position = objectToCopy.position;
        this.rotation = objectToCopy.rotation;
        this.prefab = objectToCopy.prefab;
        this.layerThisObjectIsIn = objectToCopy.layerThisObjectIsIn;
        this.properties = objectToCopy.properties;
    }


    // Creates a hydrated version of this object and puts it in the hydrated object map
    public void Hydrate(PDKLayer layerThisObjectIsIn)
    {
        // Instatieate the hydrated version of this object
        GameObject hydratedObject = (GameObject)GameObject.Instantiate(prefab, position, new Quaternion(0, 0, rotation, 0));

        // Copy this pdkObject to the hydrated version of this object
        hydratedObject.GetComponent<PDKObject>().Copy(this);
        // Remove this object from the dehydrated object map
        layerThisObjectIsIn.RemoveDehydratedObject(this);
        // Add this object to the hydrated object map
        layerThisObjectIsIn.hydratedObjects.Add(hydratedObject);
        // Destroy the dehydrated version of this object
        Destroy(this);
    }

    // Creates a dehydrates version of this object and puts it in the dehydrated object map
    public void Dehydrate()
    {
        // Create a copy of this pdkObject
        PDKObject dehydratedObject = new PDKObject(this);

        // Update the dehydrated version of this object's position
        dehydratedObject.position = transform.position;
        // Remove the hydrated version of this object from the hydrated object map
        layerThisObjectIsIn.hydratedObjects.Remove(gameObject);
        // Put the dehydrated version of this object in the dehydrated object map
        layerThisObjectIsIn.PutObjectInDehydratedMap(dehydratedObject);
        // Destory the hydrated version of this obejct
        Destroy(gameObject);
    }
}
