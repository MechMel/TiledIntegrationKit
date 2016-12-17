using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    // The PlayerController class is the basic controller for the player, and the base class for add-ons

    #region PLAYER
    // The player speed
    [HideInInspector]
    private float playerSpeed = 0.1f;
    // The player jump speed
    [HideInInspector]
    private float playerJumpSpeed = 700f;
    // The player health
    [HideInInspector]
    public int playerHealth = 4;
    // Whether the player is grounded
    [HideInInspector]
    public bool playerGrounded = false;
    // Whether the player is touching a wall to the left
    [HideInInspector]
    public bool playerTouchingLeftWall = false;
    // Whether the player is touching a wall to the right
    [HideInInspector]
    public bool playerTouchingRightWall = false;
    // The player jump speed
    [HideInInspector]
    public bool playerCanDoubleJump = false;
    // The player animation speed
    // NOT YET IMPLEMENTED
    [HideInInspector]
    public float playerAnimationSpeed = 8f;
    #endregion
    #region OBJECTS
    // The nearest rideable animal
    [HideInInspector]
    public GameObject animalPlayerIsRiding;
    // Whether riding an animal or not
    [HideInInspector]
    public bool Riding = false;
    // Whether the animal the player is riding is grounded
    [HideInInspector]
    public bool animalPlayerIsRidingGrounded = false;
    #endregion
    #region COMPONENTS
    // The animator component
    [HideInInspector]
    public Animator anim;
    // The animator component
    [HideInInspector]
    public Rigidbody2D playerRigidBody2D;
    // The SpriteRenderer component
    [HideInInspector]
    public SpriteRenderer sprRend;
    // The GroundCheck GameObject
    [HideInInspector]
    public GameObject groundCheck;
    // The WallRightCheck GameObject
    [HideInInspector]
    public GameObject rightWallCheck;
    // The WallLeftCheck GameObject
    [HideInInspector]
    public GameObject leftWallCheck;
    #endregion

    bool CheckIfTouchingObject(Vector2 origin, Vector2 direction, float distance, string tagOfObject)
    {
        // Checks if touching an object in the specified direction

        // Create a ray pointing in the direction
        RaycastHit2D objectTouching = Physics2D.Raycast(origin, direction, distance);
        // If the object hit not equal to null and the tag is the same as the argument
        if (objectTouching.collider != null && objectTouching.transform.tag == tagOfObject)
            return true;
        // If it doesn't, return false
        return false;
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

    void Start ()
    {
        // Hookup components
        anim = GetComponent<Animator>();
        playerRigidBody2D = GetComponent<Rigidbody2D>();
        sprRend = GetComponent<SpriteRenderer>();
        groundCheck = gameObject.transform.Find("GroundCheck").gameObject;
        rightWallCheck = gameObject.transform.Find("RightWallCheck").gameObject;
        leftWallCheck = gameObject.transform.Find("LeftWallCheck").gameObject;
    }
    
    void Update()
    {
        // Update() has all the physics checks

        #region GRAVITY
        // Update gravity if touching wall
        if(playerTouchingLeftWall || playerTouchingRightWall)
        {
            playerRigidBody2D.velocity = new Vector2(playerRigidBody2D.velocity.x, -0.2f);
        }
        // Update the check if the player is grounded 
        playerGrounded = CheckIfTouchingObject(groundCheck.transform.position, -Vector2.up, 0f, "Tile");
        
        

        // Update the check if the player is touching the left wall
        if (CheckIfTouchingObject(leftWallCheck.transform.position, -Vector2.right, 0.65f, "Tile"))
        {
            playerTouchingLeftWall = true;
            playerCanDoubleJump = true;
        }
        else
        {
            playerTouchingLeftWall = false;
        }
        
        // Update the check if the player is touching the right wall        
        if (CheckIfTouchingObject(rightWallCheck.transform.position, Vector2.right, 0.65f, "Tile"))
        {
            playerTouchingRightWall = true;
            playerCanDoubleJump = true;
        }
        else
        {
            playerTouchingRightWall = false;
        }
        #endregion
        #region Rideable animals
        // If close enough to a rideable animal
        if (Vector2.Distance(GetNearestObjectInArray(GameObject.FindGameObjectsWithTag("RideableAnimal")).transform.position, transform.position) < 3)
        {
            // If pressing the Interact key
            if(Input.GetButtonDown("Interact"))
            {
                // Either get on, or get off, depending on the state of Riding
                Riding = !Riding;
                // Set the animal the player is riding
                animalPlayerIsRiding = GetNearestObjectInArray(GameObject.FindGameObjectsWithTag("RideableAnimal"));
            }
        }
        
        if (Riding)
        {   
            // Update position if riding       
            transform.position = animalPlayerIsRiding.transform.Find("RidePos").position;
            // Update the grounded check for the animal the player is riding
            animalPlayerIsRidingGrounded = CheckIfTouchingObject(animalPlayerIsRiding.transform.Find("GroundCheck").transform.position, -Vector2.up, 0f, "Tile");
        }
        #endregion
    }

    void FixedUpdate()
    {
        // FixedUpdate() has the input and movement
        bool Left = Input.GetButton("Left") || Input.GetButton("dPadLeft");
        bool Right = Input.GetButton("Right") || Input.GetButton("dPadRight");
        bool Up = Input.GetButtonDown("Up") || Input.GetButtonDown("Jump");

        // If not riding
        if(!Riding)
        {
            // If pressing left
            if (Left)
            {
                // Move the player left 
                transform.Translate(-playerSpeed, 0, 0);
                // If grounded, flip the sprite, and run the walking animation
                sprRend.flipX = true;
                if (playerGrounded) anim.SetInteger("AnimState", 1);
            }
            // If pressing right
            else if (Right)
            {
                // Move the player right
                transform.Translate(playerSpeed, 0, 0);
                // If grounded, flip the sprite, and run the walking animation
                sprRend.flipX = false;
                if (playerGrounded) anim.SetInteger("AnimState", 1);
            }
            else if(!Up && playerGrounded)
            {
                // If not pressing any keys and the player is grounded, run the idle animation
                anim.SetInteger("AnimState", 0);
            }

            // Jumping
            if (Up)
            {
                // If the player is grounded
                if (playerGrounded)
                {
                    // Set the upwards velocity to zero, to counter the gravity
                    playerRigidBody2D.velocity = new Vector2(playerRigidBody2D.velocity.x, 0);
                    // Add force upwards to the rigidbody
                    playerRigidBody2D.AddForce(new Vector2(0, playerJumpSpeed), ForceMode2D.Force);
                    // Set the bool playerCanDoubleJump to true, since we have only jumped once
                    playerCanDoubleJump = true;
                }
                // Else if the playerCanDoubleJump is true
                else if (playerCanDoubleJump)
                {
                    // Set the playerCanDoubleJump to false
                    playerCanDoubleJump = false;
                    // Set the upwards velocity to zero, to counter gravity
                    playerRigidBody2D.velocity = new Vector2(playerRigidBody2D.velocity.x, 0);
                    // Add the force for jumping upwards
                    playerRigidBody2D.AddForce(new Vector2(0, playerJumpSpeed), ForceMode2D.Force);
                }
            }

            // This update on the animation is for special cases like falling
            if (!playerGrounded && !playerCanDoubleJump) anim.SetInteger("AnimState", 3);
            else if (!playerGrounded && playerCanDoubleJump) anim.SetInteger("AnimState", 2);

            // Update the enabled of the collider
            GetComponent<Collider2D>().enabled = true;         
        }
        // If riding an animal
        else
        {
            // Get the rigidbody2d of the animal the player is riding
            Rigidbody2D RigidBody2DOfAnimalPlayerIsRiding = animalPlayerIsRiding.GetComponent<Rigidbody2D>();   
            // Disable the player's collider
            GetComponent<Collider2D>().enabled = false;
            // If pressing left
            if (Left)
            {
                // Move the animal left 
                animalPlayerIsRiding.transform.Translate(playerSpeed * 1.5f, 0, 0);
                // If grounded, flip the sprite, and run the walking animation
                animalPlayerIsRiding.transform.localScale = new Vector2(-1, 1);
                sprRend.flipX = true;
                //if (playerGrounded) 
                animalPlayerIsRiding.GetComponent<Animator>().SetInteger("AnimState", 1);
            }
            // If pressing right
            else if (Right)
            {
                // Move the player right
                animalPlayerIsRiding.transform.Translate(playerSpeed*1.5f, 0, 0);
                animalPlayerIsRiding.transform.localScale = new Vector2(1, 1);
                sprRend.flipX = false;
                //if (playerGrounded) 
                animalPlayerIsRiding.GetComponent<Animator>().SetInteger("AnimState", 1);
            }
            else if (!Up && playerGrounded)
            {
                // If not pressing any keys and grounded, run the idle animation
                animalPlayerIsRiding.GetComponent<Animator>().SetInteger("AnimState", 0);
            }

            // Jumping
            if (Up)
            {
                // If the player is grounded
                if (animalPlayerIsRidingGrounded)
                {
                    // Set the upwards velocity to zero, to counter the gravity
                    RigidBody2DOfAnimalPlayerIsRiding.velocity = new Vector2(RigidBody2DOfAnimalPlayerIsRiding.velocity.x, 0);
                    // Add force upwards to the rigidbody
                    RigidBody2DOfAnimalPlayerIsRiding.AddForce(new Vector2(0, playerJumpSpeed*1.5f), ForceMode2D.Force);
                }
            }
            // Update the jump/falling animation
            if (!animalPlayerIsRidingGrounded)
            {
                if (RigidBody2DOfAnimalPlayerIsRiding.velocity.y > 0)
                {
                    animalPlayerIsRiding.GetComponent<Animator>().SetTrigger("JumpUp");
                }
                else if (RigidBody2DOfAnimalPlayerIsRiding.velocity.y < 0)
                {
                    animalPlayerIsRiding.GetComponent<Animator>().SetTrigger("JumpDown");
                }
            }
            
        }
    }

    void OnColliderStay(Collider other)
    {
        // If colliding with a Rideable animal, ignore the collision with it
        //if (other.tag == "RideableAnimal")
            //Physics2D.IgnoreCollision(transform.GetComponent<Collider2D>(), other.transform.GetComponent<Collider2D>(), true);
    }
}
