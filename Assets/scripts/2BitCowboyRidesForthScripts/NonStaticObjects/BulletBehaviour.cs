using UnityEngine;
using System.Collections;

public class BulletBehaviour : MonoBehaviour {

	// BulletBehaviour class determines the behaviour of the standard player bullet

    void OnCollisionEnter2D(Collision2D other)
    {
        // Check if other is not a player
        if(other.transform.tag != "Player")
        {
            // If collided with something
            Debug.Log("Bullet hit something");
            // Send the hit message to the object that was hit
            other.gameObject.SendMessage("Hit", null, SendMessageOptions.DontRequireReceiver);
            // Destroy itself
            Destroy(gameObject);
        }
        
    }
}
