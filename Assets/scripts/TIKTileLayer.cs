using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class PDKTileLayer : TIKLayer
{
    #region Layer Attributes
    public int width;
    public int height;
    public string encoding;
    #endregion

    #region Custom Properties
    public Dictionary<string, string> customTilesetProperties;
    public Dictionary<string, string>[] customTiles;
    #endregion

    private int[,] tileMap;


    /*
    // When this is called it creates a TileLayer from a given XmlNode for a tile layer
    public TIKTileLayer(XmlNode tileLayerXmlNode) : base (tileLayerXmlNode)
    {
        // Add all of the tile layer's attributes to this TileLayer
        width = Int32.Parse(tileLayerXmlNode.Attributes["width"].Value);
        height = Int32.Parse(tileLayerXmlNode.Attributes["height"].Value);
        XmlNode dataNode = tileLayerXmlNode.SelectSingleNode("//*[local-name() = 'data'][1]");
        encoding = dataNode.Attributes["encoding"].Value;

        // Put all the tiles from this layer in an array
        string[] tileMapData = tileLayerXmlNode.LastChild.InnerText.Split(',');
        // Determine the size of the new tile map
        tileMap = new int[width, height];

        // For each tile in this layer
        for (int currentTile = 0; currentTile < tileMapData.Length; currentTile++)
        {
            // Put this tile in the tileMap array
            tileMap[(currentTile % width), (currentTile / width)] = Int32.Parse(tileMapData[currentTile]);
        }
    }


    // When this is called it returns a tile from a given x and y coordinate
    public int getTileIDFromCorrdinate(int x, int y)
    {
        // Return the correct tile
        return tileMap[x, y];
    }
    */
}
