using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;

public class PDKJsonUtilities
{
    // This stores the characters to look for inorder to find custom properties
    string[] PROPERTIES_INDENTIFIER = { "\"properties\":\r\n" };



    // When this is called it creates and returns a new TIKMap from a given TextAsset from a tiled map
    public PDKMap CreatePDKMapFromTextAsset(TextAsset textAssetToCreateMapFrom)
    {
        // Stores the PDKTiledMap version of the given tiled map
        PDKTiledMap pdkTiledMap;

        // Create a PDKTiledMap from the given text asset
        pdkTiledMap = CreatePDKTiledMapFromTextAsset(textAssetToCreateMapFrom);
        // Return the newly created PDKMap
        return ConvertPDKTiledMapToPDKMap(pdkTiledMap);
    }
    

    // When this is called it creates and returns a new TIKMap from a given TextAsset from a tiled map
    public PDKTiledMap CreatePDKTiledMapFromTextAsset(TextAsset textAssetToCreateMapFrom)
    {
        // Convert the given text asset to a string
        string stringToCreateMapFrom = textAssetToCreateMapFrom.ToString();
        // Stores the map to return
        PDKTiledMap pdkTiledMap;

        // Correct custom properties
        CorrectCustomProperties(ref stringToCreateMapFrom);
        // Correct tile properties
        CorrectTileProperties(ref stringToCreateMapFrom);
        // Create a PDKTiledMap based on the map's Json file
        pdkTiledMap = JsonUtility.FromJson<PDKTiledMap>(stringToCreateMapFrom);
        // Return the newly created pdkTiledMap
        return pdkTiledMap;
    }


