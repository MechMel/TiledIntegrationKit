using UnityEngine;
using System.Collections;
using UnityEditor;


[CustomEditor(typeof(TIKLevelControl))]
public class TIKEditorLevelControl : Editor
{
    // Create a new GUI for the inspector
    public override void OnInspectorGUI()
    {
        // Refrence the TIKLevelControl script
        TIKLevelControl myTarget = (TIKLevelControl)target;
        // Create a field to put the text asset for this level's tile map in
        myTarget.tileMapTextAsset = (TextAsset)EditorGUILayout.ObjectField("Tile Map", myTarget.tileMapTextAsset, typeof(TextAsset), false);
        // When the GUI is changed(by the user)
        EditorGUI.BeginChangeCheck();
        // Once the level controller updates it's variables
        if (myTarget.UpdateGUIVariables())
        {
            // Create a slot for each tilest in this map
            for (int currentTileset = 0; currentTileset < myTarget.tilesetTextures.Length; currentTileset++)
            {
                myTarget.tilesetTextures[currentTileset] =
                    (Texture2D)EditorGUILayout.ObjectField
                    (myTarget.levelMap.tilesets[currentTileset].name, myTarget.tilesetTextures[currentTileset], typeof(Texture2D), false);
            }
        }
        // Stop Checking if the GUI has been changed(by the user)
        EditorGUI.EndChangeCheck();
    }
}
