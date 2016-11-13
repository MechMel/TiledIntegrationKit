using UnityEngine;
using System.Collections;

public class PDKLevelController : MonoBehaviour
{
    // This is the TIKMap for this level
    public TIKMap levelMap;
    // This is the PDKLevelRenderer for this level
    public PDKLevelRenderer levelRenderer;
    // This is how many tiles out from the camera to load this level
    public int bufferDistance;
    //
    float screenRatioWidthToHeight = (float)Screen.width / (float)Screen.height;
    //
    Camera mainCamera;
    //
    Vector3 mainCameraPosition;


    void Awake()
    {
        //
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        //
        transform.position = new Vector3(mainCameraPosition.x, mainCameraPosition.y, transform.position.z);
    }

    void Start()
    {
        //
        mainCameraPosition = GameObject.FindGameObjectWithTag("MainCamera").transform.position;
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
        if (Vector2.Distance(mainCameraPosition, transform.position) > 1)
        {
            //
            transform.position = new Vector3((int)mainCameraPosition.x, (int)mainCameraPosition.y, transform.position.z);
            //
            Reload();
        }
        //logDeltaTimeGreaterThanMinimum("Continual", 0.019f);
    }

    public void Reload()
    {
        // Calculate the width and the height of the area to render
        int renderAreaWidth = (int)(2 * screenRatioWidthToHeight * (mainCamera.orthographicSize + bufferDistance));
        int renderAreaHeight = (int)(2 * (mainCamera.orthographicSize + bufferDistance));

        // Load the appropriate area of the level
        levelRenderer.LoadRectOfMap(new Rect(
                x: (int)mainCameraPosition.x - (renderAreaWidth / 2),
                y:  -(int)mainCameraPosition.y + (renderAreaHeight / 2), 
                width: renderAreaWidth, 
                height: renderAreaHeight));
    }

    private void logDeltaTimeGreaterThanMinimum(string NameOfTime, float minimumDelta)
    {
        // If the delta time was greater than the minimum
        if (Time.deltaTime > minimumDelta)
        {
            // Log the delta time
            Debug.Log(Time.time + " " + NameOfTime + ": " + Time.deltaTime * 1000 + "miliseconds");
        }
    }
}