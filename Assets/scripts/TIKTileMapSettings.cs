using UnityEngine;
using System.Collections;

public class TIKTileMapSettings : MonoBehaviour
{
    #region Data for this Level
    // This is the text asset for this level's map
    public TextAsset tileMapTextAsset;
    // This is the TIKMap for this level
    public TIKMap levelMap;
    // This is an array of textures for each tileset
    public Texture2D[] tilesetTextures;
    #endregion

    #region Tiled Map Vhange Variables
    // This is a copy of the tileMapTextAsset. It is used to determine if tileMapText asset has been changed
    private TextAsset oldTileMapTextAsset;
    // This is a copy of levelMap. It is used to see what has changed between this map and the new one
    public TIKMap oldLevelMap;
    // This is an array of textures for each tileset in the new TextAsset for this level's map
    public Texture2D[] newTilesetTextures;
    #endregion

    // This is a copy of the tilesetTextures. It is used to determine if tilesetTextures has been changed
    public Texture2D[] oldTilesetTextures;
    // Reference to TIK's jsonUtilies class
    private TIKJsonUtilities jsonUtilities = new TIKJsonUtilities();


    // When TIKLevelControlEditor calls this function variables are checked and updated
    public bool UpdateGUI()
    {
        if (oldTileMapTextAsset == null) // If tileMapTextAsset has not been changed before
        {
            // Update the nesecary variables for a tileMapTextAsset change
            tileMapTextAssetChanged();
            // Variables have been updated
            return true;
        }
        else if (oldTileMapTextAsset != tileMapTextAsset) // If the tileMapTextAsset changed
        {
            // Update the nesecary variables for a tileMapTextAsset change
            tileMapTextAssetChanged();
            // Variables have been updated
            return true;
        }
        // Variables have been updated
        return true;
    }


    // When this is called it updates the nessecary variable for when tileMapTextAsset has been changed
    private void tileMapTextAssetChanged()
    {
        // Remeber the last levelMap
        oldLevelMap = levelMap;
        // Create a new TIKMap based on the new textAsset file
        levelMap = jsonUtilities.CreateTIKMapFromTextAsset(tileMapTextAsset);
        // Make newTilesetTextures have enough slots for each tilest in the new map
        newTilesetTextures = new Texture2D[levelMap.tilesets.Length];
        // If tilesetTextures has been instatiated
        if (tilesetTextures != null)
        {
            // For each tileset in the new map
            for (int newTileset = 0; newTileset < newTilesetTextures.Length; newTileset++)
            {
                // Go through each tileset in the old map
                for (int oldTileset = 0; oldTileset < tilesetTextures.Length; oldTileset++)
                {
                    // If these tilesets have the same name
                    if (levelMap.tilesets[newTileset].name == oldLevelMap.tilesets[oldTileset].name)
                    {
                        // Then remember the texture that the user put in
                        newTilesetTextures[newTileset] = tilesetTextures[oldTileset];
                    }
                }
            }
        }
        // Update oldTileMapTextAsset for use for next time the GUI is changed
        oldTileMapTextAsset = tileMapTextAsset;
        // Update tilesetTextures to have the new tilesets
        tilesetTextures = newTilesetTextures;
    }


    void Awake()
    {
        // Assign each of this level's TIKMap's tileset's texture variable to each of the textures the user has put in
        for (int tileset = 0; tileset < tilesetTextures.Length; tileset++)
        {
            levelMap.tilesets[tileset].imageTexture = tilesetTextures[tileset];
        }
    }
}