    // This Converts a PDKTiledMap to a PDKMap
    public PDKMap ConvertPDKTiledMapToPDKMap(PDKTiledMap pdkTiledMapToConvert)
    {
        // Stores the pdkMap that will be returned
        PDKMap pdkMap = new PDKMap();

        #region Copy Map Properties
        pdkMap.width = pdkTiledMapToConvert.width;
        pdkMap.height = pdkTiledMapToConvert.height;
        pdkMap.tileWidth = pdkTiledMapToConvert.tilewidth;
        pdkMap.tileHeight = pdkTiledMapToConvert.tileheight;
        foreach (PDKTiledCustomProperty currentProperty in pdkTiledMapToConvert.properties)
        {
            pdkMap.properties.Add(currentProperty.name, currentProperty.value);
        }
        #endregion
        #region Copy Layer Properties
        pdkMap.layers = new PDKLayer[pdkTiledMapToConvert.layers.Length];
        for (int currentLayerIndex = 0; currentLayerIndex < pdkTiledMapToConvert.layers.Length; currentLayerIndex++)
        {
            pdkMap.layers[currentLayerIndex] = new PDKLayer();
            pdkMap.layers[currentLayerIndex].name = pdkTiledMapToConvert.layers[currentLayerIndex].name;
            switch (pdkTiledMapToConvert.layers[currentLayerIndex].type)
            {
                case "tilelayer":
                    pdkMap.layers[currentLayerIndex].type = PDKLayer.layerTypes.Tile;
                    break;
                case "objectgroup":
                    pdkMap.layers[currentLayerIndex].type = PDKLayer.layerTypes.Object;
                    break;
                case "imagelayer":
                    pdkMap.layers[currentLayerIndex].type = PDKLayer.layerTypes.Image;
                    break;
            }
            pdkMap.layers[currentLayerIndex].name = pdkTiledMapToConvert.layers[currentLayerIndex].name;
            pdkMap.layers[currentLayerIndex].height = pdkTiledMapToConvert.layers[currentLayerIndex].height;
            pdkMap.layers[currentLayerIndex].width = pdkTiledMapToConvert.layers[currentLayerIndex].width;
            pdkMap.layers[currentLayerIndex].visible = pdkTiledMapToConvert.layers[currentLayerIndex].visible;
            pdkMap.layers[currentLayerIndex].opacity = pdkTiledMapToConvert.layers[currentLayerIndex].opacity;
            pdkMap.layers[currentLayerIndex].horizontalOffset = pdkTiledMapToConvert.layers[currentLayerIndex].x;
            pdkMap.layers[currentLayerIndex].verticalOffset = pdkTiledMapToConvert.layers[currentLayerIndex].y;
            pdkMap.layers[currentLayerIndex].properties = new Dictionary<string, string>();
            foreach (PDKTiledCustomProperty currentProperty in pdkTiledMapToConvert.layers[currentLayerIndex].properties)
            {
                pdkMap.layers[currentLayerIndex].properties.Add(currentProperty.name, currentProperty.value);
            }
            if (pdkTiledMapToConvert.layers[currentLayerIndex].data != null)
            {
                pdkMap.layers[currentLayerIndex].tileMap = new int[pdkTiledMapToConvert.layers[currentLayerIndex].data.Length];
                pdkTiledMapToConvert.layers[currentLayerIndex].data.CopyTo(pdkMap.layers[currentLayerIndex].tileMap, 0);
            }
            if (pdkTiledMapToConvert.layers[currentLayerIndex].objects != null)
            {
                pdkMap.layers[currentLayerIndex].objects = new PDKObject[pdkTiledMapToConvert.layers[currentLayerIndex].objects.Length];
                for (int currentObjectIndex = 0; currentObjectIndex < pdkTiledMapToConvert.layers[currentLayerIndex].objects.Length; currentObjectIndex++)
                {
                    pdkMap.layers[currentLayerIndex].objects[currentObjectIndex].name = pdkTiledMapToConvert.layers[currentLayerIndex].objects[currentObjectIndex].name;
                    pdkMap.layers[currentLayerIndex].objects[currentObjectIndex].type = pdkTiledMapToConvert.layers[currentLayerIndex].objects[currentObjectIndex].type;
                    pdkMap.layers[currentLayerIndex].objects[currentObjectIndex].id = pdkTiledMapToConvert.layers[currentLayerIndex].objects[currentObjectIndex].id;
                    pdkMap.layers[currentLayerIndex].objects[currentObjectIndex].gid = pdkTiledMapToConvert.layers[currentLayerIndex].objects[currentObjectIndex].gid;
                    pdkMap.layers[currentLayerIndex].objects[currentObjectIndex].x = pdkTiledMapToConvert.layers[currentLayerIndex].objects[currentObjectIndex].x;
                    pdkMap.layers[currentLayerIndex].objects[currentObjectIndex].y = pdkTiledMapToConvert.layers[currentLayerIndex].objects[currentObjectIndex].y;
                    pdkMap.layers[currentLayerIndex].objects[currentObjectIndex].width = pdkTiledMapToConvert.layers[currentLayerIndex].objects[currentObjectIndex].width;
                    pdkMap.layers[currentLayerIndex].objects[currentObjectIndex].height = pdkTiledMapToConvert.layers[currentLayerIndex].objects[currentObjectIndex].height;
                    pdkMap.layers[currentLayerIndex].objects[currentObjectIndex].rotation = pdkTiledMapToConvert.layers[currentLayerIndex].objects[currentObjectIndex].rotation;
                    pdkMap.layers[currentLayerIndex].objects[currentObjectIndex].visible = pdkTiledMapToConvert.layers[currentLayerIndex].objects[currentObjectIndex].visible;
                    pdkMap.layers[currentLayerIndex].objects[currentObjectIndex].properties = new Dictionary<string, string>();
                    foreach (PDKTiledCustomProperty currentProperty in pdkTiledMapToConvert.layers[currentLayerIndex].properties)
                    {
                        pdkMap.layers[currentLayerIndex].objects[currentObjectIndex].properties.Add(currentProperty.name, currentProperty.value);
                    }
                }
            }
        }
        #endregion
        #region Copy Tileset Properties
        pdkMap.tilesets = new PDKTileset[pdkTiledMapToConvert.tilesets.Length];
        for (int currentTilesetIndex = 0; currentTilesetIndex < pdkTiledMapToConvert.tilesets.Length; currentTilesetIndex++)
        {
            pdkMap.tilesets[currentTilesetIndex].name = pdkTiledMapToConvert.tilesets[currentTilesetIndex].name;
            pdkMap.tilesets[currentTilesetIndex].tileWidth = pdkTiledMapToConvert.tilesets[currentTilesetIndex].tilewidth;
            pdkMap.tilesets[currentTilesetIndex].tileHeight = pdkTiledMapToConvert.tilesets[currentTilesetIndex].tileheight;
            pdkMap.tilesets[currentTilesetIndex].tileCount = pdkTiledMapToConvert.tilesets[currentTilesetIndex].tilecount;
            pdkMap.tilesets[currentTilesetIndex].columns = pdkTiledMapToConvert.tilesets[currentTilesetIndex].columns;
            pdkMap.tilesets[currentTilesetIndex].firstGID = pdkTiledMapToConvert.tilesets[currentTilesetIndex].firstgid;
            pdkMap.tilesets[currentTilesetIndex].margin = pdkTiledMapToConvert.tilesets[currentTilesetIndex].margin;
            pdkMap.tilesets[currentTilesetIndex].spacing = pdkTiledMapToConvert.tilesets[currentTilesetIndex].spacing;
            pdkMap.tilesets[currentTilesetIndex].imageWidth = pdkTiledMapToConvert.tilesets[currentTilesetIndex].imagewidth;
            pdkMap.tilesets[currentTilesetIndex].imageHeight = pdkTiledMapToConvert.tilesets[currentTilesetIndex].imageheight;
            pdkMap.tilesets[currentTilesetIndex].properties = new Dictionary<string, string>();
            foreach (PDKTiledCustomProperty currentProperty in pdkTiledMapToConvert.tilesets[currentTilesetIndex].properties)
            {
                pdkMap.tilesets[currentTilesetIndex].properties.Add(currentProperty.name, currentProperty.value);
            }
            pdkMap.tilesets[currentTilesetIndex].tileProperties = new Dictionary<string, string>[pdkTiledMapToConvert.tilesets[currentTilesetIndex].tilecount];
            for (int currentTileIndex = 0; currentTileIndex < pdkTiledMapToConvert.tilesets[currentTilesetIndex].tilecount; currentTileIndex++)
            {
                pdkMap.tilesets[currentTilesetIndex].tileProperties[currentTileIndex] = new Dictionary<string, string>();
                foreach (PDKTiledTileProperties currentTileProperties in pdkTiledMapToConvert.tilesets[currentTilesetIndex].tileproperties)
                {
                    if (currentTileIndex == currentTileProperties.tileid)
                    {
                        foreach (PDKTiledCustomProperty currentCustomProperty in currentTileProperties.customproperties)
                        {
                            pdkMap.tilesets[currentTilesetIndex].tileProperties[currentTileIndex].Add(currentCustomProperty.name, currentCustomProperty.value);
                        }
                    }
                }
            }
        }
        #endregion
        return pdkMap;
    }


