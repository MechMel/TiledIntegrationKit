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
    //
    float screenRatio = (float)Screen.width / (float)Screen.height;


    void Start()
    {
    }

    void Update()
    {
        Vector3 mainCameraPosition = GameObject.FindGameObjectWithTag("MainCamera").transform.position;
        if (Vector2.Distance(mainCameraPosition, this.gameObject.transform.position) > bufferDistance)
        {
            transform.position = new Vector3(mainCameraPosition.x, mainCameraPosition.y, transform.position.z);
            Reload();
        }
    }

    public void Reload()
    {
        //
        levelRenderer.RemoveAllLayers();
        //
        levelRenderer.RenderRectangleOfMapAtPosition(levelMap, new Rect(0, 20, (int)(2 * screenRatio * loadDistance), 2 * loadDistance), transform.position);
    }
}
