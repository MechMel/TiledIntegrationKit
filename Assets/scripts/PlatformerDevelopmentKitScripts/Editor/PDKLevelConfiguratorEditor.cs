using UnityEngine;
using System.Linq;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(PDKLevelConfigurator))]
public class PDKLevelConfiguratorEditor : Editor
{
    //
    private PDKEditorUtil editorUtilities;
    
    //
    private void OnEnable()
    {
        editorUtilities = ScriptableObject.CreateInstance<PDKEditorUtil>();
    }  

    // This will overide unity's standard GUI
    public override void OnInspectorGUI()
    {
        // Define the target script to use
        PDKLevelConfigurator levelConfigurator = (PDKLevelConfigurator)target;

        // Display a field for buffer distance
        editorUtilities.Field("Buffer Distance", ref levelConfigurator.bufferDistance);
        // Display the map type selection drop down
        editorUtilities.Field("Map Type", ref levelConfigurator.mapType);
        #region Display Appropriate Fields for this MapType
        // If the user has picked a tiled map
        if (levelConfigurator.mapType == PDKLevelConfigurator.mapTypes.Tiled) // If the user has selected a Tiled map type
        {
            // Create the appropriate fields for a tiled map
            if (editorUtilities.Field("Tile Map", ref levelConfigurator.mapTextAsset))// If the text asset is changed
            {
                // Tell the level configurator the text asset has been changed
                levelConfigurator.TextAssetChanged();
            }
        }
        #endregion
        #region When Applicable Display the Tileset Fields
        // If tilesetTextures has ben instatiated, and the map type is not none
        if (levelConfigurator.pdkMap.tilesets != null && levelConfigurator.mapType != PDKLevelConfigurator.mapTypes.None)
        {
            // Tell the user these are tileset fields
            editorUtilities.Field("Tilesets");
            // For each tilset in this map
            for (int currentTileset = 0; currentTileset < levelConfigurator.pdkMap.tilesets.Length; currentTileset++)
            {
                // Display a field for this texture
                editorUtilities.Field(levelConfigurator.pdkMap.tilesets[currentTileset].name, ref levelConfigurator.pdkMap.tilesets[currentTileset].imageTexture);
            }
        }
        #endregion
        #region When Applicable Display the Object Fields
        // If tilesetTextures has ben instatiated, and the map type is not none
        if (levelConfigurator.pdkMap.objectsInMap != null && levelConfigurator.mapType != PDKLevelConfigurator.mapTypes.None)
        {
            // Tell the user these are object prefab fields
            editorUtilities.Field("Object Prefabs");
            // For each layer in this map
            foreach (string currentObjectType in levelConfigurator.pdkMap.objectsInMap.Keys.ToList())
            {
                // Create an temporary object that is a copy of the current object type
                UnityEngine.Object tempObject = levelConfigurator.pdkMap.objectsInMap[currentObjectType];

                // Display a field for this texture
                editorUtilities.Field(currentObjectType, ref tempObject);
                levelConfigurator.pdkMap.objectsInMap[currentObjectType] = tempObject;
            }
        }
        #endregion
    }
}
