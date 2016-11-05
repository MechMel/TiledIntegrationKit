﻿using UnityEngine;
using System;
using System.Xml;
using System.Collections.Generic;

[Serializable]
public class TIKMap
{
    #region Map Attributes
    public float version;
    public string orientation;
    public string renderOrder;
    public int width;
    public int height;
    public int tilewidth;
    public int tileheight;
    public int nextobjectid;
    public TIKLayer[] layers;
    public TIKTileset[] tilesets;
    public List<PDKLayerGroup> layerGroups;
    #endregion
    // This will store a blank tile
    Color[] blankTile;

    //The InitializeMap function initializes all the layers and tilests
    public void InitializeMap(Texture2D[] tilesetTextures)
    {
        // This is used to determine which layer group is being looked at
        int currentLayerGroupNumber = -1;

        #region Initialize Layers
        // Tell each of this map's layers to initialize
        foreach (TIKLayer layerToInitialize in layers)
        {
            layerToInitialize.InitializeLayer();
        }
        #endregion
        #region Initialize Tilesets
        // Tell each of this map's tilesets to initialize
        for (int numberOfTilesetToInitialze = 0; numberOfTilesetToInitialze < tilesets.Length; numberOfTilesetToInitialze++)
        {
            tilesets[numberOfTilesetToInitialze].InitializeTileset(tilesetTextures[numberOfTilesetToInitialze]);
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
                if (layerGroups.Count > 0 && layerGroups[currentLayerGroupNumber].groupType == layers[thisLayerNumber].layerType)
                {
                    // Add this layer to the current layer group
                    layerGroups[currentLayerGroupNumber].layerNumbers.Add(thisLayerNumber);
                }
                else
                {
                    // Increase the current layer group number
                    currentLayerGroupNumber++;
                    // Create A new LayerGroup dictionary for this type of layer
                    layerGroups.Add(new PDKLayerGroup(layers[thisLayerNumber].layerType));
                    // Add this layer to the newly created layer group
                    layerGroups[currentLayerGroupNumber].layerNumbers.Add(thisLayerNumber);
                }
            }
        }
        #endregion
        #region Create a blank tile
        // Initialize blankTile at the appropriate size
        blankTile = new Color[tilewidth * tileheight];
        // For each pixel in the blank tile
        for (int thisPixelIndex = 0; thisPixelIndex < blankTile.Length; thisPixelIndex++)
        {
            // Set this pixel to transparent
            blankTile[thisPixelIndex] = Color.clear;
        }
        #endregion
    }

    /* When this is called this function finds all tile IDs in a given rectangle, and then 
    returns a list of each ID and at what positions in the given rectangle that tile ID appears*/
    public Dictionary<int, List<int>> GetAllTilePositionsFromLayerInList(int layerToGetTilesFrom, List<int> tilePositions)
    {
        // This dictionary will contain all positions of a tile ID in the given rectangle
        Dictionary<int, List<int>> disoveredTilePostitions = new Dictionary<int, List<int>>();

        // Go through each tile in the given list
        foreach (int thisTilePosition in tilePositions)
        {
            // If the tile ID to check is not 0(a blank tile)
            if (thisTilePosition >= 0)
            {
                // This will be used to determine if it exits and then store the list this tile position should be placed in
                List<int> matchingID;

                // If the ID at the position being checked has been disovered
                if (disoveredTilePostitions.TryGetValue(layers[layerToGetTilesFrom].data[thisTilePosition], out matchingID))
                {
                    // Add this position to that ID's list of positions
                    matchingID.Add(thisTilePosition);
                }
                else
                {
                    // Create a new list for the tile ID at the position being checked
                    matchingID = new List<int>();
                    // Add this position to this tile ID's new list
                    matchingID.Add(thisTilePosition);
                    // Add this tile ID's new list to the dictionary of discovered tile IDs
                    disoveredTilePostitions[layers[layerToGetTilesFrom].data[thisTilePosition]] = matchingID;
                }
            }
        }
        // Return the lsit of all discovered tiles and their positions
        return disoveredTilePostitions;
    }

    /* When this is called this function finds all tile IDs in a given rectangle, and then 
    returns a list of each ID and at what positions in the given rectangle that tile ID appears*/
    public Dictionary<int, List<int>> GetAllTilePositionsFromLayerInRectangle(int layerToGetTilesFrom, Rect rectangleToGeTileFrom)
    {
        // This is the ID at a position in the given rectangle
        int[] tileIDsInRectangle = new int[(int)rectangleToGeTileFrom.width * (int)rectangleToGeTileFrom.height];
        // This dictionary will contain all positions of a tile ID in the given rectangle
        Dictionary<int, List<int>> disoveredTilePostitions = new Dictionary<int, List<int>>();
        // Find where in the map the rectangle should start
        int rectStartPosition = (width * (int)(rectangleToGeTileFrom.y)) + (int)rectangleToGeTileFrom.x;

        // Go through each row of the rectangle
        for (int rectY = 0; rectY < rectangleToGeTileFrom.height; rectY++)
        {
            // Go through each tile in this row of the rectangle
            for (int rectX = 0; rectX < rectangleToGeTileFrom.width; rectX++)
            {
                if (rectX + rectangleToGeTileFrom.x >= 0 && rectX + rectangleToGeTileFrom.x < width && rectY + rectangleToGeTileFrom.y >= 0 && rectY + rectangleToGeTileFrom.y < height) // If the tile in the given rectangle is inside this map
                {
                    // Find the position in the map of this tile ID from the rectangle
                    int positionOfTileIDToGet = (rectY * width) + rectX + rectStartPosition;
                    // Add the this tile ID from the map to the array of tile IDs in the given rectangle
                    tileIDsInRectangle[(rectY * (int)rectangleToGeTileFrom.width) + rectX] = layers[layerToGetTilesFrom].data[positionOfTileIDToGet];
                }
                else // If the tile in the given rectangle is outside this map
                {
                    // There is no tile at this position
                    tileIDsInRectangle[(rectY * (int)rectangleToGeTileFrom.width) + rectX] = 0;
                }
            }
        }

        // Go through each position in the given rectangle
        for (int positionOfIDBeingChecked = 0; positionOfIDBeingChecked < tileIDsInRectangle.Length; positionOfIDBeingChecked++)
        {
            // If the tile ID to check is not 0(a blank tile)
            if (tileIDsInRectangle[positionOfIDBeingChecked] != 0)
            {
                // This will be used to determine if it exits and then store the list this tile position should be placed in
                List<int> matchingID;
                // If the ID at the position being checked has been disovered
                if (disoveredTilePostitions.TryGetValue(tileIDsInRectangle[positionOfIDBeingChecked], out matchingID))
                {
                    // Add this position to that ID's list of positions
                    matchingID.Add(positionOfIDBeingChecked);
                }
                else
                {
                    // Create a new list for the tile ID at the position being checked
                    matchingID = new List<int>();
                    // Add this position to this tile ID's new list
                    matchingID.Add(positionOfIDBeingChecked);
                    // Add this tile ID's new list to the dictionary of discovered tile IDs
                    disoveredTilePostitions[tileIDsInRectangle[positionOfIDBeingChecked]] = matchingID;
                }
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
    public TIKTileset GetTileSetFromID(int tileID)
    {
        // Look through each tileset in this map
        foreach (TIKTileset tilesetToCheck in tilesets)
        {
            // If the given tile ID is in the tileset being checked
            if (tileID >= tilesetToCheck.firstgid && tileID < tilesetToCheck.firstgid + tilesetToCheck.tilecount)
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


    /*
    // When this is called it creates a new PDKmap from a given XmlDocument for a tiled map
    public TIKMap(XmlDocument mapXML)
    {
        // Create a temporary node list to put all tilesets in
        XmlNodeList allTilesetsXML = mapXML.GetElementsByTagName("tileset");
        // Determine the number of tilesets in this map, and define the allTilesets array
        allTilesets = new PDKTileset[allTilesetsXML.Count];
        // For each tileset in this map
        for (int currentTileset = 0; currentTileset < allTilesetsXML.Count; currentTileset++)
        {
            // Create a new tileset and add it to the allTilesets array
            allTilesets[currentTileset] = new TIKTileset(allTilesetsXML[currentTileset]);
        }
        // Create a temporary node list to put all layers in
        XmlNodeList allLayersXML = mapXML.GetElementsByTagName("layer");
        // Determine the number of layers in this map, and define the allLayers array
        allLayers = new PDKLayer[allLayersXML.Count];
        // For each layer in this map
        for (int currentLayer = 0; currentLayer < allLayersXML.Count; currentLayer++)
        {
            // Create a new tilLayer and add it to the allLayers array
            allLayers[currentLayer] = new PDKTileLayer(allLayersXML[currentLayer]);
        }
    }


    // When this is called it creates a new PDKmap from a given XmlDocument for a tiled map
    public PDKMap(TextAsset mapJson)
    {
        //
        orientation = mapJson.
        // Create a temporary node list to put all tilesets in
        XmlNodeList allTilesetsXML = mapJson.GetElementsByTagName("tileset");
        // Determine the number of tilesets in this map, and define the allTilesets array
        allTilesets = new PDKTileset[allTilesetsXML.Count];
        // For each tileset in this map
        for (int currentTileset = 0; currentTileset < allTilesetsXML.Count; currentTileset++)
        {
            // Create a new tileset and add it to the allTilesets array
            allTilesets[currentTileset] = new PDKTileset(allTilesetsXML[currentTileset]);
        }
        // Create a temporary node list to put all layers in
        XmlNodeList allLayersXML = mapJson.GetElementsByTagName("layer");
        // Determine the number of layers in this map, and define the allLayers array
        allLayers = new PDKLayer[allLayersXML.Count];
        // For each layer in this map
        for (int currentLayer = 0; currentLayer < allLayersXML.Count; currentLayer++)
        {
            // Create a new tilLayer and add it to the allLayers array
            allLayers[currentLayer] = new PDKTileLayer(allLayersXML[currentLayer]);
        }
    }
    */
}
