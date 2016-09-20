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
    private TIKMapSettings newMapSettings;
    // 
    public TIKMapSettings mapSettings = new TIKMapSettings(null);
    //
    private TIKMapSettings oldMapSettings = new TIKMapSettings(null);
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
            oldMapSettings.Clone(mapSettings);
            // Clear all remebered settings
            newMapSettings.Clone(mapSettings);
        }
        else if (oldMapSettings.mapTextAsset == null) // If no map was entered previously
        {
            // Create a new set of settings from the new TextAsset
            mapSettings = new TIKMapSettings(mapSettings.mapTextAsset);
            // Remember the current map settings
            oldMapSettings.Clone(mapSettings);
        }
        else if (oldMapSettings.mapTextAsset != mapSettings.mapTextAsset) // If the TextAsset for the map was changed
        {
            // Create a new set of map settings from the new TextAsset
            newMapSettings = new TIKMapSettings(mapSettings.mapTextAsset);
            // Copy over any settings that match between the current and the new TIKMapSettings
            newMapSettings.CopyMatchingSettings(mapSettings);
            // Display the new map settings
            mapSettings.Clone(newMapSettings);
            // Remember the current map settings
            oldMapSettings.Clone(mapSettings);
        }
    }


    void Awake()
    {
        // If a TIKMap has been created
        if (mapSettings.tikMap != null)
        {
            // Assign each of this level's TIKMap's tileset's texture variable to each of the textures the user has put in
            for (int tileset = 0; tileset < mapSettings.tilesetTextures.Length; tileset++)
            {
                mapSettings.tikMap.tilesets[tileset].imageTexture = mapSettings.tilesetTextures[tileset];
            }

            // Add the level controller to this object
            this.gameObject.AddComponent<PDKLevelController>();
            // Locate the level controller on this object
            PDKLevelController levelController = this.gameObject.GetComponent<PDKLevelController>();
            // Give the TIKMap with the user's settings to the levelController
            levelController.levelMap = mapSettings.tikMap;
            // Disable this script
            //this.gameObject.GetComponent<PDKLevelConfigurator>().enabled = false;
        }
    }
}
