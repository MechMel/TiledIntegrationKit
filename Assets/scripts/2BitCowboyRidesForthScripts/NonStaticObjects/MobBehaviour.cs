using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(PDKObjectProperties))]

public class MobBehaviour : MonoBehaviour
{
    // The MobBehaviour class is the base class for all mobs

    // The MobType holds the possible mob types, which will be set in the Unity inspector
    public enum MobType
    {
        PATROL,
        CHASE,
        BLOCK,
        RIDE
    }
    public MobType mobType;

    #region Mob Stats
    // Basic health
    public int health;
    // Setup the get set stuff for saving the health
    public int Health
    {
        get
        {
            // If the health property exists in the object properties
            if (pdkObjectProperties != null && pdkObjectProperties.objectProperties.ContainsKey("health"))
            {
                if(pdkObjectProperties.objectProperties.ContainsKey("health"))
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
    // Basic damage
    public float damage;
    // Basic speed
    public float speed;
    // The check for whether the mob is affected by gravity
    public bool isAffectedByGravity;
    // How far away the mob can see
    public float visibleDistance;
    // Whether the sprite is flipped or not
    [HideInInspector]
    public bool spriteFlipped = false;
    // Whether or not the mob is grounded
    [HideInInspector]
    public bool mobGrounded = false;
    // The state of the AI
    [HideInInspector]
    public int AIState = 0;
    // The animator state
    [HideInInspector]
    public int animState = 0;
    // The target of the enemy
    [HideInInspector]
    public GameObject currentTarget;

    // ---Controls

    // Move left boolean
    [HideInInspector]
    public bool moveLeft = false;
    // Move right boolean
    [HideInInspector]
    public bool moveRight = false;
    // Jump boolean
    [HideInInspector]
    public bool moveJump = false;

    // With a PATROL mob, this will need to be set
    [Tooltip("The left boundary of the mob. NOTE: Only necessary to set if the mob is a PATROL type.")]
    public Transform patrolPoint1;
    [Tooltip("The right boundary of the mob. NOTE: Only necessary to set if the mob is a PATROL type.")]
    public Transform patrolPoint2;
    // This will need to be set for CHASE MOBS
    [Tooltip("The rotatable child of the mob. NOTE: Only necessary to set if the mob is a CHASE type.")]
    public GameObject rotatableObject;
    #endregion
    #region Components
    // The array sprite renderers of the mob
    [Tooltip("The array of GameObjects that have a SpriteRenderer and Animator that is part of this object.")]
    public GameObject[] gameObjectsWithComponents;
    // The rigidbody of the mob
    [HideInInspector]
    public Rigidbody2D rigidBody2D;
    // The PDK Object Properties of the mob
    [HideInInspector]
    public PDKObjectProperties pdkObjectProperties;
    #endregion

    void Awake()
    {
        #region SetComponents
        // Set the spriteRenderer
        //spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        // Set the rigidBody2D
        rigidBody2D = GetComponentInChildren<Rigidbody2D>();
        // Set the PDKObjectProperties variable for easy reference
        pdkObjectProperties = GetComponent<PDKObjectProperties>();
        #endregion
        #region SetBasic
        // Check to update gravity
        if (!isAffectedByGravity)
            rigidBody2D.gravityScale = 0;
        // Freeze the rotation
        rigidBody2D.freezeRotation = true;
        #endregion
        #region SetupPDKObjectProperties
        // This region sets up the PDKObjectProperties, so the object can be hydrated and dehydrated

        // Initilize the objectProperties
        pdkObjectProperties.objectProperties = new Dictionary<string, string>();
        // Declare the variables that will be saved when the object is deloaded
        pdkObjectProperties.objectProperties.Add("health", health.ToString());
        #endregion     
    }

    void Update()
    {
        // Update sprite
        if (spriteFlipped)
            transform.localScale = new Vector3(-1, 1, 1);
        else
            transform.localScale = new Vector3(1, 1, 1);
        
        // Go through the GameObjects with components   
        if(mobType != MobType.CHASE)
        {
            for(int i = 0; i < gameObjectsWithComponents.Length; i++)
                // Update the animator 
                gameObjectsWithComponents[i].GetComponent<Animator>().SetInteger("AnimState", animState);
        }

        // Update the debug text
        if (transform.Find("DebugText") != null)
            transform.Find("DebugText").GetComponent<TextMesh>().text = GetComponent<PDKObjectProperties>().objectProperties["health"];
        // Update the grounded state
        /*
        // Get the collision points
        Collider2D[] groundColliders = Physics2D.OverlapCircleAll(transform.Find("GroundCheck").position, 0.01f, 1 << LayerMask.NameToLayer("Solid"));
        // Make sure a collision exists
        if (groundColliders.Length > 0)
        {
            // Check for the player being grounded
            for (int i = 0; i < groundColliders.Length; i++)
            {
                if (groundColliders[i].gameObject != gameObject)
                    mobGrounded = true;
            }
        }
        */
        // Check for death
        if (Health <= 0)
            Destroy(gameObject);

        #region PATROL
        // Updated only if the mob is a PATROL type
        if (mobType == MobType.PATROL)
        {
            // Check for collision of the player

            
            // Update the patrol movement

            // If the spriteFlipped is true
            if(spriteFlipped)
            {
                // If close enough to the patrol point
                if (transform.position.x < patrolPoint1.position.x)
                    // Flip direction
                    spriteFlipped = false;
                else
                    // Otherwise, continue moving towards the patrolPoint
                    transform.Translate(Vector3.right * speed * transform.localScale.x);
            }
            // Otherwise, meaning its false
            else
            {
                // If close enough to the patrol point
                if (transform.position.x > patrolPoint2.position.x)
                    // Flip direction
                    spriteFlipped = true;
                else
                    // Otherwise, continue moving towards the patrolPoint
                    transform.Translate(Vector3.right * speed * transform.localScale.x);
            }
        }
        #endregion
        #region BLOCK
        // Updated only if the mob is a BLOCK type
        if (mobType == MobType.BLOCK)
        {
            // Set the rigidbody to kinimatic
            rigidBody2D.isKinematic = true;
        }
        #endregion
        #region CHASE
        // Updated only if the mob is a CHASE type
        if (mobType == MobType.CHASE)
        {
            // This is the only mobtype that actually has states, or rather, actually thinks
            if(AIState == 0)
            {
                // Idle/Reset state
                while (true)
                {
                    // Check if a player is close enough
                    if (Vector2.Distance(transform.position, GetNearestObjectInArray(GameObject.FindGameObjectsWithTag("Player")).transform.position) < visibleDistance)
                    {
                        // Set the target
                        currentTarget = GetNearestObjectInArray(GameObject.FindGameObjectsWithTag("Player"));
                        // Go to the chase state
                        AIState = 1;
                        // End the code early, so we actually change states right away
                        break;
                    }

                    // Break at the end, because we don't really want this to loop
                    break;
                }
            }
            else if(AIState == 1)
            {
                // Chase state

                while (true)
                {
                    // Get the goal vector
                    Vector2 towardsTargetVector = (currentTarget.transform.position - rotatableObject.transform.position).normalized;

                    // Setup a loop, instead of continually turning and moving forward(Decides the direction imediantly)
                    //while (true)
                    //{
                        // If hit something 
                        if (Physics2D.Raycast(rotatableObject.transform.position, rotatableObject.transform.forward, 20).transform != transform)
                        {
                            // Start turning right 
                            towardsTargetVector += Physics2D.Raycast(rotatableObject.transform.position, rotatableObject.transform.forward, 20).normal * 20;
                            Debug.DrawLine(rotatableObject.transform.position, rotatableObject.transform.position + Vector3.forward);
                        }
                        //else
                            // If not running into something, stop rotating
                        //    break;                   
                    //}

                    // Translate the new vector into a rotation
                    Quaternion newRotation = Quaternion.LookRotation(towardsTargetVector);
                    // Rotate the rotatable object to the new rotation
                    rotatableObject.transform.localRotation = Quaternion.Slerp(rotatableObject.transform.localRotation, newRotation, Time.deltaTime);
                    // Move the actual mob in the new direction 
                    transform.position += rotatableObject.transform.forward * speed * Time.deltaTime;

                    // Break at the end, because we don't really want this to loop
                    break;
                }
            }
        }
        #endregion
        #region RIDE
        // Updated if the mob is of RIDE mobType
        if(mobType == MobType.RIDE)
        {
            /*
            // If receiving left
            if (moveLeft)
            {
                // Move the animal left 
                transform.Translate(speed * transform.localScale.x, 0, 0);
                // If grounded, flip the sprite, and run the walking animation
                transform.localScale = new Vector2(-1, 1);
                // Flip the sprite
                spriteFlipped = true;
                // Update the animation state
                GetComponent<Animator>().SetInteger("AnimState", 1);
            }
            // If receiving right
            else if (moveRight)
            {
                // Move the animal right 
                transform.Translate(speed * transform.localScale.x, 0, 0);
                // If grounded, flip the sprite, and run the walking animation
                transform.localScale = new Vector2(1, 1);
                // Flip the sprite
                spriteFlipped = false;
                // Update the animation state
                GetComponent<Animator>().SetInteger("AnimState", 1);
            }
            else if (!Up && playerGrounded)
            {
                // If not pressing any keys and grounded, run the idle animation
                animalPlayerIsRiding.GetComponent<Animator>().SetInteger("AnimState", 0);
            }*/
        }
        #endregion
    }

    #region Invokeable Functions
    // This group of functions are designed to be invoked from the outside

    void MoveLeft(bool setMoveLeft)
    {
        // Recieves the request to move left
        moveLeft = setMoveLeft;
    }
    void MoveRight(bool setMoveRight)
    {
        // Recieves the request to move right
        moveRight = setMoveRight;
    }
    void MoveJump(bool setMoveJump)
    {
        // Recieves the request to jump
        moveJump = setMoveJump;
    }
    void Hit(int damage)
    {
        // When hit, subtract the health
        Health -= damage;
    }
    #endregion

    GameObject GetNearestObjectInArray(GameObject[] objects)
    {
        GameObject closestObjectWithTag = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (GameObject potentialObject in objects)
        {
            Vector3 directionToTarget = potentialObject.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                closestObjectWithTag = potentialObject;
            }
        }

        return closestObjectWithTag;
    }
}
