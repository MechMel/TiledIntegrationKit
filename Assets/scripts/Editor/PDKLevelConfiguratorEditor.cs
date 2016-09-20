using UnityEngine;
using System.Collections;
using UnityEditor;


[CustomEditor(typeof(PDKLevelConfigurator))]
public class PDKLevelConfiguratorEditor : Editor
{
    public bool shouldDisplayTilesets = false;

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
            // 
            if (myTarget.UpdateMapSettings())
            {
                // Display the slots for this map's tilesets
                shouldDisplayTilesets = true;
            }
        }
        #endregion

        // If tilesetTextures has ben instatiated
        if (myTarget.mapSettings.tilesetTextures != null)
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
    }
}
