using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PDKLevelRenderer : MonoBehaviour
{
    // This is used to remember what area of the map is currently rendered
    public Rect renderedRectOfMap;
    // TODO: Fill this in later
    public Texture2D renderedTexture = null;
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

        LogTimeSinceLastCheck("Begin Compiling Layer Groups: ");
        // Go through each layer in this map
        for (int layerNumberToRender = mapToRender.layers.Length - 1; layerNumberToRender >= 0; layerNumberToRender--)
        {
            // If this layer is visible
            if (mapToRender.layers[layerNumberToRender].visible)
            {
                if (currentLayerGroupNumber < 0) // If no layer groups have been created yet
                {
                    // The first layer group should be created at position 0
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
        LogTimeSinceLastCheck("Complining Layer groups took: ");
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
            LogTimeSinceLastCheck("To create Layer Group objects it took: ");
            // Create a new texture
            Texture2D textureToRender = new Texture2D((int)rectangleToRender.width * mapToRender.tilewidth, (int)rectangleToRender.height * mapToRender.tileheight);

            // TODO: get sorting layers working
            //thisLayerSpriteRenderer.sortingLayerName = layerNumberToRender.ToString() + " " + mapToRender.layers[layerNumberToRender].name;
            // If the map has been renered once already
            if (renderedTexture != null)
            {
                // Create a texture from the given rectangle
                textureToRender = CreateTextureForRectOfLayerGroup(mapToRender, layerGroupsToRender[layerGroupNumber].layerNumbers, rectangleToRender);
                // Copy the parts of the map that have already been rendered
                //textureToRender = CreateTextureFromOverlap(mapToRender, renderedRectOfMap, rectangleToRender);
                //
                //textureToRender = FillInTextureOfOverlap(textureToRender, mapToRender, layerGroupsToRender[layerGroupNumber].layerNumbers, renderedRectOfMap, rectangleToRender);
            }
            else
            {
                // Create a texture from the given rectangle
                textureToRender = CreateTextureForRectOfLayerGroup(mapToRender, layerGroupsToRender[layerGroupNumber].layerNumbers, rectangleToRender);
            }
            LogTimeSinceLastCheck("To Create a texture for this Layer Group it took: ");
            // Store the newly created texture of the map
            renderedTexture = textureToRender;
            // Create a sprite from the texture to render
            Sprite spriteToDisplay = Sprite.Create(textureToRender, new Rect(0, 0, textureToRender.width, textureToRender.height), new Vector2(0.5f, 0.5f), mapToRender.tilewidth);
            // Display the sprite of the area to render
            layerGroupObjects[layerGroupNumber].GetComponent<SpriteRenderer>().sprite = spriteToDisplay;
            // Put this layer in the correct position
            layerGroupObjects[layerGroupNumber].transform.position = positionToCreateLayersAt;
            LogTimeSinceLastCheck("To apply the new layer gorup texture it took: ");
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

    // This finds the overlaping area of two rectangles
    private Texture2D CreateTextureFromOverlap(TIKMap mapToRender, Rect oldRect, Rect newRect)
    {
        // Determine the portion of the previously rendred section to copy
        Rect rectToCopy = GetOverlap(oldRect, newRect);
        // Create a rectangle to store the overlap
        Texture2D textureToRender = CreateTransparentTexture((int)newRect.width * mapToRender.tilewidth, (int)newRect.height * mapToRender.tileheight);

        if (rectToCopy.width > 0 && rectToCopy.height > 0)
        {
            //
            textureToRender.SetPixels(
                x: (int)((newRect.x - oldRect.x) * mapToRender.tilewidth),
                y: (int)((newRect.y - oldRect.y) * mapToRender.tileheight),
                blockWidth: (int)rectToCopy.width * mapToRender.tilewidth,
                blockHeight: (int)rectToCopy.height * mapToRender.tileheight,
                colors: renderedTexture.GetPixels(
                    x: (int)rectToCopy.x * mapToRender.tilewidth,
                    y: (int)rectToCopy.y * mapToRender.tileheight,
                    blockWidth: (int)rectToCopy.width * mapToRender.tilewidth,
                    blockHeight: (int)rectToCopy.height * mapToRender.tileheight));
        }
        // Return a rectangle containing the overlap
        return textureToRender;
    }

    //
    private Texture2D FillInTextureOfOverlap(Texture2D textureToFillIn, TIKMap mapToRender, List<int> layerGroupToRender, Rect oldRect, Rect newRect)
    {
        // These will store the area that needs to be rendred
        Rect[] rectsToRender = new Rect[4];

        //
        rectsToRender[0] = new Rect(
            x: newRect.x,
            y: oldRect.y,
            width: newRect.x - oldRect.x,
            height: oldRect.height);
        rectsToRender[1] = new Rect(
            x: oldRect.x + oldRect.width,
            y: oldRect.y,
            width: newRect.x + newRect.width - oldRect.x + oldRect.height,
            height: oldRect.height);
        rectsToRender[2] = new Rect(
            x: newRect.x,
            y: newRect.y,
            width: newRect.width,
            height: oldRect.y - newRect.y);
        rectsToRender[3] = new Rect(
            x: newRect.x,
            y: oldRect.y + oldRect.height,
            width: newRect.width,
            height: newRect.y + newRect.height - oldRect.y + oldRect.height);
        //
        for (int thisSideNumber = 0; thisSideNumber < 4; thisSideNumber++)
        {
            // Create a rect to hold the overlap between this rectangle an the area that needs to be rendered
            Rect overlap = GetOverlap(newRect, rectsToRender[thisSideNumber]);

            if (rectsToRender[thisSideNumber].width > 0 && rectsToRender[thisSideNumber].height > 0)
            {
                //
                Rect thisRectToRender = new Rect(overlap.x + rectsToRender[thisSideNumber].x, overlap.y + rectsToRender[thisSideNumber].y, overlap.width, overlap.height);

                if (overlap.width > 0 && overlap.height > 0)
                {
                    //
                    Texture2D textureToRenderForThisSide = CreateTextureForRectOfLayerGroup(mapToRender, layerGroupToRender, thisRectToRender);
                    //
                    textureToFillIn.SetPixels(
                        x: (int)overlap.x * mapToRender.tilewidth,
                        y: (int)overlap.y * mapToRender.tileheight,
                        blockWidth: (int)overlap.width * mapToRender.tilewidth,
                        blockHeight: (int)overlap.height * mapToRender.tileheight,
                        colors: textureToRenderForThisSide.GetPixels());
                }
            }
        }
        // Return the filled in texture
        return textureToFillIn;
    }

    // When this is called this function creates a texture from a given rectangle of the map
    private Texture2D CreateTextureForRectOfLayerGroup(TIKMap levelMap, List<int> layerNumbersToCreateTextureFrom, Rect rectangleOfLayerToRender)
    {
        // Create a new transparent texture to store the requested rectangle of the map
        Texture2D textureToReturn = CreateTransparentTexture((int)rectangleOfLayerToRender.width * levelMap.tilewidth, (int)rectangleOfLayerToRender.height * levelMap.tileheight);
        // Set the texture filter mode to point
        textureToReturn.filterMode = FilterMode.Point;
        List<int> positionsWithTiles = new List<int>();

        // Start at the closest layer
        layerNumbersToCreateTextureFrom.Reverse();
        // For each layer to render
        foreach (int layerNumber in layerNumbersToCreateTextureFrom)
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
                    // Calculate The x position of the pixel that this tile will start at
                    int initialPixelX = levelMap.tilewidth * (thisTilePosition % (int)rectangleOfLayerToRender.width);
                    // Calculate The y position of the pixel that this tile will start at
                    int initialPixelY = levelMap.tileheight * (((int)(rectangleOfLayerToRender.height * rectangleOfLayerToRender.width) - thisTilePosition - 1) / (int)rectangleOfLayerToRender.width);
                    // If there is no tile already at this position
                    if (!positionsWithTiles.Contains(thisTilePosition))
                    {
                        // Place this tile in the texture to return
                        textureToReturn.SetPixels(initialPixelX, initialPixelY, levelMap.tilewidth, levelMap.tileheight, thisTilesPixels);
                        // Remember that there is tile at this position
                        positionsWithTiles.Add(thisTilePosition);
                    }
                    else
                    {
                        // For each pixel in this tile
                        for (int pixelPosition = 0; pixelPosition < thisTilesPixels.Length; pixelPosition++)
                        {
                            // Calculate the x poxiiton of this pixel in the texture to return
                            int pixelX = initialPixelX + (pixelPosition % levelMap.tilewidth);
                            // Calculate the y poxiiton of this pixel in the texture to return
                            int pixelY = initialPixelY + (pixelPosition / levelMap.tilewidth);

                            if (textureToReturn.GetPixel(pixelX, pixelY).a == 0) // If the old tile is transparent
                            {
                                // Just place this pixel on top
                                textureToReturn.SetPixel(pixelX, pixelY, thisTilesPixels[pixelPosition]);
                            }
                            else // If this pixel will be placed behind another one
                            {
                                // Calculate the new combined color for this pixel in the texture to return
                                Color newPixelColor = (textureToReturn.GetPixel(pixelX, pixelY) * (1 - thisTilesPixels[pixelPosition].a)) + (thisTilesPixels[pixelPosition] * thisTilesPixels[pixelPosition].a);

                                // Set this pixel to the new compined color
                                textureToReturn.SetPixel(pixelX, pixelY, newPixelColor);
                            }
                        }
                    }
                }
            }
            // Apply the changes to the texture
            textureToReturn.Apply();
        }

        // Return the completed texture
        return textureToReturn;
    }

    // When this is called it creates and returns a transparent texture from a given width and height
    private Texture2D CreateTransparentTexture(int width, int height)
    {
        // Create a new texture
        Texture2D textureToReturn = new Texture2D(width, height);
        // Calculate how many pixels there are in the texture to return
        int numberOfPixelsInTextureToReturn = textureToReturn.width * textureToReturn.height;
        // Create a new white pixel
        Color transparentPixel = Color.white;

        // Make this pixel transparent
        transparentPixel.a = 0;

        // For each pixel in the texture to return
        for (int pixelNumber = 0; pixelNumber < numberOfPixelsInTextureToReturn; pixelNumber++)
        {
            // Make each pixel in the texture to retun transparent
            textureToReturn.SetPixel(pixelNumber % textureToReturn.width, pixelNumber / textureToReturn.width, transparentPixel);
        }
        // Apply the changes to this texture
        textureToReturn.Apply();

        // Return this completed texture
        return textureToReturn;
    }

    // This finds the overlaping area of two rectangles
    private Rect GetOverlap(Rect oldRect, Rect newRect)
    {
        // Create a rectangle to store the overlap
        Rect rectToReturn = new Rect();
        // Find the x and y offset between the two rectangles
        int xOffset = (int)oldRect.x - (int)newRect.x;
        int yOffset = (int)oldRect.y - (int)newRect.y;

        // Determine where this rectangle starts
        rectToReturn.x = (xOffset > 0) ? xOffset : 0;
        rectToReturn.y = (yOffset > 0) ? yOffset : 0;
        // Determine how wide the rectangle should be
        rectToReturn.width = oldRect.width - Mathf.Abs(xOffset);
        rectToReturn.height = oldRect.height - Mathf.Abs(yOffset);
        // Return a rectangle containing the overlap
        return rectToReturn;
    }


    private void LogTimeSinceLastCheck(string nameOfStep)
    {
        Debug.Log(nameOfStep + (Time.time - timeOfLastCheck).ToString());
        timeOfLastCheck = (float)Time.time;
        return;
    }
}

