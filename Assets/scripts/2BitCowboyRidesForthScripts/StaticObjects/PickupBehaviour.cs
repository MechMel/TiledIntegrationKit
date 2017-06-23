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
        HEALTH,
        BUFF
    }
    public bool flashing;

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
        #region destruct coin
        //
        if (pickupType == PickupType.COIN)
        {
            Invoke("DestroySelfWithDelay", 6);
            Invoke("flash", 4.5f);
            flashing = true;
        }
        #endregion
        // Setup the sprite renderer
        sprRend = GetComponent<SpriteRenderer>();
        // Set the PDKObjectProperties variable for easy reference
        pdkObjectProperties = GetComponent<PDKObjectProperties>();

        #region SetupPDKObjectProperties
        // This region sets up the PDKObjectProperties, so the object can be hydrated and dehydrated

        // Initilize the objectProperties
        pdkObjectProperties.objectProperties = new PDKMap.PDKCustomProperties();
        // Declare the variables that will be saved when the object is deloaded
        pdkObjectProperties.objectProperties.Add("health", health.ToString());
        #endregion     

        // Check to turn off gravity
        GetComponent<Rigidbody2D>().isKinematic = !isAffectedByGravity;

        // If pickup is a buff
        //if (pickupType == PickupType.COIN || pickupType == PickupType.HEALTH)
        //    // Set the BoxCollider2D to a trigger
        //    GetComponent<Collider2D>().isTrigger = true;
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
                // Add coinage to the player
                amountOfCoinToAdd = Random.Range(0, 25);
                other.gameObject.GetComponent<PlayerController>().AddCoin(amountOfCoinToAdd);
                // Destroy this gameObject
                Destroy(gameObject);

            }
            else
            {
                //artificial colliding
                if (other.gameObject.tag != "Pickup")
                {
                    if (GetComponent<Rigidbody2D>().velocity.y < -1)
                    {
                        GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, -GetComponent<Rigidbody2D>().velocity.y * 0.75f);
                    }
                    if (GetComponent<Rigidbody2D>().velocity.y < 0&& GetComponent<Rigidbody2D>().velocity.y > -1)
                    {
                        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
                    }
                }
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

        #region BUFF
        if(pickupType == PickupType.BUFF)
        {
            // If the pickup is hit by a player
            if(other.tag == "Player")
            {
                // Add the amount of buffs to the player here!*************************

                // Destroy this object
                Destroy(this);
            }
        }
        #endregion
    }

    void Hit(int damage)
    {
        // If pickup is a ITEMBOX
        if (pickupType == PickupType.ITEMBOX)
            // Decrement health when hit
            Health -= damage;
    }
    void flash()
    {
        if (flashing)
        {
            if (GetComponent<SpriteRenderer>().enabled)
            {
                GetComponent<SpriteRenderer>().enabled = false;
            }
            else
            {
                GetComponent<SpriteRenderer>().enabled = true;
            }
            Invoke("flash", .15f);
        }
        else
        {
            GetComponent<SpriteRenderer>().enabled = true;
        }
    }

    void DestroySelfWithDelay()
    {
        Destroy(gameObject);
    }
}
