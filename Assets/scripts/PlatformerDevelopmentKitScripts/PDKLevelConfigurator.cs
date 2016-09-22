using UnityEngine;
using System.Collections;

[System.Serializable]
public class PDKLevelConfigurator : MonoBehaviour
{
    // This is how far from out from the camera to load this level
    [SerializeField]
    public int loadDistance;
    // This is how close the camera can get to the edge of the loaded area before a new section should be loaded
    [SerializeField]
    public int bufferDistance;
    // This is all possible map types a user can choose
    public enum mapTypes { None, Tiled };
    // This will be used to track the map type that the user has chosen
    [SerializeField]
    public mapTypes mapType = mapTypes.None;

    #region Tiled Map Settings
    //
    private TIKMapSettings newMapSettings;
    // 
    [SerializeField]
    public TIKMapSettings mapSettings = new TIKMapSettings(null);
    #endregion

    // This is called to update variables when the text asset has been changed
    public void TextAssetChanged()
    {
        if (mapSettings.mapTextAsset == null) // If the current map has been removed
        {
            // Clear all current settings
            mapSettings = new TIKMapSettings(mapSettings.mapTextAsset);
            // Clear all remebered settings
            newMapSettings.Clone(mapSettings);
        }
        else // If a new text asset has been put in
        {
            // Create a new set of map settings from the new TextAsset
            newMapSettings = new TIKMapSettings(mapSettings.mapTextAsset);
            // Copy over any settings that match between the current and the new TIKMapSettings
            newMapSettings.CopyMatchingSettings(mapSettings);
            // Display the new map settings
            mapSettings.Clone(newMapSettings);
        }
    }


    void Awake()
    {
        // If a TIKMap has been created
        if (mapSettings.tikMap != null && mapSettings.mapTextAsset != null)
        {
            // Tell this level's map to initialize
            mapSettings.tikMap.InitializeMap(mapSettings.tilesetTextures);
            // Add the level controller to this object
            this.gameObject.AddComponent<PDKLevelController>();
            // Locate the level controller on this object
            PDKLevelController levelController = this.gameObject.GetComponent<PDKLevelController>();
            // Give the TIKMap with the user's settings to the levelController
            levelController.levelMap = mapSettings.tikMap;
            // Tell the level controller how far from the camera to load this level
            levelController.loadDistance = loadDistance;
            // Tell the level controller how close the camera can get to the edge of the loaded area before a new section of the level should be loaded
            levelController.bufferDistance = bufferDistance;
            // Disable this script
            //this.gameObject.GetComponent<PDKLevelConfigurator>().enabled = false;
        }
    }
}
