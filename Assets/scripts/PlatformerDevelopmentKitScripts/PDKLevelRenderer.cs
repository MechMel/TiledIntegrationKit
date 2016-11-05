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
    private float timeOfLastCheck = 0;



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
        // This stores the colors in the overlaping area between the renderd rect and the rect to render
        Color[] overlapColors = new Color[0];
        // This will store each tile ID and the positions of these IDs for all tiles outside of the overlap
        Dictionary<int, List<int>> tilePositions;
        // This will store the pixels for each tile
        Color[] thisTilesPixels;
        // This will store the old stack of tiles when one tile is placed ontop of another
        Color[] combinedTile;
        // This will be used to determine when there are no more transparent pixels in each tile
        bool isCompletelyOpaque;

        #region Copy Overlap and Update Texture
        // If there is an overlap
        if (overlapRect.width > 0 && overlapRect.height > 0)
        {
            // Get the pixels from the old area
            overlapColors = textureToUpdate.GetPixels(
                    x: (int)(overlapRect.xMin - renderedRectOfMap.xMin) * levelMap.tilewidth,
                    y: (int)(renderedRectOfMap.height - (overlapRect.yMax - renderedRectOfMap.yMin)) * levelMap.tileheight,
                    blockWidth: (int)overlapRect.width * levelMap.tilewidth,
                    blockHeight: (int)overlapRect.height * levelMap.tileheight);
        }
        // If the rect to render has diffrent dimmensions then the current texture
        if (rectToRender.width * levelMap.tilewidth != textureToUpdate.width || rectToRender.height * levelMap.tileheight != textureToUpdate.height)
        {
            // Adjust the size of the texture to update
            textureToUpdate = new Texture2D((int)rectToRender.width * levelMap.tilewidth, (int)rectToRender.height * levelMap.tileheight);
            // Set the filter mode
            textureToUpdate.filterMode = FilterMode.Point;
            // Aplly these changes
            textureToUpdate.Apply();
        }
        // If there is an overlap
        if (overlapRect.width > 0 && overlapRect.height > 0)
        {
            // Copy the pixels from the old area to the new area
            textureToUpdate.SetPixels(
                colors: overlapColors,
                x: (int)(overlapRect.xMin - rectToRender.xMin) * levelMap.tilewidth,
                y: (int)(rectToRender.height - (overlapRect.yMax - rectToRender.yMin)) * levelMap.tileheight,
                blockWidth: (int)overlapRect.width * levelMap.tilewidth,
                blockHeight: (int)overlapRect.height * levelMap.tileheight);
        }
        #endregion
        #region Find Tiles Outside Overlap
        // If there is an overlap
        if (overlapRect.width > 0 && overlapRect.height > 0)
        {
            // For each row inside the rect to render and above the overlap
            for (int y = (int)rectToRender.yMin; y < overlapRect.yMin; y++)
            {
                // For each tile in this row
                for (int x = (int)rectToRender.xMin; x < rectToRender.xMax; x++)
                {
                    // Add this tile to the list of tiles outside the overlap
                    outsidePositions.Add((y * levelMap.width) + x);
                }
            }
            // For each row inside the rect to render and beside the overlap
            for (int y = (int)overlapRect.yMin; y < overlapRect.yMax; y++)
            {
                // For each tile on the left of the overlap
                for (int x = (int)rectToRender.xMin; x < overlapRect.xMin; x++)
                {
                    // Add this tile to the list of tiles outside the overlap
                    outsidePositions.Add((y * levelMap.width) + x);
                }
                // For each tile on the right of the overlap
                for (int x = (int)overlapRect.xMax; x < rectToRender.xMax; x++)
                {
                    // Add this tile to the list of tiles outside the overlap
                    outsidePositions.Add((y * levelMap.width) + x);
                }
            }
            // For each row inside the rect to render and below the overlap
            for (int y = (int)overlapRect.yMax; y < rectToRender.yMax; y++)
            {
                // For each tile in this row
                for (int x = (int)rectToRender.xMin; x < rectToRender.xMax; x++)
                {
                    // Add this tile to the list of tiles outside the overlap
                    outsidePositions.Add((y * levelMap.width) + x);
                }
            }
        }
        else
        {
            for (int y = (int)rectToRender.yMin; y < rectToRender.yMax; y++)
            {
                for (int x = (int)rectToRender.xMin; x < rectToRender.xMax; x++)
                {
                    outsidePositions.Add((y * levelMap.width) + x);
                }
            }
        }
        #endregion
        #region Render the Outside Tiles
        // For each layer to render
        foreach (int layerNumber in givenLayerGroup.layerNumbers)
        {
            // Get each tile ID and the positions of these IDs for all tiles outside of the overlap
            tilePositions = levelMap.GetAllTilePositionsFromLayerInList(layerNumber, outsidePositions);
            // Go through each tile ID in the requested rectangle of the map
            foreach (int thisTileID in tilePositions.Keys)
            {
                // Get the pixels for this tile
                thisTilesPixels = levelMap.GetTilePixels(thisTileID);
                // Go through each occurance of this tile in the requested rectangle of the map
                foreach (int thisTilePosition in tilePositions[thisTileID])
                {
                    // If this tile is not already completely opaque
                    if (!opaqueTilePositions.Contains(thisTilePosition))
                    {
                        // Translate the global position of this tile to a local position
                        int thisLocalTilePosition = ((thisTilePosition % levelMap.width) - (int)rectToRender.x) + (((thisTilePosition / levelMap.width) - (int)rectToRender.y) * (int)rectToRender.width);
                        // Calculate The x position of the pixel that this tile will start at
                        int initialPixelX = levelMap.tilewidth * (thisLocalTilePosition % (int)rectToRender.width);
                        // Calculate The y position of the pixel that this tile will start at
                        int initialPixelY = levelMap.tileheight * (((int)(rectToRender.height * rectToRender.width) - (thisLocalTilePosition + 1)) / (int)rectToRender.width);
                        // If there is no tile already at this position
                        if (!positionsWithTiles.Contains(thisTilePosition))
                        {
                            // Place this tile in the texture to return
                            textureToUpdate.SetPixels(initialPixelX, initialPixelY, levelMap.tilewidth, levelMap.tileheight, thisTilesPixels);
                            // Remember that there is tile at this position
                            positionsWithTiles.Add(thisTilePosition);
                        }
                        else // If tehre is a tile already at this position
                        {
                            #region Get the Old Pixels / Setup isCompletelyOpaque
                            // Get the old stack of tiles
                            combinedTile = textureToUpdate.GetPixels(initialPixelX, initialPixelY, levelMap.tilewidth, levelMap.tileheight);
                            // It has not yet been proven that this tile is completely opaque
                            isCompletelyOpaque = true;
                            #endregion
                            #region Merge the Old and New Tile
                            // For each pixel in this tile
                            for (int pixelPosition = 0; pixelPosition < thisTilesPixels.Length; pixelPosition++)
                            {
                                // If the old pixel is not opaque
                                if (combinedTile[pixelPosition].a < 1)
                                {
                                    // Merge the old pixel with the new pixel
                                    combinedTile[pixelPosition] = (combinedTile[pixelPosition] * (1 - thisTilesPixels[pixelPosition].a)) + (thisTilesPixels[pixelPosition] * thisTilesPixels[pixelPosition].a);
                                    // If this pixel is still not opaque
                                    if (combinedTile[pixelPosition].a < 1)
                                    {
                                        // This tile still has transparent pixels
                                        isCompletelyOpaque = false;
                                    }
                                }
                            }
                            #endregion
                            #region Update the Opaque Tiles List
                            // If this tile has no transparent pixels
                            if (isCompletelyOpaque)
                            {
                                // Remember that this tile has no transparent pixels
                                opaqueTilePositions.Add(thisTilePosition);
                            }
                            #endregion
                            #region Place this Tile
                            // Place this tile in the texture to update
                            textureToUpdate.SetPixels(initialPixelX, initialPixelY, levelMap.tilewidth, levelMap.tileheight, combinedTile);
                            #endregion
                        }
                    }
                }
            }
            // Apply these changes to the texture to update
            textureToUpdate.Apply();
        }
        #endregion
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

    // This finds the overlaping area of two rectangles and returns that as a rectangle
    private Rect GetOverlap(Rect firstRect, Rect secondRect)
    {
        // Create a rectangle to store the overlap
        Rect overlapRect = new Rect();

        // Determine where this rectangle starts
        overlapRect.x = (firstRect.xMin > secondRect.xMin) ? firstRect.xMin : secondRect.xMin;
        overlapRect.y = (firstRect.yMin > secondRect.yMin) ? firstRect.yMin : secondRect.yMin;

        // Determine how wide the rectangle should be
        overlapRect.width = ((firstRect.xMax < secondRect.xMax) ? firstRect.xMax : secondRect.xMax) - overlapRect.x;
        overlapRect.height = ((firstRect.yMax < secondRect.yMax) ? firstRect.yMax : secondRect.yMax) - overlapRect.y;

        // Return a rectangle containing the overlap
        return overlapRect;
    }

    // TODO: Remove this later
    private void LogTimeTakenForAction(string nameOfActionTaken)
    {
        float millisecondsScinceLastCheck = (Time.realtimeSinceStartup - timeOfLastCheck) * 1000;

        if (millisecondsScinceLastCheck > .095)
        {
            Debug.Log(nameOfActionTaken + ": " + ((Time.realtimeSinceStartup - timeOfLastCheck) * 1000).ToString() + " milliseconds");
            timeOfLastCheck = Time.realtimeSinceStartup;
        }
    }
}