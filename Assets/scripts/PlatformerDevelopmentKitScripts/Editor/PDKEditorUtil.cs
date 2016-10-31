using UnityEngine;
using System.Collections;
using UnityEditor;

public class PDKEditorUtil : Editor
{
    // Creates an interger field tied to a variable instance
    public void CreateField(string labelText, ref int fieldInstance)
    {
        fieldInstance = EditorGUILayout.IntField(labelText, fieldInstance);
    }

    // Creates a field for map type selection
    public void CreateField(string labelText, ref PDKLevelConfigurator.mapTypes mapTypesInstance)
    {
        mapTypesInstance = (PDKLevelConfigurator.mapTypes)EditorGUILayout.EnumPopup(labelText, mapTypesInstance);
    }

    // Creates all nessecary fields for a tiled map
    public void CreateFeildsForTiledMap(PDKLevelConfigurator configuratorInstance)
    {
        // Check to see if the tile map GUI has been changed
        EditorGUI.BeginChangeCheck();
        // Create a field for a tile map text asset
        configuratorInstance.mapSettings.mapTextAsset = (TextAsset)EditorGUILayout.ObjectField("Tile Map", configuratorInstance.mapSettings.mapTextAsset, typeof(TextAsset), false);
        // If the tile map GUI has been changed
        if (EditorGUI.EndChangeCheck())
        {
            // Tell the level configurator it's text asset has been changed
            configuratorInstance.TextAssetChanged();
        }
        // If tilesetTextures has ben instatiated, and the map type is not none
        if (configuratorInstance.mapSettings.tilesetTextures != null && configuratorInstance.mapType != PDKLevelConfigurator.mapTypes.None)
        {
            // Create a slot to put the Texture2D from each Tileset in
            for (int currentTileset = 0; currentTileset < configuratorInstance.mapSettings.tilesetTextures.Length; currentTileset++)
            {
                configuratorInstance.mapSettings.tilesetTextures[currentTileset] =
                    (Texture2D)EditorGUILayout.ObjectField(
                        configuratorInstance.mapSettings.tikMap.tilesets[currentTileset].name,
                        configuratorInstance.mapSettings.tilesetTextures[currentTileset], typeof(Texture2D), false);
            }
        }
    }
}
