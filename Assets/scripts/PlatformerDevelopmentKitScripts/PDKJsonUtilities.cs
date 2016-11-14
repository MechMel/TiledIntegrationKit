using UnityEngine;
using System.Collections;

public class PDKJsonUtilities
{
    // When this is called it creates and returns a new TIKMap from a given TextAsset from a tiled map
    public PDKMap CreatePDKMapFromTextAsset(TextAsset textAssetToCreateMapFrom)
    {
        // Create a TIKMap based on the map's Json file
        PDKMap pdkMap = JsonUtility.FromJson<PDKMap>(textAssetToCreateMapFrom.ToString());
        // Return the newly created TIKMap
        return pdkMap;
    }


}
