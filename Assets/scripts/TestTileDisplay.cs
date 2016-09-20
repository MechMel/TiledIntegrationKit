using UnityEngine;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class TestTileDisplay : MonoBehaviour
{
    public TIKLayer tileLayers;
    public TIKTileset tileset;
    public TIKRenderer TIKrend;
    public TextAsset tileMapTextAsset;
    private TIKMap testMap;

    private Texture2D[] tileTextures;
    private SpriteRenderer sprRend;


    void Start()
    {
        sprRend = GetComponent<SpriteRenderer>();
        //testMap = GameObject.Find("GameObject").GetComponent<TIKLevelControl>().levelMap;
        tileTextures = new Texture2D[testMap.layers[0].data.Count];

        for (int a = 0; a < testMap.layers[0].data.Count; a++)
        {
            tileTextures[a] = tileset.GetTileTexture2D(a);
        }
        sprRend.sprite = Sprite.Create(TIKrend.CombineTexture2Ds(tileTextures, testMap.GetDimension("width")), 
        new Rect(0,0,testMap.GetDimension("width"),testMap.GetDimension("height")), new Vector2(0.5f, 0.5f));
    }
}
