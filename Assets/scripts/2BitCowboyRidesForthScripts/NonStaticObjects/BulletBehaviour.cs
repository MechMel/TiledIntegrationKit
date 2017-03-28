using UnityEngine;
using System.Collections;

public class BulletBehaviour : MonoBehaviour {

    // BulletBehaviour class determines the behaviour of the standard player bullet

    public GameObject explosion;

    void OnTriggerEnter2D(Collider2D other)
    {
        // When collided with something

        // Check if other is not a player
        if(other.transform.tag != "Player" && other.transform.tag != "Bullet" && other.transform.tag != "Pickup")
        {
            // Send the hit message to the object that was hit
            other.gameObject.SendMessage("Hit", 1, SendMessageOptions.DontRequireReceiver);
            Instantiate(explosion, transform.position, Quaternion.identity);
            // Destroy itself
            Destroy(gameObject);
        }        
    }
}
