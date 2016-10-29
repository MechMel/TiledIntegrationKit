using UnityEngine;
using System.Collections;

public class movementTest : MonoBehaviour
{
    public float velocity = 3;

    Rigidbody2D myRigidbody;
    float dPadX = 0;
    float dPadY = 0;

    void Awake()
    {
        myRigidbody = this.gameObject.GetComponent<Rigidbody2D>();
    }


    void Update ()
    {
        dPadX = Input.GetAxis("dPadX");
        dPadY = Input.GetAxis("dPadY");
        //
        myRigidbody.velocity = new Vector3(velocity * dPadX, velocity * dPadY, 0);
    }
}
