using UnityEngine;
using UnityEditor;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Xml;

public class TileMapImporter : MonoBehaviour
{
    // This is used to determne which tileset is currently being stored in the dictionary
    private int currentTileset = 0;
    // This is used to determne which layer is currently being stored in the dictionary
    private int currentLayer = 0;

    // This is the tile map to load
    public TextAsset tileMapText;
    // This will contain all the attributes for this tilemap
    public Dictionary<string, string> tileMapData = new Dictionary<string, string>();
    // This will contain all the data for all the tilesets
    public Dictionary<int, Dictionary<string, string>> tilesetsData;
    // This will contain all the data for all the layers
    public Dictionary<int, Dictionary<string, string>> layersData;
    // This will contain the tile placment for all the layers
    public Dictionary<int, Dictionary<int, Sprite[]>> tileAtPosition;
    // This will contain all the tiles for all this map
    public List<Sprite> tilesetSprites;
    // FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER
    public int[,,] tileIDMap;
    // FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER
    //private Dictionary<int, Dictionary<int, Sprite[]>> tileSpriteMap = new Dictionary<int, Dictionary<int, Sprite[]>>();



    void Awake()
    {
        // Slice all necessary tilesets
        SliceTilesets();
        // Create all necessary dictionaryies for this tilemap
        CreateDictionaries();
    }



    // When this is called all tilesets are sliced into individual, appropriatle named tiles
    private void SliceTilesets()
    {
        Texture2D myTexture = (Texture2D)AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/tilesets/environment.png");

        string path = AssetDatabase.GetAssetPath(myTexture);
        TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
        ti.isReadable = true;

        ti.spritePixelsPerUnit = 16;
        ti.filterMode = FilterMode.Point;
        ti.spriteImportMode = SpriteImportMode.Multiple;

        List<SpriteMetaData> newData = new List<SpriteMetaData>();

        int SliceWidth = 16;
        int SliceHeight = 16;

        for (int i = 0; i < myTexture.width; i += SliceWidth)
        {
            for (int j = myTexture.height; j > 0; j -= SliceHeight)
            {
                SpriteMetaData smd = new SpriteMetaData();
                smd.pivot = new Vector2(0.5f, 0.5f);
                smd.alignment = 9;
                smd.name = (((myTexture.height - j) / SliceHeight) * (myTexture.width / SliceWidth) + (i / SliceWidth) + 1).ToString();
                smd.rect = new Rect(i, j - SliceHeight, SliceWidth, SliceHeight);
                newData.Add(smd);
            }
        }

        ti.spritesheet = newData.ToArray();
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
    }



