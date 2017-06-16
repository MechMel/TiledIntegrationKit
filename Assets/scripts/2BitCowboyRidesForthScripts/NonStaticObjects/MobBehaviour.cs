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
    // Whether or not the mob can get hurt, usefull when not dealing with a mob
    public bool canGetHurt;
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
    public Vector2 patrolPoint1;
    public Vector2 patrolPoint2;
    // If one of the patrol points wasn't set, these will be put to true
    private bool patrolPoint1WasntSet = true;
    private bool patrolPoint2WasntSet = true;
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
        pdkObjectProperties.objectProperties = new PDKMap.PDKCustomProperties();
        // Declare the variables that will be saved when the object is deloaded
        pdkObjectProperties.objectProperties.Add("health", health.ToString());
        #endregion
        #region SetupSpecificMobBehaviourVars

        #region PATROL
        if (mobType == MobType.PATROL)
        {
            #region InitPatrolPoints
            // Initilaze the patrol points based off of either hitting a wall or an edge

            /*
             * Concept:
             * 
             * Basically, there will be a loop of raycasts out to a given distance.  
             * The raycast distance increments.
             * The raycasts will go in the pattern shown:
             * 
             *     -+-+-+-+-+
             *     
             * The - represents a raycast checking for a tile collision on the horizontal axis.
             * The + represents a raycast checking for a tile collision on the vertical axis.
             * 
             * In this way, the raycasting checks for a wall, than a hole in the ground.
             * If that fails, it will move over one more tile, and do the same thing.
             * This will repeat out to a given distance, since you probably don't want enemies potentially patrolling accross the whole map.
            */

            // The maximum distance the mob will patrol on both sides
            int maxDistanceToPatrol = 10;

            // Loop through for the first patrol point
            for (int i = (int)transform.position.x; i > (int)transform.position.x - maxDistanceToPatrol; i--)
            {
                // The object that was hit downwards, possibly kept to null if nothing was hit
                RaycastHit2D objectThatWasHitDown = Physics2D.Raycast(new Vector2(i, transform.position.y), Vector2.down, 1f);
                // The object that was hit to the left, possibly kept to null if nothing was hit
                RaycastHit2D objectThatWasHitLeft = Physics2D.Raycast(new Vector2(i, transform.position.y), Vector2.left, 1f);
                // Make sure that the object that was hit isn't null
                if (!objectThatWasHitDown)
                {
                    // First off, check downwards to see if we have reached an edge
                    //if (objectThatWasHitDown.transform.gameObject.tag == "Tile")
                    {
                        // If so, set this current spot in the loop to the edge of patroling to the left
                        patrolPoint1 = new Vector2(i, transform.position.y);
                        // Make sure to set the wasn't set to false
                        patrolPoint1WasntSet = false;
                        // Break out of the loop, since we found what we were looking for
                        break;
                    }
                }
                // Make sure that the object that was hit isn't null
                else if (objectThatWasHitLeft)
                {
                    // If this fails, check to the left to see if we have reached a wall
                    if (objectThatWasHitLeft.transform.gameObject.tag == "Tile")
                    {
                        // If so, set this current spot in the loop to the edge of patroling to the left
                        patrolPoint1 = new Vector2(i, transform.position.y);
                        // Make sure to set the wasn't set to false
                        patrolPoint1WasntSet = false;
                        // Break out of the loop, since we found what we were looking for
                        break;
                    }
                }
            }

            // Loop through for the second patrol point
            for (int i = (int)transform.position.x; i < (int)transform.position.x + maxDistanceToPatrol; i++)
            {
                // The object that was hit downwards, possibly kept to null if nothing was hit
                RaycastHit2D objectThatWasHitDown = Physics2D.Raycast(new Vector2(i, transform.position.y), Vector2.down, 1f);
                // The object that was hit to the right, possibly kept to null if nothing was hit
                RaycastHit2D objectThatWasHitRight = Physics2D.Raycast(new Vector2(i, transform.position.y), Vector2.right, 1f);
                // Make sure that the object that was hit isn't null
                if(!objectThatWasHitDown)
                {
                    // First off, check downwards to see if we have reached an edge
                    //if (objectThatWasHitDown.transform.gameObject.tag == "Tile")
                    {
                        // If so, set this current spot in the loop to the edge of patroling to the right
                        patrolPoint2 = new Vector2(i, transform.position.y);
                        // Make sure to set the wasn't set to false
                        patrolPoint2WasntSet = false;
                        // Break out of the loop, since we found what we were looking for
                        break;
                    }
                }
                // Make sure that the object that was hit isn't null
                else if (objectThatWasHitRight)
                {
                    // If this fails, check to the right to see if we have reached a wall
                    if (objectThatWasHitRight.transform.gameObject.tag == "Tile")
                    {
                        // If so, set this current spot in the loop to the edge of patroling to the right
                        patrolPoint2 = new Vector2(i, transform.position.y);
                        // Make sure to set the wasn't set to false
                        patrolPoint2WasntSet = false;
                        // Break out of the loop, since we found what we were looking for
                        break;
                    }
                }
            }

            // Just in case those 2 loops didn't create a patrol point, meaning that there wasn't a limit in the patrol distance
            if (patrolPoint1WasntSet)
                patrolPoint1 = new Vector2(transform.position.x - maxDistanceToPatrol, transform.position.y);
            if (patrolPoint2WasntSet)
                patrolPoint2 = new Vector2(transform.position.x + maxDistanceToPatrol, transform.position.y);
            #endregion

        }
        #endregion

        #endregion
    }

    void Update()
    {
        // Update sprite
        //if (spriteFlipped)
            GetComponent<SpriteRenderer>().flipX = spriteFlipped;
        //else
        //    GetComponent<SpriteRenderer>().flipX = true;

        // Go through the GameObjects with components   
        if (mobType != MobType.CHASE)
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
        if (Health <= 0 && canGetHurt)
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
                if (transform.position.x < patrolPoint1.x)
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
                if (transform.position.x > patrolPoint2.x)
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
                    // Flip the sprite based on the position of the player
                    if (currentTarget.transform.position.x > transform.position.x)
                        spriteFlipped = true;
                    else
                        spriteFlipped = false;
                    
                    // If hit something 
                    if (Physics2D.Raycast(rotatableObject.transform.position, rotatableObject.transform.forward, 20).transform != transform)
                    {
                        // Start turning right 
                        towardsTargetVector += Physics2D.Raycast(rotatableObject.transform.position, rotatableObject.transform.forward, 20).normal * 20;
                    }
                                       
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
        if(canGetHurt)
            Health -= damage;
    }
    #endregion

    private void OnCollisionStay2D(Collision2D other)
    {
        // If collided with anything other than a tile, ignore the collision
        if (other.gameObject.tag != "Tile")
        {
            // Send the hit
            if (other.gameObject.tag == "Player")
                other.gameObject.SendMessage("Hit", 1, SendMessageOptions.DontRequireReceiver);
            // Ignore the collision
            Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), other.gameObject.GetComponent<BoxCollider2D>());
        }
    }

    GameObject GetNearestObjectInArray(GameObject[] objects)
    {
        // Returns the nearest object in the given array of GameObjects
        // This probably isn't the most efficent way to to this, but it works, provided you have a small array

        // This stores where the closest object from the given position
        GameObject closestObjectInArray = null;
        // This stores the distance that the closest object so far has
        float closestDistanceOnRecord = Mathf.Infinity;
        // The base position to check distance from
        Vector3 currentPosition = transform.position;

        // For each potential GameObject in the objects
        foreach (GameObject potentialObject in objects)
        {
            // Get the direction to the target
            Vector3 directionToTarget = potentialObject.transform.position - currentPosition;
            // Get the distance to the target
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            // If the distance to the target is less than the closest distance on record
            if (dSqrToTarget < closestDistanceOnRecord)
            {
                // Set the closest distance on record to the new distance
                closestDistanceOnRecord = dSqrToTarget;
                // Set the potential object to the closest one in the array, since this might be the closest object in the array
                closestObjectInArray = potentialObject;
            }
        }
        // After the loop is finished, return the closest object in array
        return closestObjectInArray;
    }
}
