using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffBehaviour : MonoBehaviour {

    // The base class for all buffs

    // Note, all this script does is add health when a player collides with this.
    // It does not actually do anything with the object this component is attached to.

    // The amount of health to add to the collided object
    public int amountOfHealthToAdd;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // If the collided object is a player
        if(collision.gameObject.tag == "Player")
        {
            // Add health to the player
            collision.gameObject.SendMessage("AddHealth", amountOfHealthToAdd, SendMessageOptions.DontRequireReceiver);
            // Destroy this gameObject
            Destroy(gameObject);
        }
    }
}
