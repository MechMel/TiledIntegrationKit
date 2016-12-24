using UnityEngine;
using System.Collections;

public class BulletBehaviour : MonoBehaviour {

	// BulletBehaviour class determines the behaviour of the standard player bullet

    void OnTriggerEnter2D(Collider2D other)
    {
        // When collided with something

        // Check if other is not a player
        if(other.transform.tag != "Player")
        {
            // Send the hit message to the object that was hit
            other.gameObject.SendMessage("Hit", null, SendMessageOptions.DontRequireReceiver);
            // Destroy itself
            Destroy(gameObject);
        }
        
    }
}
