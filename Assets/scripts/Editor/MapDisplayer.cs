using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class MapDisplayer : MonoBehaviour {
    // Refrence to the tile map importer script
    private TileMapImporter tileMapImporter;
    // REMOVE THIS LATER this is the sprite for the tile that will be displayed
    private Sprite tileToDisplay;
    // REMOVE THIS LATER this is the sprite renderer attached to this object
    private SpriteRenderer spriteRenderer;
    // REMOVE THIS LATER
    //private Dictionary<string, Sprite[]> tilesetsSprites = new Dictionary<string, Sprite[]>();



    void Awake()
    {
        // Setup refernces
        tileMapImporter = GetComponent<TileMapImporter>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }



    void Start()
    {
        Texture2D path = (Texture2D)AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/tilesets/environment.png");
        string spriteSheet = AssetDatabase.GetAssetPath(path);
        Sprite[] tempTilesetSprites = AssetDatabase.LoadAllAssetsAtPath(spriteSheet).OfType<Sprite>().ToArray();
        spriteRenderer.sprite = tempTilesetSprites[tileMapImporter.tileIDMap[0,0,0]];
        //spriteRenderer.sprite = tempTilesetSprites[tileMapImporter.tileIDMap[0][0][35]]; //2560
        //spriteRenderer.sprite = (Sprite)AssetDatabase.LoadAssetAtPath<Sprite>("Assets/tilesets/environment.png/1");
        //(Sprite)AssetDatabase.LoadAssetAtPath<Sprite>("Assets/tilesets/" + tileMapImporter.layersTileMap[2560]);
    }



	void Update()
    {
    }
}
