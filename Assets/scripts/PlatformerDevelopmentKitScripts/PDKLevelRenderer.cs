using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PDKLevelRenderer : MonoBehaviour
{
    //
    public List<GameObject> layerObjects = new List<GameObject>();

    //
    public void RemoveAllLayers()
    {
        //
        foreach (GameObject thisLayerObject in layerObjects)
        {
            GameObject.Destroy(thisLayerObject);
        }
    }

    // When this is called a sprite for each layer is created and a given rectangle of the map rendered on the appropriate layers
    public void RenderRectangleOfMapAtPosition(TIKMap mapToRender, Rect rectangleToRender, Vector3 positionToCreateLayersAt)
    {
        // Go through each layer in this map
        for (int layerNumberToRender = mapToRender.layers.Length - 1; layerNumberToRender >= 0; layerNumberToRender--)
        {
            // If this layer is a tile layer
            if (mapToRender.layers[layerNumberToRender].layerType == TIKLayer.layerTypes.Tile && mapToRender.layers[layerNumberToRender].visible)
            {
                // Create a game object to render this layer on
                GameObject thisLayerObject = new GameObject();
                // Name this layer's object
                thisLayerObject.name = "Layer " + layerNumberToRender.ToString() + " " + mapToRender.layers[layerNumberToRender].name;
                // Add a sprite renderer to the game object for layer 1
                SpriteRenderer thisLayerSpriteRenderer = thisLayerObject.AddComponent<SpriteRenderer>();
                //
                thisLayerObject.transform.position = positionToCreateLayersAt;
                // TODO: get sorting layers working
                //thisLayerSpriteRenderer.sortingLayerName = layerNumberToRender.ToString() + " " + mapToRender.layers[layerNumberToRender].name;
                // Create a texture from the given rectangle
                Texture2D textureToRender = CreateTextureFromASectionOfALayer(mapToRender, layerNumberToRender, rectangleToRender);
                // Create a sprite from the texture to render
                Sprite spriteToDisplay = Sprite.Create(textureToRender, new Rect(0, 0, textureToRender.width, textureToRender.height), new Vector2(0.5f, 0.5f), mapToRender.tilewidth);
                // Display the sprite of the area to render
                thisLayerSpriteRenderer.GetComponent<SpriteRenderer>().sprite = spriteToDisplay;
                // Add the newly created object for this layer into the list of layer objects
                layerObjects.Add(thisLayerObject);
            }
        }
    }

    // When this is called this function creates a texture from a given rectangle of the map
    private Texture2D CreateTextureFromASectionOfALayer(TIKMap levelMap, int layerToRender, Rect rectangleOfLayerToRender)
    {
        // Create a new transparent texture to store the requested rectangle of the map
        Texture2D textureToReturn = CreateTransparentTexture((int)rectangleOfLayerToRender.width * levelMap.tilewidth, (int)rectangleOfLayerToRender.height * levelMap.tileheight);
        // Set the texture filter mode to point
        textureToReturn.filterMode = FilterMode.Point;
        // Create a dictionary to store each tile ID and the positions of these IDs in the requested rectangle of the map
        Dictionary<int, List<int>> tilePositions = levelMap.GetAllTilePositionsFromLayerInRectangle(layerToRender, rectangleOfLayerToRender);

        // Go through each tile ID in the requested rectangle of the map
        foreach (int thisTileID in tilePositions.Keys)
        {
            // Create an array of colors and put the pixels from this tile into it
            Color[] thisTilesPixels = levelMap.GetTilePixels(thisTileID);
            // Go through each occurance of this tile in the requested rectangle of the map
            foreach(int tilePosition in tilePositions[thisTileID])
            {
                // Draw that tile in the corect position in the texture to return 
                textureToReturn.SetPixels(
                    x: levelMap.tilewidth * (tilePosition % (int)rectangleOfLayerToRender.width), 
                    y: levelMap.tileheight * (((int)(rectangleOfLayerToRender.height * rectangleOfLayerToRender.width) - tilePosition - 1) / (int)rectangleOfLayerToRender.width),
                    blockWidth: levelMap.tilewidth,
                    blockHeight: levelMap.tileheight, 
                    colors: thisTilesPixels);
            }
        }
        // Apply the changes to the texture
        textureToReturn.Apply();

        // Return the completed texture
        return textureToReturn;
    }

    // When this is called it creates and returns a transparent texture from a given width and height
    private Texture2D CreateTransparentTexture(int width, int height)
    {
        // Create a new texture
        Texture2D textureToReturn = new Texture2D(width, height);
        // Create an array with enough pixels for the new texture
        Color[] transparentPixels = new Color[width * height];

        // For each pixel in transparentPixels
        for (int pixelNumber = 0; pixelNumber < transparentPixels.Length; pixelNumber++)
        {
            // Make this pixel white
            transparentPixels[pixelNumber] = Color.white;
            // Make this pixel transparent
            transparentPixels[pixelNumber].a = 0;
        }
        // Make this texture transparent
        textureToReturn.SetPixels(transparentPixels);
        // Apply the changes to this texture
        textureToReturn.Apply();

        // Return this completed texture
        return textureToReturn;
    }
}
