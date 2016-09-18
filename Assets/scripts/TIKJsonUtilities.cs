using UnityEngine;
using System.Collections;

public class TIKJsonUtilities
{
    // When this is called it creates and returns a new TIKMap from a given TextAsset from a tiled map
    public TIKMap CreateTIKMapFromTextAsset(TextAsset textAssetToCreateMapFrom)
    {
        TIKMap tikMap = JsonUtility.FromJson<TIKMap>(textAssetToCreateMapFrom.ToString());
        tikMap.InitializeMap();
        return tikMap;
    }


}
