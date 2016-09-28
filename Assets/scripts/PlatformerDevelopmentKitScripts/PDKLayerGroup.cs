using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PDKLayerGroup
{
    // This is the type of layer that this group contains
    public TIKLayer.layerTypes groupType;
    // This is a list of the numbers of all layers in this layer group
    public List<int> layerNumbers = new List<int>();
}
