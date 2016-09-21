using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PDKLevelRenderer : MonoBehaviour
{
    //
    public void RenderRectangleOfMap(TIKMap mapToRender, Rect rectangleToRender)
    {
        //TODO: impliment multiple layer rendering
        // Create a game object to render this layer on
        GameObject layer1Object = new GameObject();
        layer1Object.name = "Layer_1";
        // Add a sprite renderer to the game object for layer 1
        SpriteRenderer layerOneSpriteRenderer = layer1Object.AddComponent<SpriteRenderer>();
        // Create a texture from the given rectangle
        Texture2D textureToRender = CreateTextureFromASectionOfALayer(mapToRender, 0, rectangleToRender);
        // Create a sprite from the texture to render
        Sprite spriteToDisplay = Sprite.Create(textureToRender, new Rect(0, 0, textureToRender.width, textureToRender.height), new Vector2(0.5f, 0.5f), mapToRender.tilewidth);
        // Display the sprite of the area to render
        layerOneSpriteRenderer.GetComponent<SpriteRenderer>().sprite = spriteToDisplay;
    }

    // When this is called this function creates a texture from a given rectangle of the map
    private Texture2D CreateTextureFromASectionOfALayer(TIKMap levelMap, int layerToRender, Rect rectangleOfLayerToRender)
    {
        // Create a texture to store the requested rectangle of the map
        Texture2D textureToReturn = new Texture2D((int)rectangleOfLayerToRender.width * levelMap.tilewidth, (int)rectangleOfLayerToRender.height * levelMap.tileheight);
        // Create a dictionary to store each tile ID and the positions of these IDs in the requested rectangle of the map
        Dictionary<int, List<int>> tilePositions = levelMap.GetAllTilePositionsFromLayerInRectangle(layerToRender, rectangleOfLayerToRender);

        // Go through each tile ID in the requested rectangle of the map
        foreach (int thisTileID in tilePositions.Keys)
        {
            // Create an array of colors and put the pixels from this tile into it
            Color[] thisTilesPixels = levelMap.GetTileTexture(thisTileID).GetPixels(0, 0, levelMap.tilewidth, levelMap.tileheight);
            // Go through each occurance of this tile in the requested rectangle of the map
            foreach(int tilePosition in tilePositions[thisTileID])
            {
                // Draw that tile in the corect position in the texture to return 
                textureToReturn.SetPixels(tilePosition % (int)rectangleOfLayerToRender.width, tilePosition / (int)rectangleOfLayerToRender.width, levelMap.tilewidth, levelMap.tileheight, thisTilesPixels);
                // Apply the changes to the texture
                textureToReturn.Apply();
            }
        }

        // Return the completed texture
        return textureToReturn;
    }

    //The CombineSprites function combines an array of sprites, and returns one large one
    /*private Texture2D RenderLayer(TIKMap levelMap, int layerToRender)
    {
        //
        //
        Texture2D newCombinedTexture = new Texture2D(levelMap.width * levelMap.tilewidth, levelMap.height * levelMap.tileheight);

        //Here are the for-loops that create the newCombinedTexture
        for (int y = 1; y <= levelMap.height; y++)
        {
            for (int x = 1; x <= levelMap.width; x++)
            {

                //These for-loops loop through every pixel in each tile, and put them
                //into the finished newCombinedTexture
                for (int tilePixelY = 1; tilePixelY <= levelMap.tileheight; tilePixelY++)
                {
                    for (int tilePixelX = 1; tilePixelX <= levelMap.tilewidth; tilePixelX++)
                    {
                        Debug.Log(string.Format("level x:{0}, level y:{1}, pixel x:{2}, pixel y:{3}", x, y, tilePixelX, tilePixelY));
                        newCombinedTexture.SetPixel(x, y, levelMap.GetAllTilesTexturesFromLayer(layerToRender)[0].GetPixel(tilePixelX, tilePixelY));
                    }
                }
            }
        }
        return newCombinedTexture;
    }*/
}
