﻿using UnityEngine;
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

        // Display a field for layer grouping
        editorUtilities.Field("Should Group Like Layers", ref levelConfigurator.shouldGroupLayers);

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

        // Only show map details if a map and text asset exist
        if (levelConfigurator.pdkMap != null && levelConfigurator.mapTextAsset != null)
        {
            #region When Applicable Display the Previsualization Button
            // If the level is not previsualized display the previsualize button
            if (!levelConfigurator.levelIsPreVisualized)
            {
                // When the previsualization button is pressed render the entire level
                if (editorUtilities.Button("Pre-Visualize Level"))
                {
                    // Render the entire level
                    levelConfigurator.PrevisualizeLevel();
                }
            }
            // If the level is previsualized display the remove previsualization button
            else
            {
                // When the remove previsualization button is pressed render only the top left tile in the level
                if (editorUtilities.Button("Remove Pre-Visualized Level"))
                {
                    // Render only the top left tile in the level
                    levelConfigurator.RemoveLevelPrevisualization();
                }
            }
            #endregion

            #region When Applicable Display the Tileset Fields
            // If tilesetTextures has ben instatiated, and the map type is not none
            if (levelConfigurator.pdkMap.tilesets != null && levelConfigurator.mapType != PDKLevelConfigurator.mapTypes.None)
            {
                // Tell the user these are tileset fields
                editorUtilities.Field("Tilesets:");
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
                editorUtilities.Field("Object Prefabs:");
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

            #region When Applicable Display the Collider Fields
            // If tilesetTextures has ben instatiated, and the map type is not none
            if (levelConfigurator.pdkMap.colliderTypes != null && levelConfigurator.mapType != PDKLevelConfigurator.mapTypes.None)
            {
                // Tell the user these are object prefab fields
                editorUtilities.Field("Collider Prefabs:");
                // For each collider type in this map
                foreach (PDKColliderType currentColliderType in levelConfigurator.pdkMap.colliderTypes)
                {
                    // Display a field for this texture
                    editorUtilities.Field(currentColliderType.name, ref currentColliderType.gameObjectForThisCollider);
                }
            }
            #endregion
        }
    }
}
