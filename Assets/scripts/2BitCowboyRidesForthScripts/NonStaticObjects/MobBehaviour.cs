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
    // Whether the sprite is flipped or not
    public bool spriteFlipped = false;
    #endregion
    #region AI
    // The state of the AI
    [HideInInspector]
    public int AIState = 0;
    // The animator state
    [HideInInspector]
    public int animState = 0;
    #endregion

    // With a PATROL mob, this will need to be set
    [Tooltip("The two points that the mob will patrol between. NOTE: Only necessary to set if the mob is a PATROL type.")]
    public Transform patrolPoint1;
    [Tooltip("The two points that the mob will patrol between. NOTE: Only necessary to set if the mob is a PATROL type.")]
    public Transform patrolPoint2;

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
        {
            // Update the sprite
            //gameObjectsWithComponents[i].GetComponent<SpriteRenderer>().flipX = spriteFlipped;
            // Update the animator 
            gameObjectsWithComponents[i].GetComponent<Animator>().SetInteger("AnimState", animState);
        }
            

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

        }
        #endregion
        #region CHASE
        // Updated only if the mob is a CHASE type
        if (mobType == MobType.CHASE)
        {

        }
        #endregion
    }
}
