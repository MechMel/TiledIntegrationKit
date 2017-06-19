using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PDKLevelRenderer
{
    // Stores the TIKMap that this instance of PDKLevelRenderer will Use
    public PDKMap levelMap;
    // This is used to remember what area of the map is currently loaded
    public Rect loadedRectOfMap;
    // TODO: Remove this later
    private float timeOfLastCheck = 0;
    


    // Creates a new instance of a PDKLevelRenderer
    public PDKLevelRenderer(PDKMap mapToUse)
    {
        // Get the map to use
        levelMap = mapToUse;
        // Initizlse the rendered rect of the map
        loadedRectOfMap = new Rect(-1, -1, 0, 0);
        #region Setup Layer Group objects
        // Go through each layer group in this map
        for (int layerGroupIndex = 0; layerGroupIndex < mapToUse.layerGroups.Count; layerGroupIndex++)
        {
            // Create a game object to render this layer group on
            levelMap.layerGroups[layerGroupIndex].layerGroupObject = new GameObject();
            // Name this layer's object
            levelMap.layerGroups[layerGroupIndex].layerGroupObject.name = "Layer Group: " + layerGroupIndex.ToString();
            // Add a sprite renderer to this layer group's game object
            levelMap.layerGroups[layerGroupIndex].layerGroupObject.AddComponent<SpriteRenderer>();
            // Create an appropriately sized texture
            levelMap.layerGroups[layerGroupIndex].layerGroupTexture = new Texture2D(1, 1);
            // Set the filter mode
            levelMap.layerGroups[layerGroupIndex].layerGroupTexture.filterMode = FilterMode.Point;
            // Apply these changes
            levelMap.layerGroups[layerGroupIndex].layerGroupTexture.Apply();
        }
        #endregion
    }


    // This adjust each layer group's object so that is is at a new given positon rendering the correct portion of the map
    public void LoadRectOfMap(Rect rectToLoad)
    {
        // Update each layer group
        for (int indexOfLayerGroupToUpdate = 0; indexOfLayerGroupToUpdate < levelMap.layerGroups.Count; indexOfLayerGroupToUpdate++)
        {
            //Create a variable to more easily refrence the layer group to update
            PDKLayerGroup layerGroupToUpdate = levelMap.layerGroups[indexOfLayerGroupToUpdate];
            // If the layer group to update is a tileLayerGroup
            if (layerGroupToUpdate.groupType == PDKLayer.layerTypes.Tile)
            {
                // Update this layer group
                UpdateTileLayerGroup(layerGroupToUpdate, rectToLoad);
            }
            // If the layer group to update is an objectLayerGroup
            else if (layerGroupToUpdate.groupType == PDKLayer.layerTypes.Object)
            {
                // Update this object group
                UpdateObjectGroup(layerGroupToUpdate, rectToLoad);
            }
        }
        // Store the curretly loaded rectangle of the map
        loadedRectOfMap = rectToLoad;
    }

    #region TileLayerGroup Rendering
    // This updates a given tile Layer Group's texture, and then moves the layer group the apropriate position
    public void UpdateTileLayerGroup(PDKLayerGroup layerGroupToUpdate, Rect rectToRender)
    {
        // Update the texture for this layer group
        UpdateTextureForRectOfLayerGroup(ref layerGroupToUpdate.layerGroupTexture, layerGroupToUpdate, rectToRender);
        // Create a sprite from this layer group's newly created texture
        Sprite spriteToDisplay = Sprite.Create(
            texture: layerGroupToUpdate.layerGroupTexture,
            rect: new Rect(
                x: 0,
                y: 0,
                width: layerGroupToUpdate.layerGroupTexture.width,
                height: layerGroupToUpdate.layerGroupTexture.height),
            pivot: new Vector2(
                x: 0.5f,
                y: 0.5f),
            pixelsPerUnit: levelMap.tileWidth);
        // Display the newly created sprite for this layergroup
        layerGroupToUpdate.layerGroupObject.GetComponent<SpriteRenderer>().sprite = spriteToDisplay;
        // Move this layer group's object to the correct position
        layerGroupToUpdate.layerGroupObject.transform.position = new Vector3(
                x: (int)rectToRender.x + ((int)rectToRender.width / 2),
                y: -(int)rectToRender.y - ((int)rectToRender.height / 2),
                z: layerGroupToUpdate.zPosition);
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
                    x: (int)(overlapRect.xMin - loadedRectOfMap.xMin) * levelMap.tileWidth,
                    y: (int)(loadedRectOfMap.height - (overlapRect.yMax - loadedRectOfMap.yMin)) * levelMap.tileHeight,
                    blockWidth: (int)overlapRect.width * levelMap.tileWidth,
                    blockHeight: (int)overlapRect.height * levelMap.tileHeight);
        }
        // If the rect to render has diffrent dimmensions then the current texture
        if (rectToRender.width * levelMap.tileWidth != textureToUpdate.width || rectToRender.height * levelMap.tileHeight != textureToUpdate.height)
        {
            // Adjust the size of the texture to update
            textureToUpdate = new Texture2D((int)rectToRender.width * levelMap.tileWidth, (int)rectToRender.height * levelMap.tileHeight);
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
                x: (int)(overlapRect.xMin - rectToRender.xMin) * levelMap.tileWidth,
                y: (int)(rectToRender.height - (overlapRect.yMax - rectToRender.yMin)) * levelMap.tileHeight,
                blockWidth: (int)overlapRect.width * levelMap.tileWidth,
                blockHeight: (int)overlapRect.height * levelMap.tileHeight);
        }
        #endregion
        #region Find Tile Positions to Render
        #region SUMMARY:
        /*
        To optimize rendering, find the tiles that are inside the rect to render and outside the currently loaded rect of the map
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
                    tilePositionsToRender.Add(((Mathf.Abs(y) % levelMap.height) * levelMap.width) + (Mathf.Abs(x) % levelMap.width));
                }
            }
            // For each row inside the rect to render and beside the overlap
            for (int y = (int)overlapRect.yMin; y < overlapRect.yMax; y++)
            {
                // For each tile on the left of the overlap
                for (int x = (int)rectToRender.xMin; x < overlapRect.xMin; x++)
                {
                    // Add the position of this tile to the list of positions to render
                    tilePositionsToRender.Add(((Mathf.Abs(y) % levelMap.height) * levelMap.width) + (Mathf.Abs(x) % levelMap.width));
                }
                // For each tile on the right of the overlap
                for (int x = (int)overlapRect.xMax; x < rectToRender.xMax; x++)
                {
                    // Add the position of this tile to the list of positions to render
                    tilePositionsToRender.Add(((Mathf.Abs(y) % levelMap.height) * levelMap.width) + (Mathf.Abs(x) % levelMap.width));
                }
            }
            // For each row inside the rect to render and below the overlap
            for (int y = (int)overlapRect.yMax; y < rectToRender.yMax; y++)
            {
                // For each tile in this row
                for (int x = (int)rectToRender.xMin; x < rectToRender.xMax; x++)
                {
                    // Add the position of this tile to the list of positions to render
                    tilePositionsToRender.Add(((Mathf.Abs(y) % levelMap.height) * levelMap.width) + (Mathf.Abs(x) % levelMap.width));
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
                    tilePositionsToRender.Add(((Mathf.Abs(y) % levelMap.height) * levelMap.width) + (Mathf.Abs(x) % levelMap.width));
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
            tilePositions = levelMap.GetTilesByRasterList(layerNumber, tilePositionsToRender);
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
                        // Calculate The x position of the pixel that this tile will start at
                        int initialPixelX = levelMap.tileWidth * ((thisTilePosition % levelMap.width) - (int)rectToRender.x);
                        // TODO: FIND A BETTER WAY TO DO THIS
                        if (initialPixelX < 0)
                        {
                            initialPixelX = levelMap.tileWidth * ((thisTilePosition % levelMap.width) - (int)rectToRender.x + levelMap.width);
                        }
                        // Calculate The y position of the pixel that this tile will start at
                        int initialPixelY = levelMap.tileHeight * (levelMap.height - ((thisTilePosition / levelMap.width) - (int)rectToRender.y + 1));
                        // TODO: FIND A BETTER WAY TO DO THIS
                        if (initialPixelY < 0)
                        {
                            initialPixelY = levelMap.tileHeight * (levelMap.height - ((thisTilePosition / levelMap.width) - (int)rectToRender.y + 1) + levelMap.height); 
                        }

                        /*int initialPixelX;
                        int initialPixelY;
                        // Calculate The y position of the pixel that this tile will start at
                        if ((thisTilePosition % levelMap.width) > (int)rectToRender.x) // TODO: FILL THIS IN LATER
                        {
                            initialPixelX = (thisTilePosition % levelMap.width) - (int)rectToRender.x;
                        }
                        else
                        {
                            initialPixelX = (thisTilePosition % levelMap.width) - (int)rectToRender.x + levelMap.width;
                        }
                        // Calculate The y position of the pixel that this tile will start at
                        if ((thisTilePosition / levelMap.width) > (int)rectToRender.y) // TODO: FILL THIS IN LATER
                        {
                            initialPixelY = (thisTilePosition / levelMap.width) - (int)rectToRender.y;
                        }
                        else
                        {
                            initialPixelY = (thisTilePosition / levelMap.width) - (int)rectToRender.y + levelMap.height;
                        }*/
                        #endregion
                        #region Place this Tile
                        // If there is no tile already at this position
                        if (!positionsWithTiles.Contains(thisTilePosition))
                        {
                            // Place this tile in the texture to return
                            textureToUpdate.SetPixels(initialPixelX, initialPixelY, levelMap.tileWidth, levelMap.tileHeight, thisTilesPixels);
                            // Remember that there is tile at this position
                            positionsWithTiles.Add(thisTilePosition);
                        }
                        else // If there is a tile already at this position
                        {
                            #region Get the Old Pixels / Setup isCompletelyOpaque
                            // Get the old stack of tiles
                            combinedTile = textureToUpdate.GetPixels(initialPixelX, initialPixelY, levelMap.tileWidth, levelMap.tileHeight);
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
                            textureToUpdate.SetPixels(initialPixelX, initialPixelY, levelMap.tileWidth, levelMap.tileHeight, combinedTile);
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
    #endregion

    #region ObjectLayerGroupRendering
    // This updates a given layer group, such that objects that should be hydrated are, and all others are dehydrated
    private void UpdateObjectGroup(PDKLayerGroup groupToUpdate, Rect rectToLoad)
    {
        /* For each object layer in this layer group, dehydrate objects out side of the given rect
        and hydrate objects, within the given rect, that are not hydrated but should be */
        foreach (int indexOfObjectLayerToUpdate in groupToUpdate.layerNumbers)
        {
            // Create a variable to more easily refrence the object layer that is currently being updated
            PDKLayer objectLayerToUpdate = levelMap.layers[indexOfObjectLayerToUpdate];
            // This will be used to store all the objects that need to be hydrated
            PDKLayer.PDKObjectIDHashSet objectIDsToHydrate;

            // Dehydrate any objects outside of the rect to load
            objectLayerToUpdate.DehydrateExternalObjects(rectToLoad);
            // Take all dehydrated objects in the rect to render, from the dehydrated object map
            objectIDsToHydrate = objectLayerToUpdate.TakeDehydratedObjectIDsInRect(rectToLoad);
            // Hydrate each of the pdk objects, in this layer, that are not hydrated but should be
            foreach (int objectIDToHydrate in objectIDsToHydrate)
            {
                // TODO: Find a better way to do this later
                if (levelMap.layers[indexOfObjectLayerToUpdate].objects[objectIDToHydrate].prefab != null)
                {
                    // Create a hydrated copy of the current dehydrated pdk object
                    GameObject currentHydratedObject = objectLayerToUpdate.HydrateObject(objectIDToHydrate);
                    // Add the newly hydrated game object to the list of hydrated game objects
                    objectLayerToUpdate.hydratedObjects.Add((currentHydratedObject));
                }
            }
        }
    }
    #endregion

    // This examines two rectangles and finds the rectangular area of overlap between them
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