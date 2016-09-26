using UnityEngine;
using System.Collections;

public class movementTest : MonoBehaviour
{
    public float velocity = 3;

    Rigidbody2D myRigidbody;

    void Awake()
    {
        myRigidbody = this.gameObject.GetComponent<Rigidbody2D>();
    }


    void Update ()
    {
        myRigidbody.velocity = new Vector3(velocity, 0, 0);
    }
}
