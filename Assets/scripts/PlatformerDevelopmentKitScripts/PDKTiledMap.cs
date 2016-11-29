using System;
using System.Xml;

[Serializable]
public class PDKTiledMap
{
    public float version;
    public string orientation;
    public string renderOrder;
    public int width;
    public int height;
    public int tilewidth;
    public int tileheight;
    public int nextobjectid;
    public PDKTiledLayer[] layers;
    public PDKTiledTileset[] tilesets;
    public PDKTiledCustomProperty[] properties;
    /* XML
    // When this is called it creates a new PDKmap from a given XmlDocument for a tiled map
    public TIKMap(XmlDocument mapXML)
    {
        // Create a temporary node list to put all tilesets in
        XmlNodeList allTilesetsXML = mapXML.GetElementsByTagName("tileset");
        // Determine the number of tilesets in this map, and define the allTilesets array
        allTilesets = new PDKTileset[allTilesetsXML.Count];
        // For each tileset in this map
        for (int currentTileset = 0; currentTileset < allTilesetsXML.Count; currentTileset++)
        {
            // Create a new tileset and add it to the allTilesets array
            allTilesets[currentTileset] = new TIKTileset(allTilesetsXML[currentTileset]);
        }
        // Create a temporary node list to put all layers in
        XmlNodeList allLayersXML = mapXML.GetElementsByTagName("layer");
        // Determine the number of layers in this map, and define the allLayers array
        allLayers = new PDKLayer[allLayersXML.Count];
        // For each layer in this map
        for (int currentLayer = 0; currentLayer < allLayersXML.Count; currentLayer++)
        {
            // Create a new tilLayer and add it to the allLayers array
            allLayers[currentLayer] = new PDKTileLayer(allLayersXML[currentLayer]);
        }
    }


    // When this is called it creates a new PDKmap from a given XmlDocument for a tiled map
    public PDKMap(TextAsset mapJson)
    {
        //
        orientation = mapJson.
        // Create a temporary node list to put all tilesets in
        XmlNodeList allTilesetsXML = mapJson.GetElementsByTagName("tileset");
        // Determine the number of tilesets in this map, and define the allTilesets array
        allTilesets = new PDKTileset[allTilesetsXML.Count];
        // For each tileset in this map
        for (int currentTileset = 0; currentTileset < allTilesetsXML.Count; currentTileset++)
        {
            // Create a new tileset and add it to the allTilesets array
            allTilesets[currentTileset] = new PDKTileset(allTilesetsXML[currentTileset]);
        }
        // Create a temporary node list to put all layers in
        XmlNodeList allLayersXML = mapJson.GetElementsByTagName("layer");
        // Determine the number of layers in this map, and define the allLayers array
        allLayers = new PDKLayer[allLayersXML.Count];
        // For each layer in this map
        for (int currentLayer = 0; currentLayer < allLayersXML.Count; currentLayer++)
        {
            // Create a new tilLayer and add it to the allLayers array
            allLayers[currentLayer] = new PDKTileLayer(allLayersXML[currentLayer]);
        }
    }
    */
}

public class PDKTiledLayer
{
    public string name;
    public string type;
    public int height;
    public int width;
    public bool visible;
    public int opacity;
    public int x;
    public int y;
    public PDKTiledCustomProperty[] properties;
    // Tile Layer Attributes
    public int[] data;
    // Object Layer Attributes
    public string draworder;
    public PDKTiledObject[] objects;
    // Image Layer Attributes
    public string image;
    /* XML
    // When this is called it creates a Layer from a given XmlNode for a layer
    public TIKLayer(XmlNode tileLayerXmlNode)
    {
        // Add all of the layer's attributes to this Layer
        name = tileLayerXmlNode.Attributes["name"].Value;
    }
    */
}

