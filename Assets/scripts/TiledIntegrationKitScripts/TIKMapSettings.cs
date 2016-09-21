using UnityEngine;
using System.Collections;

[System.Serializable]
public class TIKMapSettings
{
    #region Variables for Map Settings
    // This is the text asset for this map
    [SerializeField]
    public TextAsset mapTextAsset;
    // This is an array of textures for each tileset
    [SerializeField]
    public Texture2D[] tilesetTextures;
    #endregion

    // This is the TIKMap for this level
    public TIKMap tikMap;
    // Reference to TIK's jsonUtilies class
    private TIKJsonUtilities tikJsonUtilities = new TIKJsonUtilities();


    // When this is called a new TIKMapSettings is created based on a given TextAsset for a map
    public TIKMapSettings(TextAsset newMapTextAsset)
    {
        // If the new text asset exists
        if (newMapTextAsset != null)
        {
            // Remeber the text asset used to create this map
            mapTextAsset = newMapTextAsset;
            // Create a new TIKMap from the TextAsset for the new map
            tikMap = tikJsonUtilities.CreateTIKMapFromTextAsset(mapTextAsset);
            // Make tilesetTextures have enough slots for each tilest in the new map
            tilesetTextures = new Texture2D[tikMap.tilesets.Length];
        }
        else
        {
            // Clear mapTextAsset
            mapTextAsset = null;
            // Clear tilesetTexture
            tilesetTextures = null;
        }
    }

    // When this is called this TIKMapSettings coppies the values of another TIKMapSettings
    public void Clone(TIKMapSettings mapSettingsToClone)
    {
        // Clone each Variable
        mapTextAsset = mapSettingsToClone.mapTextAsset;
        tilesetTextures = mapSettingsToClone.tilesetTextures;
        tikMap = mapSettingsToClone.tikMap;
    }
    
    // When this is called settings from another map are copied onto this map's settings
    public void CopyMatchingSettings(TIKMapSettings mapSettingsToCopy)
    {
        #region Check and Copy Tilesets
        // For each tileset in this map
        for (int tilesetToCheck = 0; tilesetToCheck < tilesetTextures.Length; tilesetToCheck++)
        {
            // Go through each tileset in the map to copy
            for (int tilesetToCopy = 0; tilesetToCopy < mapSettingsToCopy.tilesetTextures.Length; tilesetToCopy++)
            {
                // If these tilesets have the same name
                if (tikMap.tilesets[tilesetToCheck].name == mapSettingsToCopy.tikMap.tilesets[tilesetToCopy].name)
                {
                    // Copy that tileset from mapSettingsToCopy to this TIKMapSettings
                    tilesetTextures[tilesetToCheck] = mapSettingsToCopy.tilesetTextures[tilesetToCopy];
                }
            }
        }
        #endregion
    }
}