    // When this is called dictionaries, containing data for this tilemap, are created
    private void CreateDictionaries()
    {
        #region Create the tileMapData Dictionary
        //
        XmlDocument tileMapXML = new XmlDocument();
        // Load the tilemap for this level
        tileMapXML.LoadXml(tileMapText.text);
        XmlNodeList tileMapNodeList = tileMapXML.GetElementsByTagName("map");
        //
        foreach (XmlNode tileMapNode in tileMapNodeList)
        {
            // Add the tile map attributes to the dictionary
            tileMapData.Add("version", tileMapNode.Attributes["version"].Value);
            tileMapData.Add("orientation", tileMapNode.Attributes["orientation"].Value);
            tileMapData.Add("renderorder", tileMapNode.Attributes["renderorder"].Value);
            tileMapData.Add("width", tileMapNode.Attributes["width"].Value);
            tileMapData.Add("height", tileMapNode.Attributes["height"].Value);
            tileMapData.Add("tilewidth", tileMapNode.Attributes["tilewidth"].Value);
            tileMapData.Add("tileheight", tileMapNode.Attributes["tileheight"].Value);
            tileMapData.Add("nextobjectid", tileMapNode.Attributes["nextobjectid"].Value);
        }
        #endregion

        #region Create The tilesetsData Dictionary
        //  This will contain all the tilesets in this tilemap
        XmlNodeList tilesetList = tileMapXML.GetElementsByTagName("tileset");
        // All the data about each tileset will be put into this dictionary
        tilesetsData = new Dictionary<int, Dictionary<string, string>>();

        foreach (XmlNode currentTilesetData in tilesetList)
        {
            // Create a temporary dictionary to put data about his tileset into
            Dictionary<string, string> tempTilesetAttributes = new Dictionary<string, string>();
            // Put all data about this tileset into the temerary dictionary
            tempTilesetAttributes.Add("firstgid", currentTilesetData.Attributes["firstgid"].Value);
            tempTilesetAttributes.Add("name", currentTilesetData.Attributes["name"].Value);
            tempTilesetAttributes.Add("tilewidth", currentTilesetData.Attributes["tilewidth"].Value);
            tempTilesetAttributes.Add("tileheight", currentTilesetData.Attributes["tileheight"].Value);
            tempTilesetAttributes.Add("tilecount", currentTilesetData.Attributes["tilecount"].Value);
            tempTilesetAttributes.Add("columns", currentTilesetData.Attributes["columns"].Value);
            tempTilesetAttributes.Add("source", currentTilesetData.FirstChild.Attributes["source"].Value);
            tempTilesetAttributes.Add("width", currentTilesetData.FirstChild.Attributes["width"].Value);
            tempTilesetAttributes.Add("height", currentTilesetData.FirstChild.Attributes["height"].Value);
            // Add all this data into the tilesets data dictionary
            tilesetsData.Add(currentTileset, tempTilesetAttributes);
            // Incriment the current tileset variable
            currentTileset += 1;
        }
        #endregion

        #region Create The layersData Dictionary
        //  This will contain all the layers in this tilemap
        XmlNodeList layerList = tileMapXML.GetElementsByTagName("layer");
        // All the data about each tileset will be put into this dictionary
        layersData = new Dictionary<int, Dictionary<string, string>>();

        foreach (XmlNode tilesetData in layerList)
        {
            // Create a temporary dictionary to put data about his layer into
            Dictionary<string, string> tempLayerAttributes = new Dictionary<string, string>();
            // Put all data about this layer into the temerary dictionary
            tempLayerAttributes.Add("name", tilesetData.Attributes["name"].Value);
            tempLayerAttributes.Add("width", tilesetData.Attributes["width"].Value);
            tempLayerAttributes.Add("height", tilesetData.Attributes["height"].Value);
            tempLayerAttributes.Add("encoding", tilesetData.FirstChild.Attributes["encoding"].Value);
            tempLayerAttributes.Add("map", tilesetData.FirstChild.InnerText);
            // Add all this data into the layers data dictionary
            layersData.Add(currentLayer, tempLayerAttributes);
            // Incriment the current layer variable
            currentLayer += 1;
        }
        #endregion

        #region Create The layersTileMap Dictionary

        // FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER
        tileIDMap = new int[Int32.Parse(layersData[0]["width"]) - 1, Int32.Parse(layersData[0]["height"]) - 1, tileMapXML.GetElementsByTagName("layer").Count - 1];
        // FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER
        for (int thisLayer = 0; thisLayer < tileMapXML.GetElementsByTagName("layer").Count; thisLayer++)
        {
            // FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER
            string[] tempLayerTileMap = layerList[thisLayer].InnerText.Split(',');
            // FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER
            for (int thisTile = 0; thisTile < tempLayerTileMap.Length; thisTile ++)
            {
                // FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER
                tileIDMap[thisTile / Int32.Parse(layersData[thisLayer]["width"]), thisTile % Int32.Parse(layersData[thisLayer]["width"]), thisLayer] = Int32.Parse(tempLayerTileMap[thisTile]);
            }
        }


        /*
        // FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER
        List<int[]> iDMapYColumns = new List<int[]>();
        // FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER
        List<string[]> tileIDsStringList = new List<string[]>();
        // FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER
        List<int> layerTileIDList = new List<int>();
        // FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER
        List<List<int>> layersTileIDList = new List<List<int>>();
        // FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER
        List<int> yTileIDs = new List<int>();
        // FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER tilesetSprites
        List<List<int>> xYTileIDs = new List<List<int>>();
        // FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER
        for (int numberOfCurrentLayer = 0; numberOfCurrentLayer < layersData.Count; numberOfCurrentLayer++)
        {
            Texture2D path = (Texture2D)AssetDatabase.LoadAssetAtPath<Texture2D>(tilesetsData[0]["Source"]);
            string tileSetPath = AssetDatabase.GetAssetPath(path);
            List<Sprite> tempTilesetSprites = new List<Sprite>(AssetDatabase.LoadAllAssetsAtPath(tileSetPath).OfType<Sprite>().ToArray());
            // FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER
            tileIDsStringList.Add(layersData[numberOfCurrentLayer]["map"].Split(','));
            // FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER
            foreach (string tileIDString in tileIDsStringList[numberOfCurrentLayer])
            {
                // FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER
                layerTileIDList.Add(Int32.Parse(tileIDString));
            }
            // FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER
            layersTileIDList.Add(layerTileIDList);
            // FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER
            for (int x = 0; x < Int32.Parse(layersData[numberOfCurrentLayer]["width"]); x++)
            {
                // FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER
                for (int y = 0; y < Int32.Parse(layersData[numberOfCurrentLayer]["height"]); y++)
                {
                    yTileIDs.Add(layersTileIDList[numberOfCurrentLayer][(y * Int32.Parse(layersData[numberOfCurrentLayer]["width"])) + x]);
                }
                // FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER
                xYTileIDs.Add(yTileIDs);
            }
            // FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER : FILL THIS IN LATER
            tileIDMap.Add(xYTileIDs);
        }
        Debug.Log((tileIDMap[0][0][0]).ToString());

        XmlNodeList layerListTileMap = tileMap.GetElementsByTagName("data");
        foreach (XmlNode tilesetData in layerListTileMap)
        {
            // Create a temporary dictionary to put this layer's tile map into
            List<int> tempLayersTileMap = new List<int>();
            List<Sprite> tempLayersSpriteTileMap = new List<Sprite>();
            //
            string[] tempLayerTileMap = tilesetData.FirstChild.InnerText.Split(',');

            Texture2D path = (Texture2D)AssetDatabase.LoadAssetAtPath<Texture2D>(tilesetsData[0]["Source"]);
            string spriteSheet = AssetDatabase.GetAssetPath(path);
            List<Sprite> tempTilesetSprites = new List<Sprite>(AssetDatabase.LoadAllAssetsAtPath(spriteSheet).OfType<Sprite>().ToArray());
            //
            foreach (string valueOfCurrentTile in tempLayerTileMap)
            {
                // Put all data about this layer's tile map into the temerary dictionary
                tempLayersTileMap.Add(Int32.Parse(valueOfCurrentTile));
            }
            foreach (int valueOfCurrentTile in tempLayersTileMap)
            {
                // Put all data about this layer's tile map into the temerary dictionary
                tempLayersSpriteTileMap.Add(tempTilesetSprites[valueOfCurrentTile]);
            }
            // Add all this data into the layers tile map dictionary
            tileAtPosition = tempLayersTileMap;
        }*/
        #endregion
    }



    //
    public Sprite[] GetSpriteArrayFromPosition(int xPosition, int yPosition)
    {
        return null;
    }
}
