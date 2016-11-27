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
        // TODO: REMOVE THIS LATER
        string stringToCreateMapFrom = RewriteCustomProperties(textAssetToCreateMapFrom.ToString(), "\"properties\":\r\n", '"', '"');
        // Create a TIKMap based on the map's Json file
        PDKMap pdkMap = JsonUtility.FromJson<PDKMap>(stringToCreateMapFrom);
        // Return the newly created TIKMap
        return pdkMap;
    }


    // TODO: FILL THIS IN LATER
    public string CorrectCustomProperties(string stringToCorrect)
    {
        // TODO: FILL THIS IN LATER
        string[] chunks;

        // TODO: FILL THIS IN LATER
        chunks = stringToCorrect.Split(PROPERTIES_INDENTIFIER, StringSplitOptions.RemoveEmptyEntries);
        // TODO: FILL THIS IN LATER
        for (int thisChunkIndex = 1; thisChunkIndex < chunks.Length; thisChunkIndex++)
        {
            // TODO: FILL THIS IN LATER
            string propertiesText = "";
            // TODO: FILL THIS IN LATER
            List<int> indexOfQuotes = new List<int>();

            // Add the properties indicator back on the previous chunk
            chunks[0] += "\"properties\":[\r\n";
            // TODO: FILL THIS IN LATER
            for (int thisCharIndex = 0; !(chunks[thisChunkIndex][thisCharIndex] == '}' && indexOfQuotes.Count % 2 == 0); thisCharIndex++)
            {
                // If this character is a {
                if (chunks[thisChunkIndex][thisCharIndex] == '{' && )
                // If this character is a quotation mark
                if (chunks[thisChunkIndex][thisCharIndex] == '"')
                {
                    // Store the index of this quotation mark
                    indexOfQuotes.Add(thisCharIndex);
                }
            }
            // TODO: FILL THIS IN LATER
            for (int indexOfThisProperty = 0; indexOfThisProperty < indexOfQuotes.Count / 4; indexOfThisProperty++)
            {
                // Insert initial name text
                propertiesText += "{\r\n\"name\":";
                // TODO: FILL THIS IN LATER
                for (int thisCharIndex = indexOfQuotes[indexOfThisProperty * 4]; thisCharIndex <= indexOfQuotes[indexOfThisProperty * 4 + 1]; thisCharIndex++)
                {
                    // Insert this character into the properties text
                    propertiesText += chunks[thisChunkIndex][thisCharIndex].ToString();
                }
                // Insert comma
                propertiesText += ",\r\n";
                // Insert initial value text
                propertiesText += "\"value\":";
                // TODO: FILL THIS IN LATER
                for (int thisCharIndex = indexOfQuotes[indexOfThisProperty * 4 + 2]; thisCharIndex <= indexOfQuotes[indexOfThisProperty * 4 + 3]; thisCharIndex++)
                {
                    // Insert this character into the properties text
                    propertiesText += chunks[thisChunkIndex][thisCharIndex].ToString();
                }
                // Insert end curly brackets
                propertiesText += "}";
                // If this is not the last propertey
                if (indexOfThisProperty < (indexOfQuotes.Count / 4) - 1)
                {
                    // Insert comma
                    propertiesText += ",";
                }
                // Add the return and null characters
                propertiesText += "\r\n";
            }
            // Insert end square brackets
            propertiesText += "],\r\n";
            // TODO: FILL THIS IN LATER
            chunks[thisChunkIndex] = chunks[thisChunkIndex].Remove(0, chunks[thisChunkIndex].IndexOf("},\r\n") + 4);
            // TODO: FILL THIS IN LATER
            chunks[0] += propertiesText;
            // Add this chunk onto the end of the first one
            chunks[0] += chunks[thisChunkIndex];
        }
        // TODO: FILL THIS IN LATER
        return chunks[0];
    }


    // TODO: FILL THIS IN LATER
    string RewriteTileProperties(string stringToCorrect)
    {
        // This is the string used to find the beginning of tile properties
        string[] tilePropertiesIdentifier = { "\"tileproperties\":\r\n" };
        // This will store the parts of the string to correct
        string[] correctedString;
        // This stores the tile properties text
        string tilePropertiesText = "";
        // This stores all the nodes in tileproperties
        List<string> nodes;
        // This stores the number of curly brackets in the tile properties
        int numberOfCurlyBrackets = 1;

        // Find and split at the beginning of tie custom properties
        correctedString = stringToCorrect.Split(tilePropertiesIdentifier, StringSplitOptions.RemoveEmptyEntries);
        // Add the tile properties identifier back onto the corrected string
        correctedString[0] += "\"tileproperties\":[\r\n";
        // Go through each char after tile properties
        for (int thisCharIndex = 15; thisCharIndex < correctedString[1].Length; thisCharIndex++)
        {
            //
        }
    }


    // TODO: FILL THIS IN LATER
    public string RewriteCustomProperties(string stringToCorrect, string propertiesIdentifier, char nameIdentifer, char valueIdentifier)
    {
        // Setup the identifiers
        string[] propertiesIdentifiers = {propertiesIdentifier};
        // TODO: FILL THIS IN LATER
        string[] chunks;


        // TODO: FILL THIS IN LATER
        propertiesIdentifier = propertiesIdentifier.Remove(propertiesIdentifier.Length - 2) + "[\r\n";
        // TODO: FILL THIS IN LATER
        chunks = stringToCorrect.Split(propertiesIdentifiers, StringSplitOptions.RemoveEmptyEntries);
        // TODO: FILL THIS IN LATER
        for (int thisChunkIndex = 1; thisChunkIndex < chunks.Length; thisChunkIndex++)
        {
            // TODO: FILL THIS IN LATER
            string propertiesText = "";
            // TODO: FILL THIS IN LATER
            List<int> indexOfIdentifiers = new List<int>();

            // Add this properties indicator back
            chunks[0] += propertiesIdentifier;
            // TODO: FILL THIS IN LATER
            for (int thisCharIndex = 0; !(IsCharFollowedByRN(chunks[thisChunkIndex], thisChunkIndex, '}') && indexOfIdentifiers.Count % 2 == 0); thisCharIndex++)
            {
                // If this character is a {
                if (IsCharFollowedByRN(chunks[thisChunkIndex], thisChunkIndex, '}'))
                {

                }
                // If this character is a quotation mark
                if (chunks[thisChunkIndex][thisCharIndex] == nameIdentifer || chunks[thisChunkIndex][thisCharIndex] == valueIdentifier)
                {
                    // Store the index of this quotation mark
                    indexOfIdentifiers.Add(thisCharIndex);
                }
            }
            // TODO: FILL THIS IN LATER
            for (int indexOfThisProperty = 0; indexOfThisProperty < indexOfIdentifiers.Count / 4; indexOfThisProperty++)
            {
                // Insert initial name text
                propertiesText += "{\r\n\"name\":";
                // TODO: FILL THIS IN LATER
                for (int thisCharIndex = indexOfIdentifiers[indexOfThisProperty * 4]; thisCharIndex <= indexOfIdentifiers[indexOfThisProperty * 4 + 1]; thisCharIndex++)
                {
                    // Insert this character into the properties text
                    propertiesText += chunks[thisChunkIndex][thisCharIndex].ToString();
                }
                // Insert comma
                propertiesText += ",\r\n";
                // Insert initial value text
                propertiesText += "\"value\":";
                // TODO: FILL THIS IN LATER
                for (int thisCharIndex = indexOfIdentifiers[indexOfThisProperty * 4 + 2]; thisCharIndex <= indexOfIdentifiers[indexOfThisProperty * 4 + 3]; thisCharIndex++)
                {
                    // Insert this character into the properties text
                    propertiesText += chunks[thisChunkIndex][thisCharIndex].ToString();
                }
                // Insert end curly brackets
                propertiesText += "}";
                // If this is not the last propertey
                if (indexOfThisProperty < (indexOfIdentifiers.Count / 4) - 1)
                {
                    // Insert comma
                    propertiesText += ",";
                }
                // Add the return and null characters
                propertiesText += "\r\n";
            }
            // Insert end square brackets
            propertiesText += "],\r\n";
            // TODO: FILL THIS IN LATER
            chunks[thisChunkIndex] = chunks[thisChunkIndex].Remove(0, chunks[thisChunkIndex].IndexOf("},\r\n") + 4);
            // TODO: FILL THIS IN LATER
            chunks[0] += propertiesText;
            // Add this chunk onto the end of the first one
            chunks[0] += chunks[thisChunkIndex];
        }
        // TODO: FILL THIS IN LATER
        return chunks[0];
    }


    // This returns a list of all nodes (text inbetween curly brackets) in a given string
    List<string> GetNodes(string textToGetNodesFrom)
    {
        // This will hold each of the nodes
        List<string> nodes = new List<string>();
        // This is used to determine the number of begin curly brackets a given node
        int numberOfBrackets = 0;
        // This stores the index of the current node
        int thisNodeIndex = -1;
        // This is used to determine when to stop adding characters to a node
        bool shouldAddCharacters = false;

        // Go through each character in the text to get nodes from
        for (int thisCharIndex = 0; thisCharIndex < textToGetNodesFrom.Length; thisCharIndex++)
        {
            // If this character is a begin curly bracket
            if (IsCharFollowedByRN(textToGetNodesFrom, thisCharIndex, '{'))
            {
                // Incriment the number of brackets in this node
                numberOfBrackets++;
                // If this is the first curly bracket in this node
                if (numberOfBrackets == 1)
                {
                    // Start a new node
                    nodes.Add("");
                    // Add characters this new node
                    thisNodeIndex++;
                    // Start adding characters again
                    shouldAddCharacters = true;
                    // Skip past \r\n
                    thisCharIndex += 2;
                }
            }
            else if (IsCharFollowedByRN(textToGetNodesFrom, thisCharIndex, '}') || (IsCharFollowedByRN(textToGetNodesFrom, thisCharIndex, ',') && textToGetNodesFrom[thisCharIndex - 1] == '}')) // If this character is an end curly bracket
            {
                // Decriment the number of brackets in this node
                numberOfBrackets--;
                // If this is the last curly bracket in this node
                if (numberOfBrackets == 0)
                {
                    // Skip past \r\n
                    thisCharIndex += 2;
                    // Stop adding characters to this node
                    shouldAddCharacters = false;
                }
            }
            else if (shouldAddCharacters) // If characters should be added to this node
            {
                // Add this character to this node
                nodes[thisNodeIndex] += textToGetNodesFrom[thisCharIndex];
            }
        }
        // Return the list of nodes
        return nodes;
    }


    // This searches a string and returns the position the first nonesacped instance of one of the requested characters
    int PositionOfNextUnescapedChar(string stringToSearch, int startPosition, params char[] charsToFind)
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


    // TODO: FILL THIS IN LATER
    bool IsCharFollowedByRN(string stringToCheck, int indexOfChar, char charToCheckAgainst)
    {
        // TODO: FILL THIS IN LATER
        return stringToCheck[indexOfChar] == charToCheckAgainst && stringToCheck[indexOfChar + 1] == '\r' && stringToCheck[indexOfChar + 2] == '\n';
    }
}
