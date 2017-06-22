using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class PDKTiledUtilities
{
    // When this is called it creates and returns a new TIKMap from a given TextAsset from a tiled map
    /* Detailed Explanation...
        A tiled map does not store data in the optimal format for a pdkMap. Thus a PDKTiledMap must
        first be created from the json file for a tiled map, and then converted into a PDKMap.
    */
    public PDKMap CreatePDKMapFromTextAsset(TextAsset textAssetToCreateMapFrom)
    {
        // Create a PDKTiledMap from the given text asset
        PDKTiledMap pdkTiledMap = CreatePDKTiledMapFromTextAsset(textAssetToCreateMapFrom);
        // Convert the PDKTiledMap to a PDKMap
        PDKMap pdkMap = ConvertPDKTiledMapToPDKMap(pdkTiledMap);

        // Return the newly created PDKMap
        return pdkMap;
    }


    // When this is called it creates and returns a new TIKMap from a given TextAsset from a tiled map
    /* Detailed Explanation...
    The name of a custom property in a tiled map is the name of that custom property in the 
        json file. However, to obtain the value of a property in a json, we need to know the name
        of that property. The name of user defined custom properties cannot be known. So after the
        json is conferted to a string, all custom properties are converted to name value pairs, and
        then this string is used to created a PDKTiledMap.
    */
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
    /* Detailed Explanation...
        This goes through each property PDKTiledMap tiled map and coppies it to a PDKMap
    */
    public PDKMap ConvertPDKTiledMapToPDKMap(PDKTiledMap pdkTiledMapToConvert)
    {
        // Stores the pdkMap that will be returned
        PDKMap pdkMap = new PDKMap();

        #region Copy Map Properties
        pdkMap.width = pdkTiledMapToConvert.width;
        pdkMap.height = pdkTiledMapToConvert.height;
        pdkMap.tileWidth = pdkTiledMapToConvert.tilewidth;
        pdkMap.tileHeight = pdkTiledMapToConvert.tileheight;
        pdkMap.properties = new PDKMap.PDKCustomProperties();
        // If the map had custom properties
        if (pdkTiledMapToConvert.properties != null)
        {
            // Converat and copy each property
            foreach (PDKTiledCustomProperty currentProperty in pdkTiledMapToConvert.properties)
            {
                pdkMap.properties.Add(currentProperty.name, currentProperty.value);
            }
        }
        #endregion
        #region Copy Tileset Properties
        bool thisColliderTypeWasFound = false;
        pdkMap.tilesets = new PDKTileset[pdkTiledMapToConvert.tilesets.Length];
        // Inisilize the map's collider properties
        pdkMap.tilesWithColliders = new PDKMap.PDKIDHashet();
        pdkMap.tileColliderObjects = new PDKMap.PDKColliderObjectsForTileIDs();
        pdkMap.colliderTypes = new PDKMap.PDKColliderTypes();
        // I there are tilesets in this map
        if (pdkTiledMapToConvert.tilesets != null)
        {
            // Convert and copy each tileset
            for (int currentTilesetIndex = 0; currentTilesetIndex < pdkTiledMapToConvert.tilesets.Length; currentTilesetIndex++)
            {
                pdkMap.tilesets[currentTilesetIndex] = new PDKTileset();
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
                pdkMap.tilesets[currentTilesetIndex].properties = new PDKMap.PDKCustomProperties();
                // If this tileset has costom properties
                if (pdkTiledMapToConvert.tilesets[currentTilesetIndex].properties != null)
                {
                    // Convert and copy each property
                    foreach (PDKTiledCustomProperty currentProperty in pdkTiledMapToConvert.tilesets[currentTilesetIndex].properties)
                    {
                        pdkMap.tilesets[currentTilesetIndex].properties.Add(currentProperty.name, currentProperty.value);
                    }
                }
                pdkMap.tilesets[currentTilesetIndex].tileProperties = new PDKMap.PDKCustomProperties[pdkTiledMapToConvert.tilesets[currentTilesetIndex].tilecount];
                // Go through each tile in this map. If this tile has tile properties convert and copy each property
                for (int currentTileIndex = 0; currentTileIndex < pdkTiledMapToConvert.tilesets[currentTilesetIndex].tilecount; currentTileIndex++)
                {
                    pdkMap.tilesets[currentTilesetIndex].tileProperties[currentTileIndex] = new PDKMap.PDKCustomProperties();
                    // If this tile has tile properties
                    if (pdkTiledMapToConvert.tilesets[currentTilesetIndex].tileproperties != null)
                    {
                        // Go through each possible tile properties
                        foreach (PDKTiledTileProperties currentTileProperties in pdkTiledMapToConvert.tilesets[currentTilesetIndex].tileproperties)
                        {
                            // If this tile has custom properites
                            if (currentTileIndex == currentTileProperties.tileid)
                            {
                                // Convert and copy each property
                                foreach (PDKTiledCustomProperty currentCustomProperty in currentTileProperties.customproperties)
                                {
                                    // Copy this property
                                    pdkMap.tilesets[currentTilesetIndex].tileProperties[currentTileIndex].Add(currentCustomProperty.name, currentCustomProperty.value);
                                    // If this tile has a custom collider add it to the list of tiles with custom colliders
                                    if (currentCustomProperty.name == "PDKCustomCollider")
                                    {
                                        // Look through each of the existing collider types to see if this one already exists
                                        foreach (PDKColliderType colliderTypeToCheck in pdkMap.colliderTypes)
                                        {
                                            // If this collider type already exists add this tile ID to that collider type
                                            if (colliderTypeToCheck.name == currentCustomProperty.value)
                                            {
                                                // Add this tile ID to this collider type
                                                colliderTypeToCheck.tilesWithThisCollider.Add(currentTileIndex + 1);
                                                // Add this tile ID to the list of tiles with colliders
                                                pdkMap.tilesWithColliders.Add(currentTileIndex + 1);
                                                // A collider type for this tile was found so remember that
                                                thisColliderTypeWasFound = true;
                                                // There is no need to keep looking for a collider type so break
                                                break;
                                            }
                                        }
                                        // If no matching collider type was found create a new one
                                        if (!thisColliderTypeWasFound)
                                        {
                                            // Create a new collider type
                                            PDKColliderType newColliderType = new PDKColliderType();
                                            newColliderType.name = currentCustomProperty.value;
                                            // Add this tile ID to that collider type
                                            newColliderType.tilesWithThisCollider.Add(currentTileIndex + 1);
                                            // Add this collider type to the map's list of collider types
                                            pdkMap.colliderTypes.Add(newColliderType);
                                            // Add this tile ID to the list of tiles with colliders
                                            pdkMap.tilesWithColliders.Add(currentTileIndex + 1);
                                        }
                                        // If a matching collider type was found reset the collider type fond variable
                                        else
                                        {
                                            thisColliderTypeWasFound = false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion
        #region Copy Layer Properties
        pdkMap.layers = new PDKLayer[pdkTiledMapToConvert.layers.Length];
        // Go thorugh each layer
        for (int currentLayerIndex = 0; currentLayerIndex < pdkTiledMapToConvert.layers.Length; currentLayerIndex++)
        {
            pdkMap.layers[currentLayerIndex] = new PDKLayer();
            pdkMap.layers[currentLayerIndex].name = pdkTiledMapToConvert.layers[currentLayerIndex].name;
            // Set this layer type to the appropriate layerType
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
            pdkMap.layers[currentLayerIndex].properties = new PDKMap.PDKCustomProperties();
            // If this layer had custom properties
            if (pdkTiledMapToConvert.layers[currentLayerIndex].properties != null)
            {
                // Convert and copy each property
                foreach (PDKTiledCustomProperty currentProperty in pdkTiledMapToConvert.layers[currentLayerIndex].properties)
                {
                    pdkMap.layers[currentLayerIndex].properties.Add(currentProperty.name, currentProperty.value);
                }
            }
            // If this layer had tiles(only tile layers have tiles)
            if (pdkTiledMapToConvert.layers[currentLayerIndex].data != null)
            {
                pdkMap.layers[currentLayerIndex].tileMap = new int[pdkTiledMapToConvert.layers[currentLayerIndex].data.Length];
                pdkTiledMapToConvert.layers[currentLayerIndex].data.CopyTo(pdkMap.layers[currentLayerIndex].tileMap, 0);
            }
            // If this layer had objects
            if (pdkTiledMapToConvert.layers[currentLayerIndex].objects != null)
            {
                // Initialize the obects array
                pdkMap.layers[currentLayerIndex].objects = new PDKObject[pdkTiledMapToConvert.layers[currentLayerIndex].objects.Length];
                // Initialize the object map
                pdkMap.layers[currentLayerIndex].dehydratedObjectMap = new PDKSerializable2DHashsetOfPDKObjects(pdkMap.width, pdkMap.height);
                // Go through each slot and Initialize it
                for (int currentColumnIndex = 0; currentColumnIndex < pdkTiledMapToConvert.width; currentColumnIndex++)
                {
                    // For each row
                    for (int currentRowIndex = 0; currentRowIndex < pdkTiledMapToConvert.height; currentRowIndex++)
                    {
                        // Initialize this slot
                        pdkMap.layers[currentLayerIndex].dehydratedObjectMap.SetItem(currentColumnIndex, currentRowIndex, new PDKLayer.PDKDehydratedObjectsHashSet());
                    }
                }
                // TODO: REMOVE THIS LATER
                pdkMap.layers[currentLayerIndex].hydratedObjects = new PDKLayer.PDKGameObjectsHashSet();
                for (int currentObjectIndex = 0; currentObjectIndex < pdkTiledMapToConvert.layers[currentLayerIndex].objects.Length; currentObjectIndex++)
                {
                    pdkMap.layers[currentLayerIndex].objects[currentObjectIndex] = new PDKObject();
                    pdkMap.layers[currentLayerIndex].objects[currentObjectIndex].name = pdkTiledMapToConvert.layers[currentLayerIndex].objects[currentObjectIndex].name;
                    pdkMap.layers[currentLayerIndex].objects[currentObjectIndex].type = pdkTiledMapToConvert.layers[currentLayerIndex].objects[currentObjectIndex].type;
                    pdkMap.layers[currentLayerIndex].objects[currentObjectIndex].id = currentObjectIndex; // pdkTiledMapToConvert.layers[currentLayerIndex].objects[currentObjectIndex].id - 1;
                    pdkMap.layers[currentLayerIndex].objects[currentObjectIndex].gid = pdkTiledMapToConvert.layers[currentLayerIndex].objects[currentObjectIndex].gid;
                    pdkMap.layers[currentLayerIndex].objects[currentObjectIndex].x = pdkTiledMapToConvert.layers[currentLayerIndex].objects[currentObjectIndex].x / pdkMap.tileWidth;
                    pdkMap.layers[currentLayerIndex].objects[currentObjectIndex].y = -((pdkTiledMapToConvert.layers[currentLayerIndex].objects[currentObjectIndex].y / pdkMap.tileHeight) - 1);
                    pdkMap.layers[currentLayerIndex].objects[currentObjectIndex].objectRect = new Rect(
                        x: pdkTiledMapToConvert.layers[currentLayerIndex].objects[currentObjectIndex].x / pdkMap.tileWidth,
                        y: (pdkTiledMapToConvert.layers[currentLayerIndex].objects[currentObjectIndex].y / pdkMap.tileHeight) - 1,
                        width: pdkTiledMapToConvert.layers[currentLayerIndex].objects[currentObjectIndex].width / pdkMap.tileWidth,
                        height: pdkTiledMapToConvert.layers[currentLayerIndex].objects[currentObjectIndex].height / pdkMap.tileHeight);
                    pdkMap.layers[currentLayerIndex].objects[currentObjectIndex].rotation = pdkTiledMapToConvert.layers[currentLayerIndex].objects[currentObjectIndex].rotation;
                    pdkMap.layers[currentLayerIndex].objects[currentObjectIndex].visible = pdkTiledMapToConvert.layers[currentLayerIndex].objects[currentObjectIndex].visible;
                    pdkMap.layers[currentLayerIndex].objects[currentObjectIndex].properties = new PDKMap.PDKCustomProperties();
                    // If this object had custom properties
                    if (pdkTiledMapToConvert.layers[currentLayerIndex].properties != null)
                    {
                        // Convert and copy each property
                        foreach (PDKTiledCustomProperty currentProperty in pdkTiledMapToConvert.layers[currentLayerIndex].properties)
                        {
                            pdkMap.layers[currentLayerIndex].objects[currentObjectIndex].properties.Add(currentProperty.name, currentProperty.value);
                        }
                    }
                    // Add this object at this position, to the object map
                    pdkMap.layers[currentLayerIndex].dehydratedObjectMap.GetItem(
                        x: (int)pdkMap.layers[currentLayerIndex].objects[currentObjectIndex].x, 
                        y: -(int)pdkMap.layers[currentLayerIndex].objects[currentObjectIndex].y).Add(
                        pdkMap.layers[currentLayerIndex].objects[currentObjectIndex]);
                }
            }
        }
        #endregion
        return pdkMap;
    }


    // This modifies custom properties so that they can be properly parsed
    /* Detailed Explanation...
         In order to find and modify all custom properties the given string for a tiled map is 
         split at "\"properties\":\r\n". Then Each property is copied and changed from
         userDefinedName: userDefinedValue to {name: userDefinedName, value: userDefinedValue}.
         This corrected property is added onto the end of the previous string(in the array made by
         splitting at properties). After all properties are corrected and coppied the old
         properties are removed.
    */
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
    /* Detailed Explanation...
         In order to find and modify all tiles with custom properties the given string for a tiled 
         map is split at "\"tileproperties\":\r\n". Then Each tile is copied and changed from
         rawtileID: {userDefinedProperties} to {tileID: {userDefinedProperties}}. However each of 
         these tiles have a set of custom Properteis. In order to find and modify all custom 
         properties on this tile this string is split at "\"properties\":\r\n". Then Each property 
         is copied and changed from 
         userDefinedName: userDefinedValue to {name: userDefinedName, value: userDefinedValue}.
         This corrected property is added onto the end of the previous string(in the array made by
         splitting at properties). After all properties are corrected and coppied the old
         properties are removed.
    */
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
 