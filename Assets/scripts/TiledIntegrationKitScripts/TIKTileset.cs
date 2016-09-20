using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

[Serializable]
public class TIKTileset
{
    #region Tileset Attributes
    public int firstgid;
    public string name;
    public int tilewidth;
    public int tileheight;
    public int tilecount;
    public int columns;
    public int margin;
    public int spacing;
    #endregion

    #region Image Attributes
    public string image;
    public Texture2D imageTexture;
    public int imagewidth;
    public int imageheight;
    #endregion

    #region Custom Properties
    public Dictionary<string, string> customTilesetProperties;
    public Dictionary<string, string>[] customTiles;
    #endregion


    // This is used to remove all letters off of an extension
    private char[] allLetters = new char[] { 'a', 'b', 'b', 'c', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

    // When this is called this tileset instatiates the nessecary variables
    public void InitializeTileset(Texture2D tilesetTexture)
    {
        // Instatiate this tilest's texture
        imageTexture = tilesetTexture;
    }

    // When this is called it creates a Tileset from a given XmlNode for a tileset
    /*public TIKTileset(XmlNode tilesetXmlNode)
    {

        // Add all of the tileset attributes to the new tileset
        firstgid = Int32.Parse(tilesetXmlNode.Attributes["firstgid"].Value);
        name = tilesetXmlNode.Attributes["name"].Value;
        tilewidth = Int32.Parse(tilesetXmlNode.Attributes["tilewidth"].Value);
        tileheight = Int32.Parse(tilesetXmlNode.Attributes["tileheight"].Value);
        tilecount = Int32.Parse(tilesetXmlNode.Attributes["tilecount"].Value);
        columns = Int32.Parse(tilesetXmlNode.Attributes["columns"].Value);

        // Add all of the Image attributes to the new tileset
        XmlNode imageNode = tilesetXmlNode.SelectSingleNode("//*[local-name() = 'image'][1]");
        image = imageNode.Attributes["source"].Value;
        imageTexture = (Texture2D)Resources.Load<Texture2D>(image.TrimStart(new char[] { '.' }).TrimStart(new char[] { '/' }).TrimEnd(allLetters).TrimEnd(new char[] { '.' }));
        FormatTexture2D(imageTexture); // Format this tileset's texture
        imagewidth = Int32.Parse(imageNode.Attributes["width"].Value);
        imageheight = Int32.Parse(imageNode.Attributes["height"].Value);

        // Add all of the custom properties to the new tileset
        // Declare this tileset's dictionary for custom properties
        customTilesetProperties = new Dictionary<string, string>();
        // Put all this tileset's custom properties into a tileset
        XmlNodeList customTilestProperties = tilesetXmlNode.SelectNodes("/tileset/Properties/Property");
        // For each of this tilest's custom properties
        foreach(XmlNode currentCustomProperty in customTilestProperties)
        {
            // Add the current custom property to this tileset's dictionary for custom properties
            customTilesetProperties.Add(currentCustomProperty.Attributes["name"].Value, currentCustomProperty.Attributes["value"].Value);
        }
        // Declare this tileset's dictionary for custom tiles and determine the number of custom tiles in this tileset
        customTiles = new Dictionary<string, string>[tilecount + firstgid - 1];
        // Put all this tileset's custom tiles into a node list
        XmlNodeList customTileXmlNodes = tilesetXmlNode.SelectNodes("/tileset/tile");
        // For each custom tile in this tileset
        foreach(XmlNode currentCustomTile in customTileXmlNodes)
        {
            // Create a dictionary to temporarily store a single custom property
            Dictionary<string, string> currentCustomTileProperties = new Dictionary<string, string>();
            // Put all of this tile's custom properties into a node list
            XmlNodeList customTilePropertyNodes = currentCustomTile.SelectNodes("/tile/properties/property");
            // For each of the custom properties on this tile
            foreach (XmlNode currentCustomProperty in customTilePropertyNodes)
            {
                // Put this custom properties into the temporary dictionary
                currentCustomTileProperties.Add(currentCustomProperty.Attributes["name"].Value, currentCustomProperty.Attributes["value"].Value);
            }
            // Put all of this tile's custom properties into the dictionary of custom tiles 
            customTiles[Int32.Parse(currentCustomTile.Attributes["name"].Value)] = currentCustomTileProperties;
        }
    }*/



    // When this is called Default values for a tilset are set and applied to the given texture2D
    public void FormatTexture2D(Texture2D textureToFormat)
    {
        // Format the given Texture2D
        textureToFormat = Sprite.Create(textureToFormat, new Rect(0, 0, textureToFormat.width, textureToFormat.height), new Vector2(.5f, .5f), 1).texture;
    }



    // When this is called it recturns a sprite of a tile from a given tile ID
    public Texture2D GetTileTexture(int tileID)
    {
        // Create a new sprite for this tile from the correct portion of this tileset
        Sprite newTileSprite = Sprite.Create(imageTexture, new Rect(tilewidth * ((tileID - firstgid + 1) % (imagewidth / tilewidth)), imageheight - (tilewidth * ((tileID - firstgid + 1) / (imagewidth / tilewidth))), tilewidth, -tileheight), new Vector2(0.5f, 0.5f), tilewidth);
        // Return the newly created sprite for this tile
        return newTileSprite.texture;
    }
}
