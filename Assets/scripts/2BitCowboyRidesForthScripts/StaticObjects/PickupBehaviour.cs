using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PickupBehaviour : MonoBehaviour {

    // The different pickup types possible
    public enum PickupType
    {
        ITEMBOX,
        DOOR,
        COIN,
        HEALTH
    }
    public PickupType pickupType;

    #region Basic Stats
    [Header("---ITEMBOX VARIABLES---")]
    // The health of the pickup
    public int health;
    // Setup the get set stuff for saving the health
    public int Health
    {
        get
        {
            // If the health property exists in the object properties
            if (pdkObjectProperties != null && pdkObjectProperties.objectProperties.ContainsKey("health"))
            {
                if (pdkObjectProperties.objectProperties.ContainsKey("health"))
                    // Grab the value of the health from the PDKObjectProperties and returns it
                    return int.Parse(pdkObjectProperties.objectProperties["health"]);
                else
                    // Otherwise, return -1 indicating a null value
                    return 1;
            }
            else
            {
                // Otherwise, return 1 indicating a null value
                return 1;
            }
        }
        set
        {
            // Sets the local variable as well as updating it in the PDKObjectProperties
            health = value;
            pdkObjectProperties.objectProperties["health"] = health.ToString();
        }
    }
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
    [Header("------------------------")]
    // While being destroyed, this is set to true, to avoid multiple drops
    private bool beingDestroyed = false;
    // The amount of health to add to the collided object
    [Tooltip("Only set for HEALTH.")]
    public int amountOfHealthToAdd;
    // The amount of coins to add to the collided object
    [Tooltip("Only set for COIN.")]
    public int amountOfCoinToAdd;
    #endregion
    #region Components
    // The sprite renderer of the pickup
    [HideInInspector]
    SpriteRenderer sprRend;
    // The PDK Object Properties of the pickup
    [HideInInspector]
    public PDKObjectProperties pdkObjectProperties;
    #endregion

    void Awake ()
    {
        // Setup the sprite renderer
        sprRend = GetComponent<SpriteRenderer>();
        // Set the PDKObjectProperties variable for easy reference
        pdkObjectProperties = GetComponent<PDKObjectProperties>();
        #region SetupPDKObjectProperties
        // This region sets up the PDKObjectProperties, so the object can be hydrated and dehydrated

        // Initilize the objectProperties
        pdkObjectProperties.objectProperties = new PDKObjectProperties.PDKSerializableObjectProperties();
        // Declare the variables that will be saved when the object is deloaded
        pdkObjectProperties.objectProperties.Add("health", health.ToString());
        #endregion     

        // Check to turn off gravity
        if (isAffectedByGravity)
            GetComponent<Rigidbody2D>().isKinematic = false;
        else
            GetComponent<Rigidbody2D>().isKinematic = true;

        // If pickup is a buff
        if (pickupType == PickupType.COIN || pickupType == PickupType.HEALTH)
            // Set the BoxCollider2D to a trigger
            GetComponent<BoxCollider2D>().isTrigger = true;
    }
		
	void Update ()
    {
        #region ITEMBOX
        if (pickupType == PickupType.ITEMBOX)
        {
            // Check if destroyed
            if (Health <= 0 && !beingDestroyed)
            {
                // Destroy the object
                Invoke("DestroySelfWithDelay", 0.2f);
                beingDestroyed = true;
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
                if(Health - 1 >= 0)
                    sprRend.sprite = imagesOfPickup[Mathf.RoundToInt(Health - 1)];
            }
        }
        #endregion
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        #region COIN
        if (pickupType == PickupType.COIN)
        {
            // If the collided object is a player
            if (other.gameObject.tag == "Player")
            {
                // Add health to the player
                other.gameObject.SendMessage("AddCoin", amountOfCoinToAdd, SendMessageOptions.DontRequireReceiver);
                // Destroy this gameObject
                Destroy(gameObject);
            }
        }

        #endregion
        #region HEALTH
        if (pickupType == PickupType.HEALTH)
        {
            // If the collided object is a player
            if (other.gameObject.tag == "Player")
            {
                // Add health to the player
                other.gameObject.SendMessage("AddHealth", amountOfHealthToAdd, SendMessageOptions.DontRequireReceiver);
                // Destroy this gameObject
                Destroy(gameObject);
            }
        }
        #endregion
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        // Otherwise, if collided with another pickup
        if (pickupType == PickupType.COIN && other.gameObject.tag != "Tile")
        {
            // Ignore the collision, since we don't want coins stacking up on top of each other
            Physics2D.IgnoreCollision(GetComponent<CircleCollider2D>(), other.gameObject.GetComponent<Collider2D>());
            Physics2D.IgnoreCollision(GetComponent<CircleCollider2D>(), other.gameObject.GetComponent<BoxCollider2D>());
            Physics2D.IgnoreCollision(GetComponent<CircleCollider2D>(), other.gameObject.GetComponent<CircleCollider2D>());
        }
    }

    void Hit(int damage)
    {
        // If pickup is a BUFF
        if (pickupType == PickupType.ITEMBOX)
            // Decrement health when hit
            Health -= damage;
    }

    void DestroySelfWithDelay()
    {
        Destroy(gameObject);
    }
}
