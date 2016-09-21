using UnityEngine;
using System.Collections;
using UnityEditor;


[CustomEditor(typeof(PDKLevelConfigurator))]
public class PDKLevelConfiguratorEditor : Editor
{
    // This will overide unity's standard GUI
    public override void OnInspectorGUI()
    {
        // Define the target script to use
        PDKLevelConfigurator myTarget = (PDKLevelConfigurator)target;

        // Create the map type selection drop down
        myTarget.mapType = (PDKLevelConfigurator.mapTypes)EditorGUILayout.EnumPopup("Map Type", myTarget.mapType);

        #region Map TextAsset Change Check
        // Check to see if the GUI has been changed
        EditorGUI.BeginChangeCheck();
        if (myTarget.mapType == PDKLevelConfigurator.mapTypes.Tiled) // If the user has selected a Tiled map type
        {
            // Display a slot for the map's TextAsset
            myTarget.mapSettings.mapTextAsset = (TextAsset)EditorGUILayout.ObjectField("Tile Map", myTarget.mapSettings.mapTextAsset, typeof(TextAsset), false);
        }
        // Stop checking to see if the GUI has been changed
        if (EditorGUI.EndChangeCheck())
        {
            // Tell the level configurator to update it's settings
            myTarget.UpdateMapSettings();
        }
        #endregion

        #region Display Tileset Slots
        // If tilesetTextures has ben instatiated, and the map type is not none
        if (myTarget.mapSettings.tilesetTextures != null && myTarget.mapType != PDKLevelConfigurator.mapTypes.None)
        {
            // Create a slot to put the Texture2D from each Tileset in
            for (int currentTileset = 0; currentTileset < myTarget.mapSettings.tilesetTextures.Length; currentTileset++)
            {
                myTarget.mapSettings.tilesetTextures[currentTileset] =
                    (Texture2D)EditorGUILayout.ObjectField(
                        myTarget.mapSettings.tikMap.tilesets[currentTileset].name,
                        myTarget.mapSettings.tilesetTextures[currentTileset], typeof(Texture2D), false);
            }
        }
        #endregion
    }
}
