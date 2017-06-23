using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class PDKTileset
{
    public string name;
    public int tileWidth;
    public int tileHeight;
    public int tileCount;
    public int columns;
    public int firstGID;
    public int margin;
    public int spacing;
    public Texture2D imageTexture;
    public PDKTileColorArray[] tileColorArrays;
    public int imageWidth;
    public int imageHeight;
    public PDKMap.PDKCustomProperties properties;
    public PDKMap.PDKCustomProperties[] tileProperties;
    


    // When this is called this tileset instatiates the nessecary variables
    public void InitializeTileset()
    {
        // Compile this Tileset
        compileTileset();
    }
    

    // TODO: Fill this in later
    public void compileTileset()
    {
        // Create an array of color arrays to hold all the tiles
        PDKTileColorArray[] thisTilesetColors = new PDKTileColorArray[tileCount + 1];

        // Go through each tile in this tileset
        for (int thisTileIndex = 0; thisTileIndex < thisTilesetColors.Length; thisTileIndex++)
        {
            // Create a new tile color array for this tile
            thisTilesetColors[thisTileIndex] = new PDKTileColorArray();
            // Get this tile's colors
            Color[] thisTileColors = imageTexture.GetPixels(
                x: tileWidth * ((thisTileIndex + 1 - firstGID) % columns),
                y: tileHeight * ((tileCount - thisTileIndex + firstGID - 2) / columns),
                blockWidth: tileWidth,
                blockHeight: tileHeight);

            // Look through each previous tile in this set
            for (int previousTileIndex = 0; previousTileIndex < thisTileIndex; previousTileIndex++)
            {
                // If a tile that is exactly the same as this tile already exists
                if (thisTilesetColors[previousTileIndex].colors == thisTileColors)
                {
                    // Point to the previous tile in memory
                    thisTileColors = thisTilesetColors[previousTileIndex].colors;
                    // There is no need to continue this search
                    break;
                }
            }
            // Add this tile's colors to this tilest's aray of tile colors
            thisTilesetColors[thisTileIndex].colors = thisTileColors;
        }
        // Add this compressed array of colors to the tikMap
        tileColorArrays = thisTilesetColors;
    }



    // When this is called Default values for a tilset are set and applied to the given texture2D
    public void FormatTexture2D(Texture2D textureToFormat)
    {
        // Format the given Texture2D
        textureToFormat = Sprite.Create(textureToFormat, new Rect(0, 0, textureToFormat.width, textureToFormat.height), new Vector2(.5f, .5f), 1).texture;
    }



    // When this is called it creates and returns an array of colors for a tile from a given tile ID
    public Color[] GetTilePixels(int tileID)
    {
        // Return the appropriate tile color array
        return tileColorArrays[tileID - 1].colors;
    }
}
