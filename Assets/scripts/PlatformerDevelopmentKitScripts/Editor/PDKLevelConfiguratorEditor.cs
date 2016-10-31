using UnityEngine;
using System.Collections;
using UnityEditor;


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
        
        // Display a field for load distance
        editorUtilities.CreateField("Load Distance", ref levelConfigurator.loadDistance);
        // Display a field for buffer distance
        editorUtilities.CreateField("Buffer Distance", ref levelConfigurator.bufferDistance);
        // Create the map type selection drop down
        editorUtilities.CreateField("Map Type", ref levelConfigurator.mapType);
        // If the user has picked a tiled map
        if (levelConfigurator.mapType == PDKLevelConfigurator.mapTypes.Tiled) // If the user has selected a Tiled map type
        {
            // Create the appropriate fields for a tiled map
            editorUtilities.CreateFeildsForTiledMap(levelConfigurator);
        }
    }
}
