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
        // Create a ray pointing down
        RaycastHit2D bottomWall = Physics2D.Raycast(groundCheck.transform.position, -Vector2.up);
        // If the object hit not equal to null
        if (bottomWall.collider != null)
        {
            // Calculate the distance to the object
            float distance = Mathf.Abs(bottomWall.point.y - transform.position.y);
            //Debug.Log("Name: " + hit.rigidbody.ToString() + distance.ToString());
            // If the distance is less than 0.1f, the player must be grounded
            if(distance <= 1)
            {
                playerGrounded = true;
            }
            // Else, the player is not grounded
            else
            {
                playerGrounded = false;
            }
        }

        // Update the check if the player is touching 
        // Create a ray pointing down
        RaycastHit2D leftWall = Physics2D.Raycast(leftWallCheck.transform.position, -Vector2.right);
        // If the object hit not equal to null
        if (leftWall.collider != null)
        {
            // Calculate the distance to the object
            float distanceToLeftWall = Mathf.Abs(Vector2.Distance(transform.position, leftWall.point));
            // If the distance is less than 1f
            if (distanceToLeftWall <= 0.65f)
            {
                playerTouchingLeftWall = true;
                playerCanDoubleJump = true;
            }
            else
            {
                playerTouchingLeftWall = false;
            }
        }
        // Update the check if the player is touching the right wall
        // Create a ray pointing right
        RaycastHit2D rightWall = Physics2D.Raycast(rightWallCheck.transform.position, Vector2.right);
        // If the object hit not equal to null
        if (rightWall.collider != null)
        {
            // Calculate the distance to the object
            float distanceToRightWall = Mathf.Abs(Vector2.Distance(transform.position, rightWall.point));
            // If the distance is less than 1f
            if (distanceToRightWall <= 0.65f)
            {
                playerTouchingLeftWall = true;
                playerCanDoubleJump = true;
            }
            else
            {
                playerTouchingLeftWall = false;
            }
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
        }
        // If pressing right
        else if (Right)
        {
            // Move the player right
            transform.Translate(playerSpeed, 0, 0);
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
    }
}
