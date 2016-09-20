using UnityEngine;
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
    #endregion


    //The InitializeMap function initializes all the layers and tilests
    public void InitializeMap(Texture2D[] tilesetTextures)
    {
        // Tell each of this map's layers to initialize
        foreach(TIKLayer layer in layers)
        {
            layer.InitializeLayer();
        }
        // Tell each of this map's tilesets to initialize
        for (int currentTileset = 0; currentTileset < tilesets.Length; currentTileset++)
        {
            tilesets[currentTileset].InitializeTileset(tilesetTextures[currentTileset]);
        }
    }
    //The GetDimension function takes a string, either "width", or "height", and returns the int value of either 

    //the width or height, according to the string
    public int GetDimension(string dimension)
    {
        if (dimension == "width")
        {
            return width;
        }
        else if (dimension == "height")
        {
            return height;
        }
        else
        {
            return 0;
        }
    }

    //
    public Texture2D[] GetAllTilesTexturesFromLayer(int layerToGetTilesFrom)
    {
        // If the layer to get tiles form is a tile layer
        if (layers[layerToGetTilesFrom].layerType == TIKLayer.layerTypes.Tile)
        {
            // Create an array of textures to put all the textures formt the requested layer into
            Texture2D[] allTileTextures = new Texture2D[layers[layerToGetTilesFrom].data.Length];
            // For each tile in the requested layer
            for (int tileNumber = 0; tileNumber < layers[layerToGetTilesFrom].data.Length; tileNumber++)
            {
                // Look through each of this map's tilesets
                foreach (TIKTileset tilesetCurrentlyBeingChecked in tilesets)
                {
                    // If this tile is in the tilest currently being checked
                    if (layers[layerToGetTilesFrom].data[tileNumber] >= tilesetCurrentlyBeingChecked.firstgid - 1 && layers[layerToGetTilesFrom].data[tileNumber] < tilesetCurrentlyBeingChecked.firstgid + tilesetCurrentlyBeingChecked.tilecount)
                    {
                        // Add the texture for this tile to the array of tile textures
                        allTileTextures[tileNumber] = tilesetCurrentlyBeingChecked.GetTileTexture(layers[layerToGetTilesFrom].data[tileNumber]);
                        // There is no reason to continue checking tilesets
                        break;
                    }
                }
            }
            // Return the lest of all tile textures in the requested layer
            return allTileTextures;
        }
        else
        {
            // Tell the user what went wrong
            Debug.Log("Requested layer to get all tiles from is not a tile layer: Returned null");
            // Request is invalid, because the requested layer is not a tile layer
            return null;
        }
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
