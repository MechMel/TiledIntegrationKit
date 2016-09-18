using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TIKLevelControl : MonoBehaviour
{
    //
    public TextAsset tileMapTextAsset;
    //
    private TextAsset oldTileMapTextAsset;
    //
    public Texture2D[] tilesetTextures;
    //
    private TIKJsonUtilities jsonUtilities = new TIKJsonUtilities();
    //
    public TIKMap levelMap;


    //
    public bool UpdateGUI()
    {
        if (oldTileMapTextAsset == null) // If tileMapTextAsset has not been changed before
        {
            //
            oldTileMapTextAsset = tileMapTextAsset;
            //
            levelMap = jsonUtilities.CreateTIKMapFromTextAsset(tileMapTextAsset);
            //
            tilesetTextures = new Texture2D[levelMap.tilesets.Length];
            //
            return true;
        }
        else if (oldTileMapTextAsset != tileMapTextAsset) // If the tileMapTextAsset changed
        {
            //
            oldTileMapTextAsset = tileMapTextAsset;
            //
            levelMap = jsonUtilities.CreateTIKMapFromTextAsset(tileMapTextAsset);
            //
            tilesetTextures = new Texture2D[levelMap.tilesets.Length];
            // Variables have been set
            return true;
        }
        //
        return true;
    }
}