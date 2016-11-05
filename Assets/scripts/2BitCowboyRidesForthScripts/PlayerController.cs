using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    // The PlayerController class is the basic controller for the player, and the base class for add-ons

    #region PLAYER
    // The player speed
    [HideInInspector]
    public float playerSpeed = 0.1f;
    // The player jump speed
    [HideInInspector]
    public float playerJumpSpeed = 500f;
    // The player health
    [HideInInspector]
    public int playerHealth = 4;
    // Whether the player is grounded
    [HideInInspector]
    public bool playerGrounded = false;
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
    #endregion

    void Start ()
    {
        // Hookup components
        anim = GetComponent<Animator>();
        playerRigidBody2D = GetComponent<Rigidbody2D>();
        sprRend = GetComponent<SpriteRenderer>();
        groundCheck = gameObject.transform.Find("GroundCheck").gameObject;
	}

    void Update()
    {
        // Update() has all the physic checks

        // Update the check if the player is grounded 
        // Create a ray pointing down
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.transform.position, -Vector2.up);
        // If the object hit not equal to null
        if (hit.collider != null)
        {
            // Calculate the distance to the object
            float distance = Mathf.Abs(hit.point.y - transform.position.y);
            //Debug.Log("Name: " + hit.rigidbody.ToString() + distance.ToString());
            // If the distance is less than 0.1f, the player must be grounded
            if(distance <= 1f)
            {
                playerGrounded = true;
            }
            // Else, the player is not grounded
            else
            {
                playerGrounded = false;
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
            Debug.Log("Up pressed");
            // If the player is grounded
            if (playerGrounded)
            {
                Debug.Log("playerGrounded = true");
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
                Debug.Log("playerCanDoubleJump = true");
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
