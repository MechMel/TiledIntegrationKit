using UnityEngine;
using System.Collections;

public class PDKLevelController : MonoBehaviour
{
    // This is the TIKMap for this level
    public TIKMap levelMap;
    // This is the PDKLevelRenderer for this level
    public PDKLevelRenderer levelRenderer = new PDKLevelRenderer();
    // This is how far from out from the camera to load this level
    public int loadDistance;
    // This is how close the camera can get to the edge of the loaded area before a new section should be loaded
    public int bufferDistance;


    void Start()
    {
        //
        levelRenderer.RenderRectangleOfMap(levelMap, new Rect(0, 18, loadDistance, loadDistance));
    }
}
