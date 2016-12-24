using UnityEngine;
using System.Collections;

public class PickupBehaviour : MonoBehaviour {

    // The different pickup types possible
    public enum PickupType
    {
        ITEMBOX,
        DOOR,
        BUFF
    }
    public PickupType pickupType;

    #region Basic Stats
    // The health of the pickup
    public int health;
    // Whether the pickup is affected by gravity
    public bool isAffectedByGravity;
    // The array of what is possible to drop
    [Tooltip("The possible drops of the object. Drops a random object out of the array.")]
    public GameObject[] possibleDrops;
    // The amount of objects to drop
    [Tooltip("The amount of items to drop.")]
    public int amountToDrop;
    #endregion
    #region Components
    // The sprite renderer of the pickup
    [HideInInspector]
    SpriteRenderer sprRend;
    #endregion

    void Start ()
    {
        // Setup the sprite renderer
        sprRend = GetComponent<SpriteRenderer>();
	}
	
	
	void Update ()
    {
	    // Check if destroyed
        if(health <= 0)
        {
            // Destroy the object
            Destroy(gameObject);
            // Drop the right amount of items based on the given stats
            for(int i = 0; i < amountToDrop; i++)
            {
                // Create a random pickup from the possibleDrops array
                Instantiate(possibleDrops[Mathf.RoundToInt(Random.Range(0, possibleDrops.Length))], 
                    new Vector2(transform.position.x - i / 2, transform.position.y), Quaternion.identity);
            }
        }


	}
}
