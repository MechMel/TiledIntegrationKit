using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PDKLevelConfigurator : MonoBehaviour
{
    public enum mapTypes { None, Tiled }; // This is all possible map types a user can choose
    public int bufferDistance; // This is how many tiles out from the camera to load this level
    public TextAsset mapTextAsset; // The text asset that will be used to create this map
    public mapTypes mapType = mapTypes.None; // This will be used to track the map type that the user has chosen
    public PDKMap pdkMap; // This is the PDKMap for this level




    // This is called to update variables when the text asset has been changed
    public void TextAssetChanged()
    {
        if (mapTextAsset == null) // If the current map has been removed
        {
            // Clear all current settings
            pdkMap = CreateNewPDKMap(mapTextAsset);
        }
        else // If a new text asset has been put in
        {
            // Create a new map from the new TextAsset
            PDKMap newPDKMap = CreateNewPDKMap(mapTextAsset);
            // Copy over any properties that match between the current and the new Map
            CopyMatchingProperties(ref newPDKMap, pdkMap);
            // Update the PDKMap for this level
            pdkMap = newPDKMap;
        }
    }


    // When this is called a new PDKMap is created based on a given TextAsset
    public PDKMap CreateNewPDKMap(TextAsset newMapTextAsset)
    {
        // Create a new instance of PDKTiledUtilites
        PDKTiledUtilities pdkTiledUtilities = new PDKTiledUtilities();
        // Stores the new newPDKMap
        PDKMap newPDKMap;

        // If the new text asset exists
        if (newMapTextAsset != null)
        {
            // Create a new newPDKMap from the TextAsset for the new map
            newPDKMap = pdkTiledUtilities.CreatePDKMapFromTextAsset(newMapTextAsset);
            #region Create Object Prefab List
            // Insatiate the object dictionary
            newPDKMap.objectsInMap = new PDKMap.PDKObjectTypes();
            // For each layer in this map
            foreach (PDKLayer thisLayer in newPDKMap.layers)
            {
                // If this layer is an object layer
                if (thisLayer.type == PDKLayer.layerTypes.Object)
                {
                    // Go through each object in this layer
                    foreach (PDKObject thisObject in thisLayer.objects)
                    {
                        // If this object type has not been found already
                        if (!newPDKMap.objectsInMap.Keys.Contains(thisObject.type))
                        {
                            // Add this object type to the hashset of discovered object types
                            newPDKMap.objectsInMap.Add(thisObject.type, null);
                        }
                    }
                }
            }
            #endregion
            return newPDKMap;
        }
        else
        {
            return null;
        }
    }


    // When this is called properties from another map are copied onto this map's settings
    public void CopyMatchingProperties(ref PDKMap mapToRecieve, PDKMap mapToCopy)
    {
        #region Check and Copy Tilesets
        // If the map settings to copy tilesetTextures is not blank
        if (mapToCopy.tilesets != null && mapToCopy.height > 0)
        {
            // For each tileset in this map
            for (int tilesetToCheck = 0; tilesetToCheck < mapToRecieve.tilesets.Length; tilesetToCheck++)
            {
                // Go through each tileset in the map to copy
                for (int tilesetToCopy = 0; tilesetToCopy < mapToCopy.tilesets.Length; tilesetToCopy++)
                {
                    // If these tilesets have the same name
                    if (mapToRecieve.tilesets[tilesetToCheck].name == mapToCopy.tilesets[tilesetToCopy].name)
                    {
                        // Copy that tileset from mapSettingsToCopy to this TIKMapSettings
                        mapToRecieve.tilesets[tilesetToCheck] = mapToCopy.tilesets[tilesetToCopy];
                    }
                }
            }
        }
        #endregion
        #region Check and Copy Object Types
        // If the map to copy has objects
        if (mapToCopy.objectsInMap != null)
        {
            // For each object type in the map to copy
            foreach (string objectType in mapToCopy.objectsInMap.Keys)
            {
                // If this map has an object with the same name as the map to copy
                if (mapToRecieve.objectsInMap.Keys.Contains(objectType))
                {
                    // Copy the matching object from the map to copy
                    mapToRecieve.objectsInMap[objectType] = mapToCopy.objectsInMap[objectType];
                }
            }
        }
        #endregion
    }


    void Awake()
    {
        // If a TIKMap has been created
        if (pdkMap != null && mapTextAsset != null)
        {
            // Tell this level's map to initialize
            pdkMap.InitializeMap();
            // Add the level controller to this object
            PDKLevelController levelController = this.gameObject.AddComponent<PDKLevelController>();
            // Give the TIKMap with the user's settings to the levelController
            levelController.levelMap = pdkMap;
            // Tell the level controller how close the camera can get to the edge of the loaded area before a new section of the level should be loaded
            levelController.bufferDistance = bufferDistance;
            //
            levelController.levelRenderer = new PDKLevelRenderer(pdkMap);
            // Disable this script
            //this.gameObject.GetComponent<PDKLevelConfigurator>().enabled = false;
        }
    }


    /* collision calculations
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

            if (pdkMap.width % thisTileIndex == 0) // If this tile is on the left edge of the map
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
            else if  (pdkMap.width % (thisTileIndex + 1) == 0) // If this tile is on the right edge of the map
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

            if (thisTileIndex < pdkMap.width) // If this tile is on the top edge of the map
            {
                // The top side of this tile should have a collider
                collisionSides[0] = true;
                // If this tile is sold, but tile below is not
                if (solidTilesHashSet.Contains(data[thisTileIndex]) && !solidTilesHashSet.Contains(data[thisTileIndex + pdkMap.width]))
                {
                    // The bottom side of this tile should have a collider
                    collisionSides[2] = true;
                }
            }
            else if (thisTileIndex >= data.Length - pdkMap.width) // If this tile is on the bottom edge of the map
            {
                // The bottom side of this tile should have a collider
                collisionSides[2] = true;
                // If this tile is sold, but the above tile is not
                if (solidTilesHashSet.Contains(data[thisTileIndex]) && !solidTilesHashSet.Contains(data[thisTileIndex - pdkMap.width]))
                {
                    // The top side of this tile should have a collider
                    collisionSides[0] = true;
                }
            }
            else if (solidTilesHashSet.Contains(data[thisTileIndex])) // If this tile is solid
            {
                // If the below tile is not solid
                if (!solidTilesHashSet.Contains(data[thisTileIndex - pdkMap.width]))
                {
                    // The bottom side of this tile should have a collider
                    collisionSides[2] = true;
                }
                // If the above tile is not solid
                if (!solidTilesHashSet.Contains(data[thisTileIndex + pdkMap.width]))
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
    }*/
}
