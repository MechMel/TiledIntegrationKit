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
        RaycastHit2D objectTouching = Physics2D.Raycast(origin, direction, distance); Debug.Log(objectTouching.distance.ToString());
        // If the object hit not equal to null and the tag is the same as the argument
        if (objectTouching.collider != null && objectTouching.transform.tag == tagOfObject)
        {
            return true;
        }
        // If it doesn't, return false
        return false;
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
        // Update() has all the physic checks

        // Update gravity if touching wall
        if(playerTouchingLeftWall || playerTouchingRightWall)
        {
            playerRigidBody2D.velocity = new Vector2(playerRigidBody2D.velocity.x, -0.2f);
        }
        // Update the check if the player is grounded 
        if (CheckIfTouchingObject(groundCheck.transform.position, -Vector2.up, 0f, "Tile"))
        {
            playerGrounded = true;
            Debug.Log("Grounded!");
        }
        // Else, the player is not grounded
        else
        {
            playerGrounded = false;
            Debug.Log("Not grounded!");
        }
        

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
        
    }

    void FixedUpdate()
    {
        // FixedUpdate() has the input and movement
        bool Left = Input.GetButton("Left") || Input.GetButton("dPadLeft");
        bool Right = Input.GetButton("Right") || Input.GetButton("dPadRight");
        bool Up = Input.GetButtonDown("Up") || Input.GetButtonDown("Jump");

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
                anim.SetInteger("AnimState", 2);
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
                // Update animation
                anim.SetInteger("AnimState", 3);
            }
        }

        // This update on the animation is for special cases like falling
        if (!playerGrounded && !playerCanDoubleJump) anim.SetInteger("AnimState", 3);
        else if (!playerGrounded && playerCanDoubleJump) anim.SetInteger("AnimState", 2);
    }
}
