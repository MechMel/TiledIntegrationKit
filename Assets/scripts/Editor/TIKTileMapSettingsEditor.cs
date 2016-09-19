using UnityEngine;
using System.Collections;
using UnityEditor;


[CustomEditor(typeof(TIKTileMapSettings))]
public class TIKTileMapSettingsEditor : Editor
{
    // This will overide unity's standard GUI
    public override void OnInspectorGUI()
    {
        // Define the target script to use
        TIKTileMapSettings myTarget = (TIKTileMapSettings)target;
        // Create a slot to put the TextAsset for this level's map in
        myTarget.tileMapTextAsset = (TextAsset)EditorGUILayout.ObjectField("Tile Map", myTarget.tileMapTextAsset, typeof(TextAsset), false);
        // Check to see if the GUI has been changed
        EditorGUI.BeginChangeCheck();
        // When the GUI is changed
        if (myTarget.UpdateGUI())
        {
            // Create a slot to put the Texture2D from each Tileset in
            for (int currentTileset = 0; currentTileset < myTarget.tilesetTextures.Length; currentTileset++)
            {
                myTarget.tilesetTextures[currentTileset] =
                    (Texture2D)EditorGUILayout.ObjectField
                    (myTarget.levelMap.tilesets[currentTileset].name, myTarget.tilesetTextures[currentTileset], typeof(Texture2D), false);
            }
        }
        // Stop checking to see if the GUI has been changed
        EditorGUI.EndChangeCheck();
    }
}
