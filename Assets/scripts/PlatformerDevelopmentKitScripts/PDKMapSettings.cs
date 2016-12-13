using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PDKMapSettings
{
    #region Variables for Map Settings
    // This is the text asset for this map
    [SerializeField]
    public TextAsset mapTextAsset;
    // This is an array of textures for each tileset
    [SerializeField]
    public Texture2D[] tilesetTextures;
    // TODO: FILL THIS IN LATER
    [SerializeField]
    public Object[] objectPrefabs;
    // TODO: FILL THIS IN LATER
    [SerializeField]
    public List<string> objectTypes;
    #endregion

    // This is the PDKMap for this level
    public PDKMap pdkMap;
    // Reference to PDK's jsonUtilies class
    private PDKTiledUtilities pdkJsonUtilities = new PDKTiledUtilities();


    // When this is called a new TIKMapSettings is created based on a given TextAsset for a map
    public PDKMapSettings(TextAsset newMapTextAsset)
    {
        // If the new text asset exists
        if (newMapTextAsset != null)
        {
            // Remeber the text asset used to create this map
            mapTextAsset = newMapTextAsset;
            // Create a new TIKMap from the TextAsset for the new map
            pdkMap = pdkJsonUtilities.CreatePDKMapFromTextAsset(mapTextAsset);
            // Make tilesetTextures have enough slots for each tilest in the new map
            tilesetTextures = new Texture2D[pdkMap.tilesets.Length];
            #region Create Object Prefab List
            // Instatiate the object types list
            objectTypes = new List<string>();
            // For each layer in this map
            foreach (PDKLayer thisLayer in pdkMap.layers)
            {
                // If this layer is an object layer
                if (thisLayer.type == PDKLayer.layerTypes.Object)
                {
                    // Go through each object in this layer
                    foreach (PDKObject thisObject in thisLayer.objects)
                    {
                        // If this object type has not been found already
                        if (!objectTypes.Contains(thisObject.type))
                        {
                            // Add this object type to the hashset of discovered object types
                            objectTypes.Add(thisObject.type);
                        }
                    }
                }
            }
            // Make obectPrefabs have enough slots for each object type in the new map
            objectPrefabs = new Object[objectTypes.Count];
            #endregion
        }
        else
        {
            // Clear mapTextAsset
            mapTextAsset = null;
            // Clear tilesetTexture
            tilesetTextures = null;
            // Clear objectPrefabs
            objectPrefabs = null;
            // Clear objectPrefabs
            objectPrefabs = null;
        }
    }

    // When this is called this TIKMapSettings coppies the values of another TIKMapSettings
    public void Clone(PDKMapSettings mapSettingsToClone)
    {
        // Clone each Variable
        mapTextAsset = mapSettingsToClone.mapTextAsset;
        pdkMap = mapSettingsToClone.pdkMap;
        tilesetTextures = mapSettingsToClone.tilesetTextures;
        objectTypes = mapSettingsToClone.objectTypes;
        objectPrefabs = mapSettingsToClone.objectPrefabs;
    }
    
    // When this is called settings from another map are copied onto this map's settings
    public void CopyMatchingSettings(PDKMapSettings mapSettingsToCopy)
    {
        #region Check and Copy Tilesets
        // If the map settings to copy tilesetTextures is not blank
        if (mapSettingsToCopy.tilesetTextures != null && mapSettingsToCopy.pdkMap.height > 0)
        {
            // For each tileset in this map
            for (int tilesetToCheck = 0; tilesetToCheck < tilesetTextures.Length; tilesetToCheck++)
            {
                // Go through each tileset in the map to copy
                for (int tilesetToCopy = 0; tilesetToCopy < mapSettingsToCopy.tilesetTextures.Length; tilesetToCopy++)
                {
                    // If these tilesets have the same name
                    if (pdkMap.tilesets[tilesetToCheck].name == mapSettingsToCopy.pdkMap.tilesets[tilesetToCopy].name)
                    {
                        // Copy that tileset from mapSettingsToCopy to this TIKMapSettings
                        tilesetTextures[tilesetToCheck] = mapSettingsToCopy.tilesetTextures[tilesetToCopy];
                    }
                }
            }
        }
        #endregion
        #region Check and Copy Object Prefabs
        // If the map settings to copy obectPrefabs is not blank
        if (mapSettingsToCopy.objectPrefabs != null)
        {
            // For each object type in this map
            for (int thisObjectIndex = 0; thisObjectIndex < objectTypes.Count; thisObjectIndex++)
            {
                // If this object type exists in the map settings to copy
                if (mapSettingsToCopy.objectTypes.Contains(objectTypes[thisObjectIndex]))
                {
                    // Copy the prefab for this object from the map settings to copy
                    objectPrefabs[thisObjectIndex] = mapSettingsToCopy.objectPrefabs[thisObjectIndex];
                }
            }
        }
        #endregion
    }
}
