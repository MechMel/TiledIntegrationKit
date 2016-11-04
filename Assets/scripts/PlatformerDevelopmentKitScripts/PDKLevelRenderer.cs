using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PDKLevelRenderer : MonoBehaviour
{
    // This is the texture that is currently rendered
    public Texture2D[] layerGroupTextures;
    // TODO: Fill this in later
    public TIKMap levelMap;
    // This is used to remember what area of the map is currently rendered
    public Rect renderedRectOfMap;
    // This is used to store all layer group objects that this renderer creates
    public List<GameObject> layerGroupObjects = new List<GameObject>();
    // TODO: Remove this later
    //private float timeOfLastCheck = 0;



    // TODO: Fill this in later
    public PDKLevelRenderer(TIKMap mapToUse)
    {
        // TODO: Fill this in later  
        levelMap = mapToUse;
        // Set the layer group textures to the appropriate size
        layerGroupTextures = new Texture2D[levelMap.layerGroups.Count];
        //
        renderedRectOfMap = new Rect(0, 0, 1, 1);
        // Go through each layer group in this map
        for (int layerGroupNumber = 0; layerGroupNumber < mapToUse.layerGroups.Count; layerGroupNumber++)
        {
            // Create a game object to render this layer group on
            GameObject thisLayerGroupObject = new GameObject();
            // Add the newly created object for this layer into the list of layer objects
            layerGroupObjects.Add(thisLayerGroupObject);
            // Name this layer's object
            layerGroupObjects[layerGroupNumber].name = "Layer Group: " + layerGroupNumber.ToString();
            // Add a sprite renderer to this layer group's game object
            SpriteRenderer thisLayerSpriteRenderer = layerGroupObjects[layerGroupNumber].AddComponent<SpriteRenderer>();
            // Create an appropriately sized texture
            layerGroupTextures[layerGroupNumber] = new Texture2D(1, 1);
            // Set the filter mode
            layerGroupTextures[layerGroupNumber].filterMode = FilterMode.Point;
            // Aplly these changes
            layerGroupTextures[layerGroupNumber].Apply();

        }
    }
    
    // This adjust each layer group's object so that is is at a new given positon rendering the correct portion of the map
    public void RenderRectOfMap(Rect rectToRender)
    {
        // Go through each layer group to render
        for (int layerGroupNumber = 0; layerGroupNumber < levelMap.layerGroups.Count; layerGroupNumber++)
        {
            // Render this texture
            UpdateTextureForRectOfLayerGroup(ref layerGroupTextures[layerGroupNumber], levelMap.layerGroups[layerGroupNumber], rectToRender);
            // Create a sprite from the texture to render
            Sprite spriteToDisplay = Sprite.Create(
                texture: layerGroupTextures[layerGroupNumber], 
                rect: new Rect(
                    x: 0, 
                    y: 0, 
                    width: layerGroupTextures[layerGroupNumber].width, 
                    height: layerGroupTextures[layerGroupNumber].height), 
                pivot: new Vector2(
                    x: 0.5f, 
                    y: 0.5f), 
                pixelsPerUnit: levelMap.tilewidth);
            // Display the sprite of the area to render
            layerGroupObjects[layerGroupNumber].GetComponent<SpriteRenderer>().sprite = spriteToDisplay;
            // Move the object for this layer group to the correct position
            layerGroupObjects[layerGroupNumber].transform.position = new Vector3(
                x:  (int)rectToRender.x + ((int)rectToRender.width / 2),
                y: -(int)rectToRender.y + ((int)rectToRender.height / 2),
                z:  -layerGroupNumber);
        }
        // Store the curretly rendered rectangle of the map
        renderedRectOfMap = rectToRender;
    }

    // When this is called this function creates a texture from a given rectangle of a given layer group
    private void UpdateTextureForRectOfLayerGroup(ref Texture2D textureToUpdate, PDKLayerGroup givenLayerGroup, Rect rectToRender)
    {
        // TODO: Fill This In Later
        List<int> positionsWithTiles = new List<int>();
        // TODO: Fill This In Later
        List<int> opaqueTilePositions = new List<int>();
        // TODO: Fill This In Later
        Rect overlapRect = GetOverlap(renderedRectOfMap, rectToRender);
        //
        List<int> outsidePositions = new List<int>();

        if (overlapRect.width > 0 && overlapRect.height > 0)
        {
            // TODO: Fill This In Later
            Color[] colorsInOverlap = textureToUpdate.GetPixels(
                x: (int)(overlapRect.xMin - renderedRectOfMap.xMin) * levelMap.tilewidth,
                y: (int)(renderedRectOfMap.height - (overlapRect.yMax - renderedRectOfMap.yMin)) * levelMap.tileheight,
                blockWidth: (int)overlapRect.width * levelMap.tilewidth,
                blockHeight: (int)overlapRect.height * levelMap.tileheight);
        }
        #region TEST
        for (int y = (int)rectToRender.yMin; y < overlapRect.yMin; y++)
        {
            for (int x = (int)rectToRender.xMin; x <= rectToRender.xMax; x++)
            {
                outsidePositions.Add((y * levelMap.width) + x);
            }
        }
        for (int y = (int)overlapRect.yMin; y < overlapRect.yMax; y++)
        {
            for (int x = (int)rectToRender.xMin; x < overlapRect.xMin; x++)
            {
                outsidePositions.Add((y * levelMap.width) + x);
            }
            for (int x = (int)overlapRect.xMin; x < rectToRender.xMax; x++)
            {
                outsidePositions.Add((y * levelMap.width) + x);
            }
        }
        for (int y = (int)overlapRect.yMax; y < rectToRender.yMax; y++)
        {
            if (y < rectToRender.yMin)
                y = (int)rectToRender.yMin;
            for (int x = (int)rectToRender.xMin; x < rectToRender.xMax; x++)
            {
                outsidePositions.Add((y * levelMap.width) + x);
            }
        }
        #endregion
        // If the rect to render has diffrent dimmensions then the current texture
        if (rectToRender.width * levelMap.tilewidth != textureToUpdate.width || rectToRender.height * levelMap.tileheight  != textureToUpdate.height)
        {
            // Adjust the size of the texture to update
            textureToUpdate = new Texture2D((int)rectToRender.width * levelMap.tilewidth, (int)rectToRender.height * levelMap.tileheight);
            // Set the filter mode
            textureToUpdate.filterMode = FilterMode.Point;
            // Aplly these changes
            textureToUpdate.Apply();
        }
        // Create a new transparent texture to store the requested rectangle of the map
        SetTextureToTransparent(textureToUpdate);

        // For each layer to render
        foreach (int layerNumber in givenLayerGroup.layerNumbers)
        {
            // TODO: REMOVE THIS LATER
            Dictionary<int, List<int>> tilePositions = levelMap.GetAllTilePositionsFromLayerInList(layerNumber, outsidePositions);
            // Create a dictionary to store each tile ID and the positions of these IDs in the requested rectangle of the map
            //Dictionary<int, List<int>> tilePositions = levelMap.GetAllTilePositionsFromLayerInRectangle(layerNumber, rectToRender);

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
                        int thisLocalTilePosition = ((thisTilePosition % levelMap.width) - (int)rectToRender.x) + (((thisTilePosition / levelMap.width) - (int)rectToRender.y) * (int)rectToRender.width) ;
                        // Calculate The x position of the pixel that this tile will start at
                        int initialPixelX = levelMap.tilewidth * (thisLocalTilePosition % (int)rectToRender.width);
                        // Calculate The y position of the pixel that this tile will start at
                        int initialPixelY = levelMap.tileheight * (((int)(rectToRender.height * rectToRender.width) - thisLocalTilePosition) / (int)rectToRender.width);
                        // If there is no tile already at this position
                        if (!positionsWithTiles.Contains(thisTilePosition))
                        {
                            if (initialPixelX > textureToUpdate.width - 1 || initialPixelX < 0 ||
                                initialPixelY > textureToUpdate.height - 1 || initialPixelY < 0)
                            {
                                int test = 3;
                            }
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
    private void GetPixesForRectOfLayerGroup(Color[] colorArrayToModify, PDKLayerGroup givenLayerGroup, Rect rectOfToRender)
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

    // This finds the overlaping area of two rectangles and returns the overlap relative to the first rectangle
    private Rect GetOverlap(Rect firstRect, Rect secondRect)
    {
        // Create a rectangle to store the overlap
        Rect overlapRect = new Rect();
        // Find the x and y offset between the two rectangles
        int xOffset = (int)firstRect.xMin - (int)secondRect.xMin;
        int yOffset = (int)firstRect.yMin - (int)secondRect.yMin;

        // Determine where this rectangle starts
        overlapRect.x = secondRect.xMin + xOffset;
        overlapRect.y = secondRect.yMin + yOffset;
        // Determine how wide the rectangle should be
        overlapRect.width = firstRect.width - Mathf.Abs(xOffset);
        overlapRect.height = firstRect.height - Mathf.Abs(yOffset);
        // Return a rectangle containing the overlap
        return overlapRect;
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