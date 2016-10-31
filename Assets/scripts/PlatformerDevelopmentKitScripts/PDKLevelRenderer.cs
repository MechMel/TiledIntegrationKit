using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PDKLevelRenderer : MonoBehaviour
{
    // This is the texture that is currently rendered
    public Texture2D renderedTexture = null;
    // This is used to remember what area of the map is currently rendered
    public Rect renderedRectOfMap;
    // This is used to store all layer group objects that this renderer creates
    public Dictionary<int, GameObject> layerGroupObjects = new Dictionary<int, GameObject>();
    // TODO: Remove this later
    private float timeOfLastCheck = 0;


    // When this is called a sprite for each layer is created and a given rectangle of the map rendered on the appropriate layers
    public void RenderRectangleOfMapAtPosition(TIKMap mapToRender, Rect rectangleToRender, Vector3 positionToCreateLayersAt)
    {
        // This is used to store an ordered set of a layer groups in this map
        Dictionary<int, PDKLayerGroup> layerGroupsToRender = new Dictionary<int, PDKLayerGroup>();
        // This is used to track which layer group is currently being compared against
        int currentLayerGroupNumber = -1;

        // Go through each layer in this map
        for (int layerNumberToRender = mapToRender.layers.Length - 1; layerNumberToRender >= 0; layerNumberToRender--)
        {
            // If this layer is visible
            if (mapToRender.layers[layerNumberToRender].visible)
            {
                if (currentLayerGroupNumber < 0) // If no layer groups have been created yet
                {
                    // CThe first layer group should be created at position 0
                    currentLayerGroupNumber = 0;
                    // Create A new LayerGroup dictionary for this type of layer
                    layerGroupsToRender.Add(currentLayerGroupNumber, CreateNewLayerGroup(mapToRender.layers[layerNumberToRender].layerType, layerNumberToRender));
                }
                else if (layerGroupsToRender[currentLayerGroupNumber].groupType == mapToRender.layers[layerNumberToRender].layerType) // If this layer is the same type as the current layer group
                {
                    // Add this layer to the current layer group
                    layerGroupsToRender[currentLayerGroupNumber].layerNumbers.Add(layerNumberToRender);
                }
                else
                {
                    // Increase the current layer group number
                    currentLayerGroupNumber++;
                    // Create A new LayerGroup dictionary for this type of layer
                    layerGroupsToRender.Add(currentLayerGroupNumber, CreateNewLayerGroup(mapToRender.layers[layerNumberToRender].layerType, layerNumberToRender));
                }
            }
        }
        // Go through each layer group to render
        foreach (int layerGroupNumber in layerGroupsToRender.Keys)
        {
            //
            if (layerGroupObjects.Keys.Count == 0)
            {
                // Create a game object to render this layer group on
                GameObject thisLayerGroupObject = new GameObject();
                // Add the newly created object for this layer into the list of layer objects
                layerGroupObjects.Add(layerGroupNumber, thisLayerGroupObject);
                // Name this layer's object
                layerGroupObjects[layerGroupNumber].name = "Layer Group " + layerGroupNumber.ToString() + rectangleToRender.x.ToString() + "," + rectangleToRender.y.ToString();
                // Add a sprite renderer to this layer group's game object
                SpriteRenderer thisLayerSpriteRenderer = layerGroupObjects[layerGroupNumber].AddComponent<SpriteRenderer>();
            }
            // If no texture has been rendred yet
            if (renderedTexture == null)
            {
                // Create an appropriately sized texture
                renderedTexture = new Texture2D((int)rectangleToRender.width * mapToRender.tilewidth, (int)rectangleToRender.height * mapToRender.tileheight);
                // Set the filter type
                renderedTexture.filterMode = FilterMode.Point;
            }
            // Render this texture
            UpdateTextureForRectOfLayerGroup(renderedTexture, mapToRender, layerGroupsToRender[layerGroupNumber], rectangleToRender);

            // TODO: get sorting layers working
            //thisLayerSpriteRenderer.sortingLayerName = layerNumberToRender.ToString() + " " + mapToRender.layers[layerNumberToRender].name;
            // TODO: Fill this in later
            Rect rectangleToRenderTopLeft = new Rect(rectangleToRender.x - (rectangleToRender.width / 2), rectangleToRender.y + (rectangleToRender.height / 2), rectangleToRender.width, rectangleToRender.height);
            // Create a sprite from the texture to render
            Sprite spriteToDisplay = Sprite.Create(
                texture: renderedTexture, 
                rect: new Rect(0, 0, (int)rectangleToRender.width * mapToRender.tilewidth, (int)rectangleToRender.height * mapToRender.tileheight), 
                pivot: new Vector2(0.5f, 0.5f), 
                pixelsPerUnit: mapToRender.tilewidth);
            // Display the sprite of the area to render
            layerGroupObjects[layerGroupNumber].GetComponent<SpriteRenderer>().sprite = spriteToDisplay;
            // Put this layer in the correct position
            layerGroupObjects[layerGroupNumber].transform.position = positionToCreateLayersAt;
        }
        // Store the curretly rendered rectangle of the map
        renderedRectOfMap = rectangleToRender;
    }

    // When this is called it creates a new layer group with a specified type, and then adds a layer number to this new layer group
    private PDKLayerGroup CreateNewLayerGroup(TIKLayer.layerTypes layerGroupType, int firstLayerNumber)
    {
        // Create the new layer group
        PDKLayerGroup layerGroupToReturn = new PDKLayerGroup();
        // Set this layer group's type
        layerGroupToReturn.groupType = layerGroupType;
        // Add the first layer to this layer group
        layerGroupToReturn.layerNumbers.Add(firstLayerNumber);
        // Return the newly created layer group
        return layerGroupToReturn;
    }

    // When this is called this function creates a texture from a given rectangle of the map
    private void UpdateTextureForRectOfLayerGroup(Texture2D textureToUpdate, TIKMap levelMap, PDKLayerGroup givenLayerGroup, Rect rectangleOfLayerToRender)
    {
        // Create a new transparent texture to store the requested rectangle of the map
        SetTextureToTransparent(textureToUpdate);
        // TODO: Fill This In Later
        List<int> positionsWithTiles = new List<int>();
        // TODO: Fill This In Later
        List<int> opaqueTilePositions = new List<int>();
        
        // For each layer to render
        foreach (int layerNumber in givenLayerGroup.layerNumbers)
        {
            // Create a dictionary to store each tile ID and the positions of these IDs in the requested rectangle of the map
            Dictionary<int, List<int>> tilePositions = levelMap.GetAllTilePositionsFromLayerInRectangle(layerNumber, rectangleOfLayerToRender);

            // Go through each tile ID in the requested rectangle of the map
            foreach (int thisTileID in tilePositions.Keys)
            {
                // Create an array of colors and put the pixels from this tile into it
                Color[] thisTilesPixels = levelMap.GetTilePixels(thisTileID);

                // Go through each occurance of this tile in the requested rectangle of the map
                foreach (int thisTilePosition in tilePositions[thisTileID])
                {
                    // If this tile is not already completely opaque
                    if (!opaqueTilePositions.Contains(thisTilePosition))
                    {
                        // Calculate The x position of the pixel that this tile will start at
                        int initialPixelX = levelMap.tilewidth * (thisTilePosition % (int)rectangleOfLayerToRender.width);
                        // Calculate The y position of the pixel that this tile will start at
                        int initialPixelY = levelMap.tileheight * (((int)(rectangleOfLayerToRender.height * rectangleOfLayerToRender.width) - thisTilePosition - 1) / (int)rectangleOfLayerToRender.width);
                        // If there is no tile already at this position
                        if (!positionsWithTiles.Contains(thisTilePosition))
                        {
                            // Place this tile in the texture to return
                            textureToUpdate.SetPixels(initialPixelX, initialPixelY, levelMap.tilewidth, levelMap.tileheight, thisTilesPixels);
                            // Remember that there is tile at this position
                            positionsWithTiles.Add(thisTilePosition);
                        }
                        else
                        {
                            // TODO: Fill this in later
                            Color[] combinedTile = textureToUpdate.GetPixels(initialPixelX, initialPixelY, levelMap.tilewidth, levelMap.tileheight);
                            // TODO: Fill this in later
                            bool isCompletelyOpaque = true;

                            // For each pixel in this tile
                            for (int pixelPosition = 0; pixelPosition < thisTilesPixels.Length; pixelPosition++)
                            {
                                if (combinedTile[pixelPosition].a < 1)
                                {
                                    // Merge the old pixel with the new pixel
                                    combinedTile[pixelPosition] = (combinedTile[pixelPosition] * (1 - thisTilesPixels[pixelPosition].a)) + (thisTilesPixels[pixelPosition] * thisTilesPixels[pixelPosition].a);
                                }
                                if (combinedTile[pixelPosition].a < 1)
                                    isCompletelyOpaque = false;
                            }
                            //
                            if (isCompletelyOpaque)
                                opaqueTilePositions.Add(thisTilePosition);
                            // Place this tile in the texture to return
                            textureToUpdate.SetPixels(initialPixelX, initialPixelY, levelMap.tilewidth, levelMap.tileheight, combinedTile);
                        }
                    }
                }
            }
            // Apply the changes to the texture
            textureToUpdate.Apply();
        }
        // The texture has been updated
        return;
    }

    // When this is called this function creates a texture from a given rectangle of the map
    private void GetPixesForRectOfLayerGroup(Color[] colorArrayToModify, TIKMap levelMap, PDKLayerGroup givenLayerGroup, Rect rectOfToRender)
    {
        // These store the width and height of tiles in this map
        int tileWidth = levelMap.tilewidth;
        int tileHeight = levelMap.tileheight;
        // TODO: Fill this in later
        Color[] colorsToReturn = new Color[(int)(rectOfToRender.width * tileWidth * rectOfToRender.height * tileHeight)];
        // Create a new layer group so as not to dammage the given layer group
        List<int> numbersOfLayersToRender = new List<int>(givenLayerGroup.layerNumbers);
        // TODO: Fill this in later
        List<int> positionsWithTiles = new List<int>();

        // Start at the closest layer
        numbersOfLayersToRender.Reverse();
        // For each pixel in the colorsToReturn
        for (int thisTileindex = 0; thisTileindex < colorsToReturn.Length; thisTileindex++)
        {
            // Set this pixel to transparent
            colorsToReturn[thisTileindex] = Color.clear;
        }
        // For each layer to render
        foreach (int layerNumber in numbersOfLayersToRender)
        {
            // Create a dictionary to store each tile ID and the positions of these IDs in the requested rectangle of the map
            Dictionary<int, List<int>> tilePositions = levelMap.GetAllTilePositionsFromLayerInRectangle(layerNumber, rectOfToRender);

            // Go through each tile ID in the requested rectangle of the map
            foreach (int thisTileID in tilePositions.Keys)
            {
                // Create an array of colors and put the pixels from this tile into it
                Color[] thisTilesPixels = levelMap.GetTilePixels(thisTileID);

                // Go through each occurance of this tile in the requested rectangle of the map
                foreach (int thisTilePosition in tilePositions[thisTileID])
                {
                    // Calculate The x position of the pixel that this tile will start at
                    int initialPixelX = tileWidth * (thisTilePosition % (int)rectOfToRender.width);
                    // Calculate The y position of the pixel that this tile will start at
                    int initialPixelY = tileHeight * (((int)(rectOfToRender.height * rectOfToRender.width) - thisTilePosition - 1) / (int)rectOfToRender.width);
                    // If there is no tile already at this position
                    if (!positionsWithTiles.Contains(thisTilePosition))
                    {
                        // Remember that there is tile at this position
                        positionsWithTiles.Add(thisTilePosition);
                    }
                    else
                    {
                        Color[] oldTilePixels = new Color[5]; //textureToReturn.GetPixels(initialPixelX, initialPixelY, tileWidth, tileHeight);

                        // For each pixel in this tile
                        for (int pixelPosition = 0; pixelPosition < thisTilesPixels.Length; pixelPosition++)
                        {
                            if (oldTilePixels[pixelPosition].a >= .999) // If this pixel the new tile is transparent
                            {
                                // Keep the old pixel
                                thisTilesPixels[pixelPosition] = oldTilePixels[pixelPosition];
                            }
                            else if (oldTilePixels[pixelPosition].a <= .999)
                            {
                                // Merge the old pixel with the new pixel
                                thisTilesPixels[pixelPosition] = (oldTilePixels[pixelPosition] * (1 - thisTilesPixels[pixelPosition].a)) + (thisTilesPixels[pixelPosition] * thisTilesPixels[pixelPosition].a);
                            }
                        }
                    }
                    for (int thisPixelIndex = 0; thisPixelIndex < tileWidth * tileHeight; thisPixelIndex++)
                    {
                        //
                        colorsToReturn[0] = thisTilesPixels[thisPixelIndex];
                    }
                }
            }
        }
        // This Function Is complete
        return;
    }

    // When this is called it takes an given texture and sets all the pixels to clear
    private void SetTextureToTransparent(Texture2D textureToMakeTransparent)
    {
        // Create a new color array of transparent colors
        Color[] transparentColors = new Color[textureToMakeTransparent.width * textureToMakeTransparent.height];

        // For each pixel in the texture to return
        for (int thisColorIndex = 0; thisColorIndex < textureToMakeTransparent.width * textureToMakeTransparent.height; thisColorIndex++)
        {
            // Set this Color to Transparent
            transparentColors[thisColorIndex] = Color.clear;
        }
        // Place the transparent colors in the texture
        textureToMakeTransparent.SetPixels(transparentColors);
        // Apply the changes to this texture
        textureToMakeTransparent.Apply();
        // The texture has been set to transparent
        return;
    }

    // TODO: Remove this later
    /*
    private void LogTimeTakenForAction(string nameOfActionTaken)
    {
        float millisecondsScinceLastCheck = (Time.realtimeSinceStartup - timeOfLastCheck) * 1000;

        if (millisecondsScinceLastCheck > .095)
        {
            Debug.Log(nameOfActionTaken + ": " + ((Time.realtimeSinceStartup - timeOfLastCheck) * 1000).ToString() + " milliseconds");
            timeOfLastCheck = Time.realtimeSinceStartup;
        }
    }*/
}