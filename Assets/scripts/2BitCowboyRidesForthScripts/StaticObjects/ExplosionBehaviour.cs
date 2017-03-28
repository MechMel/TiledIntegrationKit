using UnityEngine;
using System.Collections;

public class ExplosionBehaviour : MonoBehaviour {
    // A quick little script to handle the explosion drop

    // The range of the explosion
    [HideInInspector]
    private float rangeOfExplosion = 3f;

    void Start()
    {
        // Invoke the Explode() function
        Invoke("Explode", 0.2f);
    }

	void Explode()
    {
        // Run the explosion animation
        GetComponent<Animator>().SetTrigger("TriggerExplosion");
        // Get all the non solid objects in the explosion range
        Collider2D[] hitObjectColliders = Physics2D.OverlapCircleAll(transform.position, rangeOfExplosion, 1 << LayerMask.NameToLayer("NonSolid"));
        // Loop through each object that was hit
        foreach (Collider2D objectThatWasHit in hitObjectColliders)
        {
            // If the objectThatWasHit isn't ourself
            if (objectThatWasHit.gameObject != gameObject)
                // Send the Hit message on the objectThatWasHit, and apply 4 damage
                objectThatWasHit.gameObject.SendMessage("Hit", 4, SendMessageOptions.DontRequireReceiver);
        }
        // Destroy the object after half a second
        Destroy(gameObject, 0.5f);
	}
}
