using UnityEngine;
using System.Collections;
using UnityEditor;


[CustomEditor(typeof(TIKLevelControl))]
public class TIKEditorLevelControl : Editor
{
    //
    public override void OnInspectorGUI()
    {
        //
        TIKLevelControl myTarget = (TIKLevelControl)target;
        //
        myTarget.tileMapTextAsset = (TextAsset)EditorGUILayout.ObjectField("Tile Map", myTarget.tileMapTextAsset, typeof(TextAsset), false);
        //
        EditorGUI.BeginChangeCheck();
        if (myTarget.UpdateGUI())
        {
            //
            for (int currentTileset = 0; currentTileset < myTarget.tilesetTextures.Length; currentTileset++)
            {
                //
                myTarget.tilesetTextures[currentTileset] =
                    (Texture2D)EditorGUILayout.ObjectField
                    (myTarget.levelMap.tilesets[currentTileset].name, myTarget.tilesetTextures[currentTileset], typeof(Texture2D), false);
            }
        }
        //
        EditorGUI.EndChangeCheck();
    }
}