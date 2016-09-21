using UnityEngine;
using System.Collections;

public class PDKLevelController : MonoBehaviour
{
    // This is the TIKMap for this level
    public TIKMap levelMap;
    // This is the PDKLevelRenderer for this level
    public PDKLevelRenderer levelRenderer = new PDKLevelRenderer();


    void Start()
    {
        //
        levelRenderer.RenderRectangleOfMap(levelMap, new Rect(3, 100, 3, 3));
    }
}
