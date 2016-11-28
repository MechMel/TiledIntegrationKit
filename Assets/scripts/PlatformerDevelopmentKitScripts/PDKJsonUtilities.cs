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
        // TODO: FILL THIS IN LATER
        string stringToCreateMapFrom = textAssetToCreateMapFrom.ToString();
        // Stores the map to return
        PDKMap pdkMap;

        // Correct custom properties
        CorrectCustomProperties(ref stringToCreateMapFrom);
        // Correct tile properties
        CorrectTileProperties(ref stringToCreateMapFrom);
        // Create a PDKMap based on the map's Json file
        pdkMap = JsonUtility.FromJson<PDKMap>(stringToCreateMapFrom);
        // Return the newly created PDKMap
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