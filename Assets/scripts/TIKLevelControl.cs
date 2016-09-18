using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TIKLevelControl : MonoBehaviour
{
    // This is the text asset for this level's tile map
    public TextAsset tileMapTextAsset;
    // This is the text asset for this Level's tile map before the user put a new text asset in
    private TextAsset oldTileMapTextAsset;
    // This is an array that will contain all the textures for this map's tilesets
    public Texture2D[] tilesetTextures;
    // This is an instance of TIK's JsonUtilites class
    private TIKJsonUtilities jsonUtilities = new TIKJsonUtilities();
    // This is the TIKMap for this level
    public TIKMap levelMap;


    // Check which variables have changed and set others accordingly
    public bool UpdateGUIVariables()
    {
        if (oldTileMapTextAsset == null) // If tileMapTextAsset has not been changed before
        {
            // After the nessecary variables are updated
            if (MapTextAssetChanged())
            {
                // Variables have been updated
                return true;
            }
        }
        else if (oldTileMapTextAsset != tileMapTextAsset) // If the tileMapTextAsset changed
        {
            // After the nessecary variables are updated
            if (MapTextAssetChanged())
            {
                // Variables have been updated
                return true;
            }
        }
        // No variables where changed
        return false;
    }


    // This is called when the map text asset is changed. This function updates the necessary variables
    public bool MapTextAssetChanged()
    {
        // The tile map TextAsset has changed, so remember the new tile map TextAsset
        oldTileMapTextAsset = tileMapTextAsset;
        // Create a new TIKMap from the tile map TextAsset
        levelMap = jsonUtilities.CreateTIKMapFromTextAsset(tileMapTextAsset);
        // Instatiate the tilesetTextures array
        tilesetTextures = new Texture2D[levelMap.tilesets.Length];
        // Variables have been updated
        return true;
    }
}
