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
    [Header("---ITEMBOX VARIABLES---")]
    // The health of the pickup
    public int health;
    // Whether the pickup is affected by gravity
    public bool isAffectedByGravity;
    // The array of images
    [Tooltip("The array of images for breakable pickups. Only set for ITEMBOX pickups.")]
    public Sprite[] imagesOfPickup;
    // The array of what is possible to drop
    [Tooltip("Drops a random object out of the array. Only set for ITEMBOX pickups.")]
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

        // Check to turn off gravity
        if (isAffectedByGravity)
            GetComponent<Rigidbody2D>().isKinematic = false;
        else
            GetComponent<Rigidbody2D>().isKinematic = true;

        // If pickup is a BUFF
        if (pickupType == PickupType.BUFF)
            // Set the BoxCollider2D to a trigger
            GetComponent<BoxCollider2D>().isTrigger = true;
    }
		
	void Update ()
    {
        #region ITEMBOX
        if (pickupType == PickupType.ITEMBOX)
        {
            // Check if destroyed
            if (health <= 0)
            {
                // Destroy the object
                Destroy(gameObject);
                // If the amount to drop is greater than zero
                if(amountToDrop > 0)
                {
                    // Drop the right amount of items based on the given stats
                    for (int i = 0; i < amountToDrop; i++)
                    {
                        // Create a random pickup from the possibleDrops array
                        Instantiate(possibleDrops[Mathf.RoundToInt(Random.Range(0, possibleDrops.Length))],
                            new Vector2(transform.position.x + amountToDrop/2 - i, transform.position.y), Quaternion.identity);
                    }
                }
                
            }
            else
            {
                // Update the sprite of the pickup based on the health
                sprRend.sprite = imagesOfPickup[Mathf.RoundToInt(health - 1)];
            }
        }
        #endregion
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // If the pickup is of BUFF pickup type
        if (pickupType == PickupType.BUFF && other.tag != "Bullet")
        {
            // Destroy itself
            Destroy(gameObject);
        }
    }

    void Hit(int damage)
    {
        // If pickup is a BUFF
        if (pickupType == PickupType.ITEMBOX)
            // Decrement health when hit
            health -= damage;
    }
}