    // This modifies custom properties so that they can be properly parsed
    void CorrectCustomProperties(ref string stringToCorrect)
    {
        // This will store the parts of the string to correct
        string[] splitStringToCorrect;
        // Stores the position of the character that is being examined
        int currentCharPosition = 0;

        // Find and split at the beginning of tie custom properties
        splitStringToCorrect = stringToCorrect.Split(new string[] { "\"properties\":\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        // Go through each set of properties
        for (int indexOfCurrentString = 1; indexOfCurrentString < splitStringToCorrect.Length; indexOfCurrentString++)
        {
            // Used to determine if the current property is the first propery in this list
            bool isFirstProperty = true;
            // Used to store the index of the end of each quote
            int indexOfEndQuote;

            // Add the properties identifier back onto the corrected string
            splitStringToCorrect[0] += "\"properties\":[";
            // Start at the first unescaped quote or end curly bracket
            currentCharPosition = IndexOfNextUnescapedChar(splitStringToCorrect[indexOfCurrentString], 0, new Char[] { '"', '}' });
            do
            {
                // If this char is a quotation mark
                if (splitStringToCorrect[indexOfCurrentString][currentCharPosition] == '"')
                {
                    // If this is not the first custom property add a comma to the end of the last custom property
                    splitStringToCorrect[0] += (isFirstProperty) ? "" : ",";
                    // This is the start of a custom property, so add the appropriate text
                    splitStringToCorrect[0] += "{";
                    // Find the index of the end of this quote
                    indexOfEndQuote = IndexOfNextUnescapedChar(splitStringToCorrect[indexOfCurrentString], currentCharPosition + 1, new Char[] { '"' });
                    // Get and add the name
                    splitStringToCorrect[0] += "\"name\":\"" + splitStringToCorrect[indexOfCurrentString].Substring(currentCharPosition + 1, indexOfEndQuote - currentCharPosition - 1) + "\",";
                    // Move to the start of the value
                    currentCharPosition = IndexOfNextUnescapedChar(splitStringToCorrect[indexOfCurrentString], indexOfEndQuote + 1, new Char[] { '"' });
                    // Find the index of the end of this quote
                    indexOfEndQuote = IndexOfNextUnescapedChar(splitStringToCorrect[indexOfCurrentString], currentCharPosition + 1, new Char[] { '"' });
                    // Get and add the value
                    splitStringToCorrect[0] += "\"value\":\"" + splitStringToCorrect[indexOfCurrentString].Substring(currentCharPosition + 1, indexOfEndQuote - currentCharPosition - 1) + "\"";
                    // Move to the first unescaped quote or end curly bracket
                    currentCharPosition = IndexOfNextUnescapedChar(splitStringToCorrect[indexOfCurrentString], indexOfEndQuote + 1, new Char[] { '"', '}' });
                    // This is the end of a custom property, so add the appropriate text
                    splitStringToCorrect[0] += "}";
                    // The next custom property is not the first property
                    isFirstProperty = false;
                }
            }
            while (splitStringToCorrect[indexOfCurrentString][currentCharPosition] != '}'); // While the end of custom properties has not been reached
            // This is the end of this property set, so add the appropriate text
            splitStringToCorrect[0] += "]";
            // Stitch together the corrected, first string with everything in the current string after this set of properties
            splitStringToCorrect[0] += splitStringToCorrect[indexOfCurrentString].Substring(currentCharPosition + 1);
        }
        stringToCorrect = splitStringToCorrect[0];
    }


    // This modifies tile properties so that they can be properly parsed
    void CorrectTileProperties(ref string stringToCorrect)
    {
        // This will store the parts of the string to correct
        string[] splitStringToCorrect;
        // Stores the position of the character that is being examined
        int currentCharPosition = 0;
        // Used to determine if the current tile is the first tile in this list
        bool isFirstTile = true;
        // Used to store the index of the end of each quote
        int indexOfEndQuote;

        // Find and split at the beginning of tie custom properties
        splitStringToCorrect = stringToCorrect.Split(new string[] { "\"tileproperties\":\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        // If there are no tile properties in this map do not proceed any further
        if (splitStringToCorrect.Length <= 1) { return; }
        // Go through each set of properties
        for (int indexOfCurrentString = 1; indexOfCurrentString < splitStringToCorrect.Length; indexOfCurrentString++)
        {
            // Add the tile properties identifier back onto the corrected string
            splitStringToCorrect[0] += "\"tileproperties\":[";
            // Start at the first unescaped quote or end curly bracket
            currentCharPosition = IndexOfNextUnescapedChar(splitStringToCorrect[indexOfCurrentString], 0, new Char[] { '"', '}' });
            do
            {
                // If this char is a quotation mark
                if (splitStringToCorrect[indexOfCurrentString][currentCharPosition] == '"')
                {
                    // Used to determine if the current property is the first custom property in this list
                    bool isFirstProperty = true;

                    // Find the index of the end of this quote
                    indexOfEndQuote = IndexOfNextUnescapedChar(splitStringToCorrect[indexOfCurrentString], currentCharPosition + 1, new Char[] { '"' });
                    // If this is not the first tile add a comma to the end of the last tile properties
                    splitStringToCorrect[0] += (isFirstTile) ? "" : ",";
                    // This is the start of a custom tile, so add the appropriate text
                    splitStringToCorrect[0] += "{";
                    // Get and add the tileid
                    splitStringToCorrect[0] += "\"tileid\":" + splitStringToCorrect[indexOfCurrentString].Substring(currentCharPosition + 1, indexOfEndQuote - currentCharPosition - 1) + ',';
                    // Move to the first unescaped quote or end curly bracket
                    currentCharPosition = IndexOfNextUnescapedChar(splitStringToCorrect[indexOfCurrentString], indexOfEndQuote + 1, new Char[] { '"', '}' });
                    // Get and add this tile's properties
                    splitStringToCorrect[0] += "\"customproperties\":[";
                    do
                    {
                        // If this char is a quotation mark
                        if (splitStringToCorrect[indexOfCurrentString][currentCharPosition] == '"')
                        {
                            // If this is not the first custom property add a comma to the end of the last custom property
                            splitStringToCorrect[0] += (isFirstProperty) ? "" : ",";
                            // This is the start of a custom property, so add the appropriate text
                            splitStringToCorrect[0] += "{";
                            // Find the index of the end of this quote
                            indexOfEndQuote = IndexOfNextUnescapedChar(splitStringToCorrect[indexOfCurrentString], currentCharPosition + 1, new Char[] { '"' });
                            // Get and add the name
                            splitStringToCorrect[0] += "\"name\":\"" + splitStringToCorrect[indexOfCurrentString].Substring(currentCharPosition + 1, indexOfEndQuote - currentCharPosition - 1) + "\",";
                            // Move to the start of the value
                            currentCharPosition = IndexOfNextUnescapedChar(splitStringToCorrect[indexOfCurrentString], indexOfEndQuote + 1, new Char[] { '"' });
                            // Find the index of the end of this quote
                            indexOfEndQuote = IndexOfNextUnescapedChar(splitStringToCorrect[indexOfCurrentString], currentCharPosition + 1, new Char[] { '"' });
                            // Get and add the value
                            splitStringToCorrect[0] += "\"value\":\"" + splitStringToCorrect[indexOfCurrentString].Substring(currentCharPosition + 1, indexOfEndQuote - currentCharPosition - 1) + "\"";
                            // Move to the first unescaped quote or end curly bracket
                            currentCharPosition = IndexOfNextUnescapedChar(splitStringToCorrect[indexOfCurrentString], indexOfEndQuote + 1, new Char[] { '"', '}' });
                            // This is the end of a custom property, so add the appropriate text
                            splitStringToCorrect[0] += "}";
                            // The next custom property is not the first property
                            isFirstProperty = false;
                        }
                    }
                    while (splitStringToCorrect[indexOfCurrentString][currentCharPosition] != '}'); // While the end of custom properties has not been reached
                                                                                 // Move to the first unescaped quote or end curly bracket
                    currentCharPosition = IndexOfNextUnescapedChar(splitStringToCorrect[indexOfCurrentString], currentCharPosition + 1, new Char[] { '"', '}' });
                    // This is the end of this tile's custom properties, so add the appropriate text
                    splitStringToCorrect[0] += "]}";
                    // The next tile is not the first tile
                    isFirstTile = false;
                }
            }
            while (splitStringToCorrect[indexOfCurrentString][currentCharPosition] != '}'); // While the end of tile properties has not been reached
            // This is the end of this tile properties, so add the appropriate text
            splitStringToCorrect[0] += "]";
            // Stitch together the corrected, first string with everything in this string after the tile properties
            splitStringToCorrect[0] += splitStringToCorrect[indexOfCurrentString].Substring(currentCharPosition + 1);
        }
        stringToCorrect = splitStringToCorrect[0];
    }


    // This searches a string and returns the index the first nonesacped instance of one of the requested characters
    int IndexOfNextUnescapedChar(string stringToSearch, int startPosition, params char[] charsToFind)
    {
        // If the initial index is out of bounds
        if (startPosition >= stringToSearch.Length)
        {
            return -1;
        }
        // For each character, in the string to search, at or after the initial index
        for (int currentCharIndex = startPosition; currentCharIndex < stringToSearch.Length; currentCharIndex++)
        {
            // If this char is one of the characters to find
            if (charsToFind.Contains<char>(stringToSearch[currentCharIndex]))
            {
                // If this is a non escaped character return the current position
                if (currentCharIndex == 0 || stringToSearch[currentCharIndex - 1] != '\\')
                {
                    return currentCharIndex;
                }
            }
        }
        return -1;
    }
}