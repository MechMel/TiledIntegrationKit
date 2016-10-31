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
    //
    Vector3 mainCameraPosition;

    void Awake()
    {
        //
        transform.position = new Vector3(mainCameraPosition.x, mainCameraPosition.y, transform.position.z);
    }

    void Start()
    {
        //
        transform.position = new Vector3((int)mainCameraPosition.x, (int)mainCameraPosition.y, transform.position.z);
        //
        Reload();
    }

    void Update()
    {
        //
        mainCameraPosition = GameObject.FindGameObjectWithTag("MainCamera").transform.position;
        //
        if (Vector2.Distance(mainCameraPosition, transform.position) > bufferDistance)
        {
            //
            transform.position = new Vector3((int)mainCameraPosition.x, (int)mainCameraPosition.y, transform.position.z);
            //
            Reload();
        }
        logDeltaTimeGreaterThanMinimum("Continual", .025f);
    }

    public void Reload()
    {
        //
        levelRenderer.RenderRectangleOfMapAtPosition(levelMap, new Rect(transform.position.x, -transform.position.y, (int)(2 * screenRatio * loadDistance), 2 * loadDistance), transform.position);
    }

    private void logDeltaTimeGreaterThanMinimum(string NameOfTime, float minimumDelta)
    {
        // If the delta time was greater than the minimum
        if (Time.deltaTime > minimumDelta)
        {
            // Log the delta time
            Debug.Log(Time.time + " " + NameOfTime + ": " + Time.deltaTime);
        }
    }
}