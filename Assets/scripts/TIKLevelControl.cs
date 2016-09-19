using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TIKLevelControl : MonoBehaviour
{
    // This is the text asset for this level's map
    public TextAsset tileMapTextAsset;
    // This is a copy of the tileMapTextAsset. It is used to determine if tileMapText asset has ben changed
    private TextAsset oldTileMapTextAsset;
    // This is an array of textures for each tileset
    public Texture2D[] tilesetTextures;
    // Reference to TIK's jsonUtilies class
    private TIKJsonUtilities jsonUtilities = new TIKJsonUtilities();
    // This is the TIKMap for this level
    public TIKMap levelMap;


    // When TIKLevelControlEditor calls this function variables are checked and updated
    public bool UpdateGUI()
    {
        if (oldTileMapTextAsset == null) // If tileMapTextAsset has not been changed before
        {
            // Update oldTileMapTextAsset for use for next time the GUI is changed
            oldTileMapTextAsset = tileMapTextAsset;
            // Create a new TIKMap based on the new textAsset file
            levelMap = jsonUtilities.CreateTIKMapFromTextAsset(tileMapTextAsset);
            // Make tilesetTextures have enough slots for each tilest in this levelMap
            tilesetTextures = new Texture2D[levelMap.tilesets.Length];
            // Variables have been updated
            return true;
        }
        else if (oldTileMapTextAsset != tileMapTextAsset) // If the tileMapTextAsset changed
        {
            // Update oldTileMapTextAsset for use for next time the GUI is changed
            oldTileMapTextAsset = tileMapTextAsset;
            // Create a new TIKMap based on the new textAsset file
            levelMap = jsonUtilities.CreateTIKMapFromTextAsset(tileMapTextAsset);
            // Make tilesetTextures have enough slots for each tilest in this levelMap
            tilesetTextures = new Texture2D[levelMap.tilesets.Length];
            // Variables have been updated
            return true;
        }
        // Variables have been updated
        return true;
    }
}