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
    public PDKTileLayer tileLayer;
    //private UnityEngine.Object tileMapTMX;
    public TextAsset tileMapTextAsset;
    //private SpriteRenderer ender;
    //private XmlDocument tileMapXML = new XmlDocument();
    private TIKMap map;

    private Texture2D[] tileTextures;
    private SpriteRenderer sprRend;


    void Awake()
    {
        //ender = this.GetComponent<SpriteRenderer>();
        //AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(tileMapTMX), tileMapTMX.name.TrimEnd('t', 'm', 'x') + "xml");
        //AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(tileMapTMX), ImportAssetOptions.ForceUpdate);
        //File.Move("Assets/tilemaps/basic_test_level.tmx", Path.ChangeExtension("Assets/tilemaps/basic_test_level.tmx", ".xml"));
        //tileMapText = (TextAsset)AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/tilemaps/basic_test_level.tmx");
        //tileMapText = (TextAsset)AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/tilemaps/" + tileMapTMX.name);
        // Create all necessary dictionaryies for this tilemap
        //CreateDictionaries();
    }
    
    private void CreateDictionaries()
    {
        //tileMapXML.LoadXml(tileMapTXT.text);
        //tileMapXML.Load(AssetDatabase.GetAssetPath(tileMapTMX));
        //tileMapTMX.
        //LoadFromResource
        //tileMapXML = tileMapTMX as XmlDocument;
        //tileMapTXT = (TextAsset)Resources.Load(tileMapTMX.name + ".tmx");
        //Debug.Log(tileMapTXT.text);
        //Debug.Log(tileMapTMX.name);
        /*if (tileMapTXT == null)
        {
            Debug.Log("out");
        }
        else
        {
            Debug.Log("in");
        }

        //tileMapXML.LoadXml(tileMapText.text);

        map = new PDKMap(tileMapXML);
        tileset = map.allTilesets[0];
        if (map.allLayers[0] as PDKTileLayer != null)
        {
            tileLayer = map.allLayers[0] as PDKTileLayer;
        }*/
    }


    void Start()
    {
        sprRend = GetComponent<SpriteRenderer>();
        TIKJsonUtilities tIKJsonUtilitiesInstance = new TIKJsonUtilities();
        TIKMap exampleMap = tIKJsonUtilitiesInstance.CreateTIKMapFromTextAsset(tileMapTextAsset);
        Debug.Log(exampleMap.tilesets[0].tilecount);
        //ender.sprite = tileset.GetTileSprite(tileLayer.getTileIDFromCorrdinate(4, 32)); //Sprite.Create(tileset.GetTile(0), new Rect(16, 16, 32, 32), new Vector2(.5f, .5f), 16);

        for(int a = 0; a < map.layers[0].data.Count; a++)
        {
            tileTextures[a] = tileset.GetTileTexture2D(a);
        }
        sprRend.sprite = Sprite.Create(TIKrend.CombineTexture2Ds(tileTextures, map.GetDimension("width")), 
        new Rect(0,0,map.GetDimension("width"),map.GetDimension("height")), new Vector2(0.5f, 0.5f));
    }
}
