using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PDKLayerGroup
{
    // This is the type of layer that this group contains
    public TIKLayer.layerTypes groupType;
    // This is a list of the numbers of all layers in this layer group
    public List<int> layerNumbers = new List<int>();

    // Creates a new layer group
    public PDKLayerGroup(TIKLayer.layerTypes groupTypeToBe)
    {
        // Set this layer group's group type to the group type to be
        groupType = groupTypeToBe;
        // A new layer group has been created
        return;
    }
}
