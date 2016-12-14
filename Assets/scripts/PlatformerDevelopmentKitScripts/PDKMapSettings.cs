using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PDKMapSettings
{
    #region Variables for Map Settings
    // This is the text asset for this map
    [SerializeField]
    public TextAsset mapTextAsset;
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
            #region Create Object Prefab List
            // Insatiate the object dictionary
            pdkMap.objectsInMap = new Dictionary<string, Object>();
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
                        if (!pdkMap.objectsInMap.Keys.Contains(thisObject.type))
                        {
                            // Add this object type to the hashset of discovered object types
                            pdkMap.objectsInMap.Add(thisObject.type, null);
                        }
                    }
                }
            }
            #endregion
        }
        else
        {
            // Clear mapTextAsset
            mapTextAsset = null;
        }
    }

    // When this is called this TIKMapSettings coppies the values of another TIKMapSettings
    public void Clone(PDKMapSettings mapSettingsToClone)
    {
        // Clone each Variable
        mapTextAsset = mapSettingsToClone.mapTextAsset;
        pdkMap = mapSettingsToClone.pdkMap;
    }
    
    // When this is called settings from another map are copied onto this map's settings
    public void CopyMatchingSettings(PDKMapSettings mapSettingsToCopy)
    {
        #region Check and Copy Tilesets
        // If the map settings to copy tilesetTextures is not blank
        if (mapSettingsToCopy.pdkMap.tilesets != null && mapSettingsToCopy.pdkMap.height > 0)
        {
            // For each tileset in this map
            for (int tilesetToCheck = 0; tilesetToCheck < pdkMap.tilesets.Length; tilesetToCheck++)
            {
                // Go through each tileset in the map to copy
                for (int tilesetToCopy = 0; tilesetToCopy < mapSettingsToCopy.pdkMap.tilesets.Length; tilesetToCopy++)
                {
                    // If these tilesets have the same name
                    if (pdkMap.tilesets[tilesetToCheck].name == mapSettingsToCopy.pdkMap.tilesets[tilesetToCopy].name)
                    {
                        // Copy that tileset from mapSettingsToCopy to this TIKMapSettings
                        pdkMap.tilesets[tilesetToCheck] = mapSettingsToCopy.pdkMap.tilesets[tilesetToCopy];
                    }
                }
            }
        }
        #endregion
        #region Check and Copy Object Types
        // If the map to copy has objects
        if (mapSettingsToCopy.pdkMap.objectsInMap != null)
        {
            // For each object type in the map to copy
            foreach (string objectType in mapSettingsToCopy.pdkMap.objectsInMap.Keys)
            {
                // If this map has an object with the same name as the map to copy
                if (pdkMap.objectsInMap.Keys.Contains(objectType))
                {
                    // Copy the matching object from the map to copy
                    pdkMap.objectsInMap[objectType] = mapSettingsToCopy.pdkMap.objectsInMap[objectType];
                }
            }
        }
        #endregion
    }
}
