using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PDKLevelRenderer : MonoBehaviour
{
    // This is used to remember what area of the map is currently rendered
    public Rect renderedRectOfMap;
    // This is the texture that is being put to gether so that it can be rendered
    public Texture2D textureToRender;
    // This is the texture that is currently rendered
    public Texture2D renderedTexture;
    // This is used to store all layer group objects that this renderer creates
    public Dictionary<int, GameObject> layerGroupObjects = new Dictionary<int, GameObject>();

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
            //
            textureToRender = new Texture2D((int)rectangleToRender.width * mapToRender.tilewidth, (int)rectangleToRender.height * mapToRender.tileheight);

            // TODO: get sorting layers working
            //thisLayerSpriteRenderer.sortingLayerName = layerNumberToRender.ToString() + " " + mapToRender.layers[layerNumberToRender].name;
            //
            Rect rectangleToRenderTopLeft = new Rect(rectangleToRender.x - (rectangleToRender.width / 2), rectangleToRender.y + (rectangleToRender.height / 2), rectangleToRender.width, rectangleToRender.height);
            // Create a texture from the given rectangle
            textureToRender = CreateTextureForRectOfLayerGroup(mapToRender, layerGroupsToRender[layerGroupNumber].layerNumbers, rectangleToRender);
            // Create a sprite from the texture to render
            Sprite spriteToDisplay = Sprite.Create(textureToRender, new Rect(0, 0, textureToRender.width, textureToRender.height), new Vector2(0.5f, 0.5f), mapToRender.tilewidth);
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
    private Texture2D CreateTextureFromASectionOfASetOfLayers(TIKMap levelMap, List<int> layerNumbersToCreateTextureFrom, Rect rectangleOfLayerToRender)
    {
        // Create a new transparent texture to store the requested rectangle of the map
        Texture2D textureToReturn = CreateTransparentTexture((int)rectangleOfLayerToRender.width * levelMap.tilewidth, (int)rectangleOfLayerToRender.height * levelMap.tileheight);
        // Set the texture filter mode to point
        textureToReturn.filterMode = FilterMode.Point;

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
                foreach (int tilePosition in tilePositions[thisTileID])
                {
                    // Calculate The x position of the pixel that this tile will start at
                    int initialPixelX = levelMap.tilewidth * (tilePosition % (int)rectangleOfLayerToRender.width);
                    // Calculate The y position of the pixel that this tile will start at
                    int initialPixelY = levelMap.tileheight * (((int)(rectangleOfLayerToRender.height * rectangleOfLayerToRender.width) - tilePosition - 1) / (int)rectangleOfLayerToRender.width);
                    // For each pixel in this tile
                    for (int pixelPosition = 0; pixelPosition < thisTilesPixels.Length; pixelPosition++)
                    {
                        // Calculate the x poxiiton of this pixel in the texture to return
                        int pixelX = initialPixelX + (pixelPosition % levelMap.tilewidth);
                        // Calculate the y poxiiton of this pixel in the texture to return
                        int pixelY = initialPixelY + (pixelPosition / levelMap.tilewidth);
                        // Calculate the new combined color for this pixel in the texture to return
                        Color newPixelColor = (thisTilesPixels[pixelPosition] * (1 - textureToReturn.GetPixel(pixelX, pixelY).a)) + (textureToReturn.GetPixel(pixelX, pixelY) * textureToReturn.GetPixel(pixelX, pixelY).a);
                        // Set this pixel to the new compined color
                        textureToReturn.SetPixel(pixelX, pixelY, newPixelColor);
                    }
                }
            }
            // Apply the changes to the texture
            textureToReturn.Apply();
        }

        // Return the completed texture
        return textureToReturn;
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
}