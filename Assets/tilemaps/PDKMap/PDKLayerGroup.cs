using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PDKLayerGroup
{
    // This is the type of layer that this group contains
    public PDKLayer.layerTypes groupType;
    // This is a list of the numbers of all layers in this layer group
    public List<int> layerNumbers = new List<int>();
    // This is the z position this group should be rendred at
    public int zPosition;
    // Refrence to this layer group's object (Used with Tile and Image Layer Groups)
    public GameObject layerGroupObject;
    // The Texture currently applied to this layer group (Used with Tile LayerGroups)
    public Texture2D layerGroupTexture;

    // Creates a new layer group
    public PDKLayerGroup(PDKLayer.layerTypes groupTypeToBe)
    {
        // Set this layer group's group type to the group type to be
        groupType = groupTypeToBe;
        // A new layer group has been created
        return;
    }
}
