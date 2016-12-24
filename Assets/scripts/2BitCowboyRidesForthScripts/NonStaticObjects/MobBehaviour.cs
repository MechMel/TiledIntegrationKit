using UnityEngine;
using System.Collections;

public class MobBehaviour : MonoBehaviour
{
    // The MobBehaviour class is the base class for all mobs

    // The MobType holds the possible mob types, which will be set in the Unity inspector
    public enum MobType
    {
        PATROL,
        CHASE,
        BLOCK
    }
    public MobType mobType;

    #region Basic Stats
    // Basic health
    public float health;
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
    #endregion
    #region AI
    // The state of the AI
    [HideInInspector]
    public int AIState = 0;
    // The animator state
    [HideInInspector]
    public int animState = 0;
    // The target of the enemy
    [HideInInspector]
    public GameObject currentTarget;
    #endregion

    // With a PATROL mob, this will need to be set
    [Tooltip("The left boundary of the mob. NOTE: Only necessary to set if the mob is a PATROL type.")]
    public Transform patrolPoint1;
    [Tooltip("The right boundary of the mob. NOTE: Only necessary to set if the mob is a PATROL type.")]
    public Transform patrolPoint2;
    // This will need to be set for CHASE MOBS
    [Tooltip("The rotatable child of the mob. NOTE: Only necessary to set if the mob is a CHASE type.")]
    public GameObject rotatableObject;

    #region Components
    // The array sprite renderers of the mob
    [Tooltip("The array of GameObjects that have a SpriteRenderer and Animator that is part of this object.")]
    public GameObject[] gameObjectsWithComponents;
    // The rigidbody of the mob
    [HideInInspector]
    public Rigidbody2D rigidBody2D; 
    #endregion

    void Start()
    {
        #region SetComponents
        // Set the spriteRenderer
        //spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        // Set the rigidBody2D
        rigidBody2D = GetComponentInChildren<Rigidbody2D>();
        #endregion
        #region SetBasic
        // Check to update gravity
        if (!isAffectedByGravity)
            rigidBody2D.gravityScale = 0;
        // Freeze the rotation
        rigidBody2D.freezeRotation = true;
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
        for(int i = 0; i < gameObjectsWithComponents.Length; i++)
            // Update the animator 
            gameObjectsWithComponents[i].GetComponent<Animator>().SetInteger("AnimState", animState);

        // Check for death
        if (health <= 0)
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
                    transform.Translate(Vector3.right * speed);
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
                    transform.Translate(Vector3.right * speed);
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
    }

    void Hit()
    {
        // When hit, subtract the health
        health--;
    }
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
