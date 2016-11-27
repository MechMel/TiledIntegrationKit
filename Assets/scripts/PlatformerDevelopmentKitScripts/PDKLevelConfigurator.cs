using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PDKLevelConfigurator : MonoBehaviour
{
    // This is how many tiles out from the camera to load this level
    [SerializeField]
    public int bufferDistance;
    // This is all possible map types a user can choose
    public enum mapTypes { None, Tiled };
    // This will be used to track the map type that the user has chosen
    [SerializeField]
    public mapTypes mapType = mapTypes.None;
    // This contains all oneway tiles in this map
    [SerializeField]
    public int[] oneWayTiles;
    // This contains all completely tiles in this map
    [SerializeField]
    public int[] solidTiles;

    #region Tiled Map Settings
    //
    private PDKMapSettings newMapSettings;
    // 
    [SerializeField]
    public PDKMapSettings mapSettings = new PDKMapSettings(null);
    #endregion

    // This is called to update variables when the text asset has been changed
    public void TextAssetChanged()
    {
        if (mapSettings.mapTextAsset == null) // If the current map has been removed
        {
            // Clear all current settings
            mapSettings = new PDKMapSettings(mapSettings.mapTextAsset);
            // Clear all remebered settings
            newMapSettings.Clone(mapSettings);
        }
        else // If a new text asset has been put in
        {
            // Create a new set of map settings from the new TextAsset
            newMapSettings = new PDKMapSettings(mapSettings.mapTextAsset);
            // Copy over any settings that match between the current and the new TIKMapSettings
            newMapSettings.CopyMatchingSettings(mapSettings);
            // Display the new map settings
            mapSettings.Clone(newMapSettings);
        }
    }

    void Awake()
    {
        // If a TIKMap has been created
        if (mapSettings.pdkMap != null && mapSettings.mapTextAsset != null)
        {
            // This stores the prefabs for each object type
            Dictionary<string, UnityEngine.Object> objects = new Dictionary<string, Object>();

            #region Compile Object Dictionary
            // Go through each object
            for (int thisObjectIndex = 0; thisObjectIndex < mapSettings.objectTypes.Count; thisObjectIndex++)
            {
                // Add this object to the dictionary of objects
                objects.Add(mapSettings.objectTypes[thisObjectIndex], mapSettings.objectPrefabs[thisObjectIndex]);
            }
            #endregion
            // Tell this level's map to initialize
            mapSettings.pdkMap.InitializeMap(mapSettings.tilesetTextures, objects);
            // Add the level controller to this object
            PDKLevelController levelController = this.gameObject.AddComponent<PDKLevelController>();
            // Give the TIKMap with the user's settings to the levelController
            levelController.levelMap = mapSettings.pdkMap;
            // Tell the level controller how close the camera can get to the edge of the loaded area before a new section of the level should be loaded
            levelController.bufferDistance = bufferDistance;
            //
            levelController.levelRenderer = new PDKLevelRenderer(mapSettings.pdkMap);
            // Disable this script
            //this.gameObject.GetComponent<PDKLevelConfigurator>().enabled = false;
        }
    }


    //
    private List<Vector2>[] CalculateCollisions(int[] data)
    {
        // This will store the solid tiles
        HashSet<int> solidTilesHashSet = new HashSet<int>();
        // This will store the collision data
        List<Vector2>[] collisionData = new List<Vector2>[data.Length];

        // For each solid tile
        foreach (int thisTileID in solidTiles)
        {
            // Add this tile to the solid tiles hashset
            solidTilesHashSet.Add(thisTileID);
        }
        // For each tile
        for (int thisTileIndex = 0; thisTileIndex < data.Length; thisTileIndex++)
        {
            bool[] collisionSides = new bool[4];
            //
            Vector2[] thisSideCollider;

            if (mapSettings.pdkMap.width % thisTileIndex == 0) // If this tile is on the left edge of the map
            {
                // The left side of this tile should have a collider
                collisionSides[1] = true;
                // If this tile is sold, but the next tile is not
                if (solidTilesHashSet.Contains(data[thisTileIndex]) && !solidTilesHashSet.Contains(data[thisTileIndex + 1]))
                {
                    // The right side of this tile should have a collider
                    collisionSides[3] = true;
                }
            }
            else if  (mapSettings.pdkMap.width % (thisTileIndex + 1) == 0) // If this tile is on the right edge of the map
            {
                // The right side of this tile should have a collider
                collisionSides[3] = true;
                // If this tile is sold, but the last tile is not
                if (solidTilesHashSet.Contains(data[thisTileIndex]) && !solidTilesHashSet.Contains(data[thisTileIndex - 1]))
                {
                    // The left side of this tile should have a collider
                    collisionSides[1] = true;
                }
            }
            else if (solidTilesHashSet.Contains(data[thisTileIndex])) // If this tile is solid
            {
                // If the last tile is not solid
                if (!solidTilesHashSet.Contains(data[thisTileIndex - 1]))
                {
                    // The left side of this tile should have a collider
                    collisionSides[1] = true;
                }
                // If the next tile is not solid
                if (!solidTilesHashSet.Contains(data[thisTileIndex + 1]))
                {
                    // The right side of this tile should have a collider
                    collisionSides[3] = true;
                }
            }

            if (thisTileIndex < mapSettings.pdkMap.width) // If this tile is on the top edge of the map
            {
                // The top side of this tile should have a collider
                collisionSides[0] = true;
                // If this tile is sold, but tile below is not
                if (solidTilesHashSet.Contains(data[thisTileIndex]) && !solidTilesHashSet.Contains(data[thisTileIndex + mapSettings.pdkMap.width]))
                {
                    // The bottom side of this tile should have a collider
                    collisionSides[2] = true;
                }
            }
            else if (thisTileIndex >= data.Length - mapSettings.pdkMap.width) // If this tile is on the bottom edge of the map
            {
                // The bottom side of this tile should have a collider
                collisionSides[2] = true;
                // If this tile is sold, but the above tile is not
                if (solidTilesHashSet.Contains(data[thisTileIndex]) && !solidTilesHashSet.Contains(data[thisTileIndex - mapSettings.pdkMap.width]))
                {
                    // The top side of this tile should have a collider
                    collisionSides[0] = true;
                }
            }
            else if (solidTilesHashSet.Contains(data[thisTileIndex])) // If this tile is solid
            {
                // If the below tile is not solid
                if (!solidTilesHashSet.Contains(data[thisTileIndex - mapSettings.pdkMap.width]))
                {
                    // The bottom side of this tile should have a collider
                    collisionSides[2] = true;
                }
                // If the above tile is not solid
                if (!solidTilesHashSet.Contains(data[thisTileIndex + mapSettings.pdkMap.width]))
                {
                    // The top side of this tile should have a collider
                    collisionSides[0] = true;
                }
            }
            // Go through each side
            for (int thisSide = 0; thisSide < collisionSides.Length; thisSide++)
            {
                // If this side should have a collider but the previous side should not
                if (collisionSides[thisSide] && !collisionSides[(thisSide + 3) % 4])
                {
                    collisionData[thisTileIndex] = new List<Vector2>();
                    switch (thisSide)
                    {
                        case 0:
                            collisionData[thisTileIndex].Add(new Vector2(.5f, .5f));
                            collisionData[thisTileIndex].Add(new Vector2(-.5f, .5f));
                            break;
                        case 01:
                            collisionData[thisTileIndex].Add(new Vector2(-.5f, .5f));
                            collisionData[thisTileIndex].Add(new Vector2(-.5f, -.5f));
                            break;
                        case 2:
                            collisionData[thisTileIndex].Add(new Vector2(-.5f, -.5f));
                            collisionData[thisTileIndex].Add(new Vector2(.5f, -.5f));
                            break;
                        case 3:
                            collisionData[thisTileIndex].Add(new Vector2(.5f, -.5f));
                            collisionData[thisTileIndex].Add(new Vector2(.5f, .5f));
                            break;
                    }
                    // Go through each other side
                    for (int thisOtherSide = thisSide; thisOtherSide % collisionSides.Length != thisSide; thisOtherSide++)
                    {
                        // If this other side should have a collider
                        if (collisionSides[thisOtherSide % collisionSides.Length])
                        {
                            // 

                        }
                    }
                }
            }
        }
        return collisionData;
    }
}
