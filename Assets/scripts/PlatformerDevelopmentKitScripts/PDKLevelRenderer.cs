using UnityEngine;
using System.Collections;

public class PDKLevelRenderer : MonoBehaviour
{
    //
    public void RenderLevel(TIKMap mapToRender)
    {
        // 
        GameObject layerOneObject = new GameObject();
        //
        SpriteRenderer layerOneSpriteRenderer = layerOneObject.AddComponent<SpriteRenderer>();
        //
        layerOneSpriteRenderer.sprite = Sprite.Create(RenderLayer(mapToRender, 0), new Rect(0, 0, mapToRender.width * mapToRender.tilewidth, mapToRender.height * mapToRender.tileheight), new Vector2(0.5f, 0.5f));
    }

    //The CombineSprites function combines an array of sprites, and returns one large one
    private Texture2D RenderLayer(TIKMap levelMap, int layerToRender)
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
    }
}
