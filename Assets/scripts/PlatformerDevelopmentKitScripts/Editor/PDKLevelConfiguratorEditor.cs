using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(PDKLevelConfigurator))]
public class PDKLevelConfiguratorEditor : Editor
{
    //
    private PDKEditorUtil editorUtilities = new PDKEditorUtil();

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
            if (editorUtilities.Field("Tile Map", ref levelConfigurator.mapSettings.mapTextAsset))// If the text asset is changed
            {
                // Tell the level configurator the text asset has been changed
                levelConfigurator.TextAssetChanged();
            }
        }
        #endregion
        #region When Applicable Display the Tileset Fields
        // If tilesetTextures has ben instatiated, and the map type is not none
        if (levelConfigurator.mapSettings.tilesetTextures != null && levelConfigurator.mapType != PDKLevelConfigurator.mapTypes.None)
        {
            // For each tilset in this map
            for (int currentTileset = 0; currentTileset < levelConfigurator.mapSettings.tilesetTextures.Length; currentTileset++)
            {
                // Display a field for this texture
                editorUtilities.Field(levelConfigurator.mapSettings.tikMap.tilesets[currentTileset].name, ref levelConfigurator.mapSettings.tilesetTextures[currentTileset]);
            }
        }
        #endregion
        #region When Applicable Display the Solid and Oneway Tile Fields
        editorUtilities.Field("Solid Tiles", "Solid Tile", ref levelConfigurator.solidTiles);
        editorUtilities.Field("One-way Tiles", "On-eway Tile", ref levelConfigurator.oneWayTiles);
        #endregion 
    }
}
