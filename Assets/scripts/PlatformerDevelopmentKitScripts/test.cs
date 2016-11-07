using UnityEngine;
using System.Collections;

public class test : MonoBehaviour {

    EdgeCollider2D myCollider;
    // Use this for initialization
    void Awake()
    {
        myCollider = this.gameObject.AddComponent<EdgeCollider2D>();
    }
    void Start()
    {
        Vector2[] newPoints = new Vector2[2];
        newPoints[0] = new Vector2(-0.5f, 0.5f);
        newPoints[1] = new Vector2(0.5f, 0.5f);
        myCollider.points = newPoints;
        Debug.Log(myCollider.points[1].y);
    }
}
