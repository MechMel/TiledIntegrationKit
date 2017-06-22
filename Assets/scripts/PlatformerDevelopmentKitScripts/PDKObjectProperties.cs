using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PDKObjectProperties : MonoBehaviour
{
    // TODO: FILL THIS IN LATER
    public int id;
    public int gid;
    [HideInInspector]
    public PDKLayer layerThisObjectIsIn;

    // Holds this object's properties
    public PDKMap.PDKCustomProperties objectProperties;

    private void OnDestroy()
    {
        layerThisObjectIsIn.HydratedObectDestoryed(gameObject);
    }
}
