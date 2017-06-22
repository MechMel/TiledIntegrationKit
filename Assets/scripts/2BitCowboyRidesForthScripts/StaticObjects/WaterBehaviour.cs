using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBehaviour : MonoBehaviour
{
    // The behaviour of the water prefab
	
	
	void Update ()
    {
        // The built in Update() function

        // Raycast down to check for the player
        if (Physics2D.CircleCast(new Vector2(transform.position.x, transform.position.y - 1f), 0.5f, Vector2.down))
        {
            if (Physics2D.CircleCast(new Vector2(transform.position.x, transform.position.y - 1f), 0.5f, Vector2.down).transform.gameObject.tag == "Player")
                // If we hit the player, send a message to the player saying he is in water, and should jump out automatically
                Physics2D.CircleCast(new Vector2(transform.position.x, transform.position.y - 1f), 0.5f, Vector2.down).transform.gameObject.SendMessage("HitWater", SendMessageOptions.DontRequireReceiver);
        }
        // Raycast down to check for the player
        if (Physics2D.CircleCast(new Vector2(transform.position.x, transform.position.y - 1f), 0.5f, Vector2.down).transform.gameObject.tag == "Player")
        {
            // If we hit the player, send a message to the player saying he is in water
            Physics2D.CircleCast(new Vector2(transform.position.x, transform.position.y - 1f), 0.5f, Vector2.down).transform.gameObject.SendMessage("UpdateInWaterState", true, SendMessageOptions.DontRequireReceiver);
        }
            

    }
}