public class PDKTiledTileset
{
    public string name;
    public int tilewidth;
    public int tileheight;
    public int tilecount;
    public int columns;
    public int firstgid;
    public int margin;
    public int spacing;
    public string image;
    public int imagewidth;
    public int imageheight;
    public PDKTiledCustomProperty[] properties;
    public PDKTiledTileProperties[] tileproperties;
    /* XML
    // When this is called it creates a Tileset from a given XmlNode for a tileset
    public TIKTileset(XmlNode tilesetXmlNode)
    {

        // Add all of the tileset attributes to the new tileset
        firstgid = Int32.Parse(tilesetXmlNode.Attributes["firstgid"].Value);
        name = tilesetXmlNode.Attributes["name"].Value;
        tilewidth = Int32.Parse(tilesetXmlNode.Attributes["tilewidth"].Value);
        tileheight = Int32.Parse(tilesetXmlNode.Attributes["tileheight"].Value);
        tilecount = Int32.Parse(tilesetXmlNode.Attributes["tilecount"].Value);
        columns = Int32.Parse(tilesetXmlNode.Attributes["columns"].Value);

        // Add all of the Image attributes to the new tileset
        XmlNode imageNode = tilesetXmlNode.SelectSingleNode("//*[local-name() = 'image'][1]");
        image = imageNode.Attributes["source"].Value;
        imageTexture = (Texture2D)Resources.Load<Texture2D>(image.TrimStart(new char[] { '.' }).TrimStart(new char[] { '/' }).TrimEnd(allLetters).TrimEnd(new char[] { '.' }));
        FormatTexture2D(imageTexture); // Format this tileset's texture
        imagewidth = Int32.Parse(imageNode.Attributes["width"].Value);
        imageheight = Int32.Parse(imageNode.Attributes["height"].Value);

        // Add all of the custom properties to the new tileset
        // Declare this tileset's dictionary for custom properties
        customTilesetProperties = new Dictionary<string, string>();
        // Put all this tileset's custom properties into a tileset
        XmlNodeList customTilestProperties = tilesetXmlNode.SelectNodes("/tileset/Properties/Property");
        // For each of this tilest's custom properties
        foreach(XmlNode currentCustomProperty in customTilestProperties)
        {
            // Add the current custom property to this tileset's dictionary for custom properties
            customTilesetProperties.Add(currentCustomProperty.Attributes["name"].Value, currentCustomProperty.Attributes["value"].Value);
        }
        // Declare this tileset's dictionary for custom tiles and determine the number of custom tiles in this tileset
        customTiles = new Dictionary<string, string>[tilecount + firstgid - 1];
        // Put all this tileset's custom tiles into a node list
        XmlNodeList customTileXmlNodes = tilesetXmlNode.SelectNodes("/tileset/tile");
        // For each custom tile in this tileset
        foreach(XmlNode currentCustomTile in customTileXmlNodes)
        {
            // Create a dictionary to temporarily store a single custom property
            Dictionary<string, string> currentCustomTileProperties = new Dictionary<string, string>();
            // Put all of this tile's custom properties into a node list
            XmlNodeList customTilePropertyNodes = currentCustomTile.SelectNodes("/tile/properties/property");
            // For each of the custom properties on this tile
            foreach (XmlNode currentCustomProperty in customTilePropertyNodes)
            {
                // Put this custom properties into the temporary dictionary
                currentCustomTileProperties.Add(currentCustomProperty.Attributes["name"].Value, currentCustomProperty.Attributes["value"].Value);
            }
            // Put all of this tile's custom properties into the dictionary of custom tiles 
            customTiles[Int32.Parse(currentCustomTile.Attributes["name"].Value)] = currentCustomTileProperties;
        }
    }
    */
}

public class PDKTiledObject
{
    public string name;
    public string type;
    public int id;
    public int gid;
    public int x;
    public int y;
    public int width;
    public int height;
    public int rotation;
    public bool visible;
    public PDKTiledCustomProperty[] properties;
}

public class PDKTiledTileProperties
{
    public int tileid;
    public PDKTiledCustomProperty[] customproperties;
}

public class PDKTiledCustomProperty
{
    public string name;
    public string value;
}