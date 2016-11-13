using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PDKLevelRenderer : MonoBehaviour
{
    // These store the textures that are currently rendered for each layer group
    public Texture2D[] layerGroupTextures;
    // Stores the TIKMap that this instance of PDKLevelRenderer will Use
    public PDKMap levelMap;
    // This is used to remember what area of the map is currently loaded
    public Rect loadedRectOfMap;
    // This is used to store all layer group objects that this renderer creates
    public List<GameObject> layerGroupObjects = new List<GameObject>();
    // TODO: Remove this later
    private float timeOfLastCheck = 0;
    


    // Creates a new instance of a PDKLevelRenderer
    public PDKLevelRenderer(PDKMap mapToUse)
    {
        // Get the map to use
        levelMap = mapToUse;
        // Set the layer group textures to the appropriate size
        layerGroupTextures = new Texture2D[levelMap.layerGroups.Count];
        // Initizlse the rendered rect of the map
        loadedRectOfMap = new Rect(0, 0, 1, 1);
        #region Setup Layer Group objects
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
        #endregion
    }


    // This adjust each layer group's object so that is is at a new given positon rendering the correct portion of the map
    public void LoadRectOfMap(Rect rectToLoad)
    {
        // Go through each layer group in the level map
        for (int thisLayerGroupIndex = 0; thisLayerGroupIndex < levelMap.layerGroups.Count; thisLayerGroupIndex++)
        {
            // If this layer group is made up of tile layers
            if (levelMap.layerGroups[thisLayerGroupIndex].groupType == PDKLayer.layerTypes.Tile)
            {
                // Update this layer group
                UpdateTileLayerGroup(thisLayerGroupIndex, rectToLoad);
            }
        }
        // Store the curretly loaded rectangle of the map
        loadedRectOfMap = rectToLoad;
    }


    // This updates a given tile Layer Group's texture, and then moves the layer group the apropriate position
    public void UpdateTileLayerGroup(int layerGroupIndex, Rect rectToRender)
    {
            // Update the texture for this layer group
            UpdateTextureForRectOfLayerGroup(ref layerGroupTextures[layerGroupIndex], levelMap.layerGroups[layerGroupIndex], rectToRender);
            // Create a sprite from this layer group's newly created texture
            Sprite spriteToDisplay = Sprite.Create(
                texture: layerGroupTextures[layerGroupIndex],
                rect: new Rect(
                    x: 0,
                    y: 0,
                    width: layerGroupTextures[layerGroupIndex].width,
                    height: layerGroupTextures[layerGroupIndex].height),
                pivot: new Vector2(
                    x: 0.5f,
                    y: 0.5f),
                pixelsPerUnit: levelMap.tilewidth);
            // Display the newly created sprite for this layergroup
            layerGroupObjects[layerGroupIndex].GetComponent<SpriteRenderer>().sprite = spriteToDisplay;
            // Move this layer group's object to the correct position
            layerGroupObjects[layerGroupIndex].transform.position = new Vector3(
                x: (int)rectToRender.x + ((int)rectToRender.width / 2),
                y: -(int)rectToRender.y + ((int)rectToRender.height / 2),
                z: -layerGroupIndex);
    }


    // Alters a given layer group's texture based on the rectangle that is being rendered
    private void UpdateTextureForRectOfLayerGroup(ref Texture2D textureToUpdate, PDKLayerGroup givenLayerGroup, Rect rectToRender)
    {
        #region Overlap Variables
        // This stores the overlap between the rendered rect and the rect to render
        Rect overlapRect = GetOverlap(loadedRectOfMap, rectToRender);
        // This stores the colors in the overlaping area between the renderd rect and the rect to render
        Color[] overlapColors = new Color[0];
        #endregion
        #region Tile Position Variables
        // This is used to store which positions already have tiles at them
        List<int> positionsWithTiles = new List<int>();
        // This is used to stroe which positions have no transparent pixels at them
        List<int> opaqueTilePositions = new List<int>();
        // This stores the positions of tiles that will be rendered
        List<int> tilePositionsToRender = new List<int>();
        // This will store each tile ID and the positions of these IDs for all tiles outside of the overlap
        Dictionary<int, List<int>> tilePositions;
        #endregion
        #region Tile Placement Variables
        // This will store the pixels for each tile
        Color[] thisTilesPixels;
        // This will store the old stack of tiles when one tile is placed ontop of another
        Color[] combinedTile;
        // This will be used to determine when there are no more transparent pixels in each tile
        bool isCompletelyOpaque;
        #endregion

        #region Copy Overlap and Update Texture
        #region SUMMARY:
        /*
        If the currently loaded rectangle of the map overlaps with the rect to render
            then record all pixels that are in that overlap from this layer group's current texture.
        If the rect to render has diffrent dimensions than the currently loaded rectangle of the map
            then create a new texture with the appropriate dimensions, for the rect to render
            and replace this layer group's texture with this new one.
        Paste all the pixels that were in that overlap, onto this layer group's texture.
        */
        #endregion
        // If there is an overlap
        if (overlapRect.width > 0 && overlapRect.height > 0)
        {
            // Get the pixels from the old area
            overlapColors = textureToUpdate.GetPixels(
                    x: (int)(overlapRect.xMin - loadedRectOfMap.xMin) * levelMap.tilewidth,
                    y: (int)(loadedRectOfMap.height - (overlapRect.yMax - loadedRectOfMap.yMin)) * levelMap.tileheight,
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
        #region Find Tile Positions to Render
        #region SUMMARY:
        /*
        To optimize rendering, find the tiles that are outside the currently loaded rect of the map and inside the rect to render
        If the rect to render overlaps with the currently loaded rect
            add the position of each tile above, beside, and below the overlap to the list of tile positions to render.
        If the rect to render does not overlap with the currently loaded rect
            add the position of all tiles in the rect to render to the list of tile positions to render.
        */
        #endregion
        // If there is an overlap
        if (overlapRect.width > 0 && overlapRect.height > 0)
        {
            // For each row inside the rect to render and above the overlap
            for (int y = (int)rectToRender.yMin; y < overlapRect.yMin; y++)
            {
                // For each tile in this row
                for (int x = (int)rectToRender.xMin; x < rectToRender.xMax; x++)
                {
                    // Add the position of this tile to the list of positions to render
                    tilePositionsToRender.Add((y * levelMap.width) + x);
                }
            }
            // For each row inside the rect to render and beside the overlap
            for (int y = (int)overlapRect.yMin; y < overlapRect.yMax; y++)
            {
                // For each tile on the left of the overlap
                for (int x = (int)rectToRender.xMin; x < overlapRect.xMin; x++)
                {
                    // Add the position of this tile to the list of positions to render
                    tilePositionsToRender.Add((y * levelMap.width) + x);
                }
                // For each tile on the right of the overlap
                for (int x = (int)overlapRect.xMax; x < rectToRender.xMax; x++)
                {
                    // Add the position of this tile to the list of positions to render
                    tilePositionsToRender.Add((y * levelMap.width) + x);
                }
            }
            // For each row inside the rect to render and below the overlap
            for (int y = (int)overlapRect.yMax; y < rectToRender.yMax; y++)
            {
                // For each tile in this row
                for (int x = (int)rectToRender.xMin; x < rectToRender.xMax; x++)
                {
                    // Add the position of this tile to the list of positions to render
                    tilePositionsToRender.Add((y * levelMap.width) + x);
                }
            }
        }
        else // If there is no overlap
        {
            // For each row inside the rect to render
            for (int y = (int)rectToRender.yMin; y < rectToRender.yMax; y++)
            {
                // For each tile in this row
                for (int x = (int)rectToRender.xMin; x < rectToRender.xMax; x++)
                {
                    // Add the position of this tile to the list of positions to render
                    tilePositionsToRender.Add((y * levelMap.width) + x);
                }
            }
        }
        #endregion;
        #region Render the Outside Tiles
        #region SUMMARY:
        /*
        To optimize rendering, find the tiles that are outside the currently loaded rect of the map and inside the rect to render
        If the rect to render overlaps with the currently loaded rect
            add the position of each tile above, beside, and below the overlap to the list of tile positions to render.
        If the rect to render does not overlap with the currently loaded rect
            add the position of all tiles in the rect to render to the list of tile positions to render.
        */
        #endregion
        // For each layer to render
        foreach (int layerNumber in givenLayerGroup.layerNumbers)
        {
            #region Get all Tiles To Render
            // Get each tile ID and the positions of these IDs for all tiles outside of the overlap
            tilePositions = levelMap.GetAllTilePositionsFromLayerInList(layerNumber, tilePositionsToRender);
            #endregion
            #region Put Each Tile to Render on the Texture to Update
            // Go through each tile ID in the tile positions to render
            foreach (int thisTileID in tilePositions.Keys)
            {
                // Get the pixels for this tile
                thisTilesPixels = levelMap.GetTilePixels(thisTileID);
                #region Place this Tile at Each of Its Positions
                // Go through each occurance of this tile in the requested rectangle of the map
                foreach (int thisTilePosition in tilePositions[thisTileID])
                {
                    // If this tile is not already completely opaque
                    if (!opaqueTilePositions.Contains(thisTilePosition))
                    {
                        #region Get the X and Y Positons For this Tile on the Textue to  Update
                        // Translate the global position of this tile to a local position
                        int thisLocalTilePosition = ((thisTilePosition % levelMap.width) - (int)rectToRender.x) + (((thisTilePosition / levelMap.width) - (int)rectToRender.y) * (int)rectToRender.width);
                        // Calculate The x position of the pixel that this tile will start at
                        int initialPixelX = levelMap.tilewidth * (thisLocalTilePosition % (int)rectToRender.width);
                        // Calculate The y position of the pixel that this tile will start at
                        int initialPixelY = levelMap.tileheight * (((int)(rectToRender.height * rectToRender.width) - (thisLocalTilePosition + 1)) / (int)rectToRender.width);
                        #endregion
                        #region Place this Tile
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
                            // Place this tile in the texture to update
                            textureToUpdate.SetPixels(initialPixelX, initialPixelY, levelMap.tilewidth, levelMap.tileheight, combinedTile);
                        }
                        #endregion
                    }
                }
                #endregion
            }
            #endregion
            // Apply these changes to the texture to update
            textureToUpdate.Apply();
        }
        #endregion
        // The texture has been updated
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