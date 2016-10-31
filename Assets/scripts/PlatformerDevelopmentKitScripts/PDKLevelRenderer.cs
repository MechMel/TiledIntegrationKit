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
    public List<GameObject> layerGroupObjects = new List<GameObject>();
    // TODO: Remove this later
    private float timeOfLastCheck = 0;


    // TODO: Fill this in
    public void RenderRectOfMap(TIKMap levelMap, Rect rectToRender)
    {
        // Go through each layer group to render
        for (int layerGroupNumber = 0; layerGroupNumber < levelMap.layerGroups.Count; layerGroupNumber++)
        {
            // If layer group objects have not been created yet
            if (layerGroupObjects.Count < levelMap.layerGroups.Count)
            {
                // Create a game object to render this layer group on
                GameObject thisLayerGroupObject = new GameObject();
                // Add the newly created object for this layer into the list of layer objects
                layerGroupObjects.Add(thisLayerGroupObject);
                // Name this layer's object
                layerGroupObjects[layerGroupNumber].name = "Layer Group: " + layerGroupNumber.ToString();
                // Add a sprite renderer to this layer group's game object
                SpriteRenderer thisLayerSpriteRenderer = layerGroupObjects[layerGroupNumber].AddComponent<SpriteRenderer>();
            }
            // If no texture has been rendred yet
            if (renderedTexture == null)
            {
                // Create an appropriately sized texture
                renderedTexture = new Texture2D((int)rectToRender.width * levelMap.tilewidth, (int)rectToRender.height * levelMap.tileheight);
                // Set the filter type
                renderedTexture.filterMode = FilterMode.Point;
            }
            // Render this texture
            UpdateTextureForRectOfLayerGroup(renderedTexture, levelMap, levelMap.layerGroups[layerGroupNumber], rectToRender);

            // TODO: get sorting layers working
            //thisLayerSpriteRenderer.sortingLayerName = layerNumberToRender.ToString() + " " + mapToRender.layers[layerNumberToRender].name;
            // TODO: Fill this in later
            Rect rectangleToRenderTopLeft = new Rect(rectToRender.x - (rectToRender.width / 2), rectToRender.y + (rectToRender.height / 2), rectToRender.width, rectToRender.height);
            // Create a sprite from the texture to render
            Sprite spriteToDisplay = Sprite.Create(
                texture: renderedTexture, 
                rect: new Rect(0, 0, (int)rectToRender.width * levelMap.tilewidth, (int)rectToRender.height * levelMap.tileheight), 
                pivot: new Vector2(0.5f, 0.5f), 
                pixelsPerUnit: levelMap.tilewidth);
            // Display the sprite of the area to render
            layerGroupObjects[layerGroupNumber].GetComponent<SpriteRenderer>().sprite = spriteToDisplay;
            // Put this layer in the correct position
            layerGroupObjects[layerGroupNumber].transform.position = new Vector3((int)rectToRender.x + ((int)rectToRender.width / 2), -(int)rectToRender.y + ((int)rectToRender.height / 2), -layerGroupNumber);
        }
        // Store the curretly rendered rectangle of the map
        renderedRectOfMap = rectToRender;
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