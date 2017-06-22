using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class PDKMap
{
    #region Whatever these are called
    [System.Serializable]
    public class PDKCustomProperties : PDKSerializableDictionay<string, string> { }
    [System.Serializable]
    public class PDKIDHashet : PDKSerializableHashSet<int> { }
    [System.Serializable]
    public class PDKColliderTypes : PDKSerializableHashSet<PDKColliderType> { }
    [System.Serializable]
    public class PDKColliderObjectsForTileIDs : PDKSerializableDictionay<int, GameObject> { }
    [System.Serializable]
    public class PDKObjectTypes : PDKSerializableSortedDictionay<string, UnityEngine.Object> { }
    #endregion

    public int width;
    public int height;
    public int tileWidth;
    public int tileHeight;
    public PDKLayer[] layers;
    public PDKTileset[] tilesets;
    #region Collider Type Variables
    // Used durring run time
    public PDKIDHashet tilesWithColliders;
    public PDKColliderObjectsForTileIDs tileColliderObjects;
    // Used in editor
    public PDKColliderTypes colliderTypes;
    #endregion
    public List<PDKLayerGroup> layerGroups;
    public PDKCustomProperties properties;
    // TODO: FILL THIS IN LATER
    public PDKObjectTypes objectsInMap;
    // This will store a blank tile
    public Color[] blankTile;
    // TODO: FILL THIS IN LATER



    //The InitializeMap function initializes all the layers and tilests
    public void InitializeMap(bool shouldGroupLayers)
    {
        // This is used to determine which layer group is being looked at
        int currentLayerGroupNumber = -1;
        
        #region Initialize Tilesets
        // Tell each of this map's layers to initialize
        foreach (PDKTileset CurrentTileset in tilesets)
        {
            CurrentTileset.InitializeTileset();
        }
        #endregion
        #region Initialize Layers
        // Tell each of this map's layers to initialize
        foreach (PDKLayer layerToInitialize in layers)
        {
            layerToInitialize.InitializeLayer(objectsInMap);
        }
        #endregion
        #region Create Layer Groups
        // Instatiate a list for layerGroups
        layerGroups = new List<PDKLayerGroup>();
        // Go through each layer in this map
        for (int thisLayerNumber = layers.Length - 1; thisLayerNumber >= 0; thisLayerNumber--)
        {
            // If this layer is visible
            if (layers[thisLayerNumber].visible)
            {
                // If this layer is the same type as the current layer group
                if (shouldGroupLayers && layerGroups.Count > 0 && layerGroups[currentLayerGroupNumber].groupType == layers[thisLayerNumber].type)
                {
                    // Add this layer to the current layer group
                    layerGroups[currentLayerGroupNumber].layerNumbers.Add(thisLayerNumber);
                }
                else
                {
                    // Increase the current layer group number
                    currentLayerGroupNumber++;
                    // Create A new LayerGroup dictionary for this type of layer
                    layerGroups.Add(new PDKLayerGroup(layers[thisLayerNumber].type));
                    // Calculate the Z position of this layer group
                    layerGroups[currentLayerGroupNumber].zPosition = currentLayerGroupNumber;
                    // Add this layer to the newly created layer group
                    layerGroups[currentLayerGroupNumber].layerNumbers.Add(thisLayerNumber);
                }
            }
        }
        #endregion
        #region Create a blank tile
        // Initialize blankTile at the appropriate size
        blankTile = new Color[tileWidth * tileHeight];
        // For each pixel in the blank tile
        for (int thisPixelIndex = 0; thisPixelIndex < blankTile.Length; thisPixelIndex++)
        {
            // Set this pixel to transparent
            blankTile[thisPixelIndex] = Color.clear;
        }
        #endregion
    }


    /* When this is called this function finds all tile IDs in a given rectangle, and then 
    returns a list of each ID and at what positions in the given rectangle that tile ID appears at */
    public Dictionary<int, List<int>> GetTilesByRasterList(int layerToGetTilesFrom, List<int> rasterPositions)
    {
        // This dictionary will contain all positions of a tile ID in the given rectangle
        Dictionary<int, List<int>> disoveredTilePostitions = new Dictionary<int, List<int>>();

        // Go through each position in the given list
        foreach (int thisTilePosition in rasterPositions)
        {
            // This will be used to determine if the position exists and then store the list this tile position should be placed in
            List<int> matchingID;

            // If the ID at the position being checked has been disovered add this position to that ID's list of positions
            if (disoveredTilePostitions.TryGetValue(layers[layerToGetTilesFrom].tileMap[thisTilePosition], out matchingID))
            {
                matchingID.Add(thisTilePosition);
            }
            else
            {
                // Create a new list for the tile ID at the position being checked
                matchingID = new List<int>();
                // Add this position to this tile ID's new list
                matchingID.Add(thisTilePosition);
                // Add this tile ID's new list to the dictionary of discovered tile IDs
                disoveredTilePostitions[layers[layerToGetTilesFrom].tileMap[thisTilePosition]] = matchingID;

            }
        }
        // Return the lsit of all discovered tiles and their positions
        return disoveredTilePostitions;
    }


    /* When this is called this function finds all tile IDs in a given rectangle, and then 
    returns a list of each ID and at what positions in the given rectangle that tile ID appears at */
    public Dictionary<int, List<Vector2>> GetTilesByCoordinateList(int layerToGetTilesFrom, List<Vector2> coordinatePositions)
    {
        // This dictionary will contain all positions of a tile ID in the given rectangle
        Dictionary<int, List<Vector2>> disoveredTilePostitions = new Dictionary<int, List<Vector2>>();
        // This stores the raster position of each element in the given list
        int rasterPosition;

        // Go through each tile in the given list and find all tiles 
        foreach (Vector2 tileCoordinates in coordinatePositions)
        {
            // Calculate this tile's raster position
            rasterPosition = (int)((tileCoordinates.y * width) + tileCoordinates.x);
            // This will be used to determine if it exits and then store the list this tile position should be placed in
            List<Vector2> matchingID;

            // If the ID at the position being checked has been disovered
            if (disoveredTilePostitions.TryGetValue(layers[layerToGetTilesFrom].tileMap[rasterPosition], out matchingID))
            {
                // Add this position to that ID's list of positions
                matchingID.Add(tileCoordinates);
            }
            else
            {
                // Create a new list for the tile ID at the position being checked
                matchingID = new List<Vector2>();
                // Add this position to this tile ID's new list
                matchingID.Add(tileCoordinates);
                // Add this tile ID's new list to the dictionary of discovered tile IDs
                disoveredTilePostitions[layers[layerToGetTilesFrom].tileMap[rasterPosition]] = matchingID;

            }
        }
        // Return the lsit of all discovered tiles and their positions
        return disoveredTilePostitions;
    }


    // When this is called this function finds the tileset with the texture for the requested tile and then returns an array of colors for that tile
    public Color[] GetTilePixels(int tileID)
    {
        // If this tile is not zero
        if (tileID > 0)
        {
            // Return the pixels for the requested tile
            return GetTileSetFromID(tileID).GetTilePixels(tileID);
        }
        else
        {
            // Return a blank tile
            return blankTile;
        }
    }


    // When this is called this function looks through all tilesets, finds the tileset that contains the given tile ID, and then returns that tileset
    public PDKTileset GetTileSetFromID(int tileID)
    {
        // Look through each tileset in this map
        foreach (PDKTileset tilesetToCheck in tilesets)
        {
            // If the given tile ID is in the tileset being checked
            if (tileID >= tilesetToCheck.firstGID && tileID < tilesetToCheck.firstGID + tilesetToCheck.tileCount)
            {
                // Return this tileset
                return tilesetToCheck;
            }
        }
        // In form the user that the tile ID was out of range
        Debug.Log("Tile ID for requested tileset was out of range: returned null");
        // The tile ID did not match any tileset in this map
        return null;
    }
}
