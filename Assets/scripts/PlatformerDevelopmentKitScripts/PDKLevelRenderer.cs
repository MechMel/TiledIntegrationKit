using UnityEngine;
using System.Collections;

public class PDKLevelRenderer : MonoBehaviour
{
    //The CombineSprites function combines an array of sprites, and returns one large one
    public Texture2D CombineTexture2Ds(Texture2D[] tile2D, int tileSize)
    {
        TIKMap TIKmap = new TIKMap();
        int mapWidth = TIKmap.GetDimension("width");
        int mapHeight = TIKmap.GetDimension("height");
        Texture2D newCombinedTexture = new Texture2D(mapWidth, mapHeight);

        //Here are the for-loops that create the newCombinedTexture
        for (int y = 0; y < mapHeight / tileSize; y++)
        {
            for (int x = 0; x < mapWidth / tileSize; x++)
            {

                //These for-loops loop through every pixel in each tile, and put them
                //into the finished newCombinedTexture
                for (int tilePixelY = y; tilePixelY < y + tileSize; tilePixelY++)
                {
                    for (int tilePixelX = x; tilePixelX < x + tileSize; tilePixelX++)
                    {
                        newCombinedTexture.SetPixel(x, y,
                        newCombinedTexture.GetPixel(tilePixelX, tilePixelY));
                    }
                }
            }
        }
        return newCombinedTexture;
    }
}
