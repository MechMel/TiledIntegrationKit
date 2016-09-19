using UnityEngine;
using System.Collections;

public class PDKLevelConfigurator : MonoBehaviour
{
    // This is all possible map types a user can choose
    public enum mapTypes { None, Tiled };
    // This will be used to track the map type that the user has chosen
    public mapTypes mapType = mapTypes.None;

    #region Tiled Map Settings
    //
    public TIKMapSettings newMapSettings;
    // 
    public TIKMapSettings mapSettings;
    //
    public TIKMapSettings oldMapSettings;
    #endregion


    // When this is called the current map type is evaluated and appropriate action is taken
    public bool UpdateMapSettings()
    {
        if (mapType == mapTypes.Tiled) // If the current map type is a Tiled map
        {
            // Update the appropriate settings for a Tiled map
            UpdateTiledMapSettings();
        }
        // Variables have been updated
        return true;
    }

    // When this is called the current tiled map settings are compared with the old map settings to determine what has been changed. Apprpriate action is then taken
    private void UpdateTiledMapSettings()
    {
        if (mapSettings.mapTextAsset == null) // If the current map has been removed
        {
            // Clear all current settings
            mapSettings = new TIKMapSettings(mapSettings.mapTextAsset);
            // Clear all remebered settings
            oldMapSettings = mapSettings;
            // Clear all remebered settings
            newMapSettings = mapSettings;
        }
        else if (oldMapSettings.mapTextAsset == null) // If no map was entered previously
        {
            // Create a new set of settings from the new TextAsset
            mapSettings = new TIKMapSettings(mapSettings.mapTextAsset);
            // Remember the current map settings
            oldMapSettings = mapSettings;
        }
        else if (oldMapSettings.mapTextAsset != mapSettings.mapTextAsset) // If the TextAsset for the map was changed
        {
            // Create a new set of map settings from the new TextAsset
            newMapSettings = new TIKMapSettings(mapSettings.mapTextAsset);
            // Copy over any settings that match between the current and the new TIKMapSettings
            newMapSettings.CopyMatchingSettings(mapSettings);
            // Display the new map settings
            mapSettings = newMapSettings;
            // Remember the current map settings
            oldMapSettings = mapSettings;
        }
    }


    void Awake()
    {
        // Assign each of this level's TIKMap's tileset's texture variable to each of the textures the user has put in
        for (int tileset = 0; tileset < tilesetTextures.Length; tileset++)
        {
            levelMap.tilesets[tileset].imageTexture = tilesetTextures[tileset];
        }
    }
}
