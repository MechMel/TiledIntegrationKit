using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    // The PlayerController class is the basic controller for the player, and the base class for add-ons

    #region Player
    // The player speed
    [HideInInspector]
    private float playerSpeed = 0.2f;
    // The player jump speed
    [HideInInspector]
    private float playerJumpSpeed = 1100f;
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
    // Whether the player can shoot
    [HideInInspector]
    private bool playerCanShoot = true;
    // Whether the player can slide down a wall
    [HideInInspector]
    private bool playerCanSlide = false;
    // The run once variable for sliding
    [HideInInspector]
    private bool playerCanSlideOnce = false;
    // How long the player waits on the wall before sliding
    [HideInInspector]
    private float playerWaitToSlideTime = 3f;
    // The reload time of the player
    [HideInInspector]
    public float playerReloadTime = 0.3f;
    // The player animation speed
    // NOT YET IMPLEMENTED
    [HideInInspector]
    public float playerAnimationSpeed = 8f;
    // Whether the player is flipped or not
    [HideInInspector]
    public bool playerFlipped = true;
    #endregion
    #region Objects
    // The bullet prefab
    public GameObject bullet;
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
    #region Components
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
    /*
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
        
    }*/

    void Update()
    {
        // FixedUpdate() has the input and movement
        bool Left = Input.GetButton("Left") || Input.GetButton("dPadLeft");
        bool Right = Input.GetButton("Right") || Input.GetButton("dPadRight");
        bool Up = Input.GetButtonDown("Up") || Input.GetButtonDown("Jump");
        bool Space = Input.GetButtonDown("Space");

        #region Collision Detection

        // Set the collisions by default off
        playerTouchingLeftWall = false;
        playerTouchingRightWall = false;
        playerGrounded = false;

        // Get the collision points
        Collider2D[] groundColliders = Physics2D.OverlapCircleAll(groundCheck.transform.position, 0.2f);
        Collider2D[] leftWallColliders = Physics2D.OverlapCircleAll(leftWallCheck.transform.position, 0.2f);
        Collider2D[] rightWallColliders = Physics2D.OverlapCircleAll(rightWallCheck.transform.position, 0.2f);
        // Check for the player being grounded
        for (int i = 0; i < groundColliders.Length; i++)
        {
            if (groundColliders[i].gameObject != gameObject)
                playerGrounded = true;
        }
        // Check for the player touching the left wall
        for (int i = 0; i < leftWallColliders.Length; i++)
        {
            if (leftWallColliders[i].gameObject != gameObject)
            {
                playerTouchingLeftWall = true;
                playerCanDoubleJump = true;
            }
        }
        // Check for the player touching the right wall
        for (int i = 0; i < rightWallColliders.Length; i++)
        {
            if (rightWallColliders[i].gameObject != gameObject)
            {
                playerTouchingRightWall = true;
                playerCanDoubleJump = true;
            }
        }
        #endregion
        #region Movement
        // Update the flip of player
        if (playerFlipped)
            transform.localScale = new Vector3(-1, 1, 1);
        else
            transform.localScale = new Vector3(1, 1, 1);
        // If not riding
        if(!Riding)
        {            
            // If pressing left
            if (Left)
            {
                // Move the player left 
                transform.Translate(playerSpeed, 0, 0);
                // If grounded, flip the sprite, and run the walking animation
                playerFlipped = true;
                if (playerGrounded) anim.SetInteger("AnimState", 1);
            }
            // If pressing right
            else if (Right)
            {
                // Move the player right
                transform.Translate(playerSpeed, 0, 0);
                // If grounded, flip the sprite, and run the walking animation
                playerFlipped = false;
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
            if (!playerGrounded && !playerCanDoubleJump && !playerTouchingLeftWall && !playerTouchingRightWall)
                anim.SetInteger("AnimState", 3);
            else if (!playerGrounded && playerCanDoubleJump && !playerTouchingLeftWall && !playerTouchingRightWall)
                anim.SetInteger("AnimState", 2);

            // If touching the ground, reset the wall sliding
            if (playerGrounded)
                playerCanSlideOnce = false;

            // Wall sliding
            if (playerTouchingLeftWall && !playerGrounded || playerTouchingRightWall && !playerGrounded)
            {
                // Set the animator state to wall sliding
                anim.SetInteger("AnimState", 4);
                // If the player can slide
                if (playerCanSlide)
                {
                    // Set the slide once to on, so the sliding doesn't continue to reset
                    playerCanSlideOnce = true;
                    // Set the sliding on the rigidBody2D
                    playerRigidBody2D.velocity = new Vector2(playerRigidBody2D.velocity.x, 1.2f);
                }
                // Otherwise, invoke the sliding to start
                else if(!playerCanSlideOnce)
                {
                    Invoke("WaitForSlide", playerWaitToSlideTime);
                    // Stop the falling
                    playerRigidBody2D.velocity = new Vector2(playerRigidBody2D.velocity.x, 1.7f);
                }
                    
            }

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
                playerFlipped = true;
                //if (playerGrounded) 
                animalPlayerIsRiding.GetComponent<Animator>().SetInteger("AnimState", 1);
            }
            // If pressing right
            else if (Right)
            {
                // Move the player right
                animalPlayerIsRiding.transform.Translate(playerSpeed*1.5f, 0, 0);
                animalPlayerIsRiding.transform.localScale = new Vector2(1, 1);
                playerFlipped = false;
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
        #endregion
        #region Shooting

        // If space is pressed, and canShoot is true
        if(Space && playerCanShoot)
        {
            // Set the playerCanShoot back to false, so as not to have rapid fire
            //playerCanShoot = false;
            // Create a new bullet at the BulletPos of the player
            GameObject newBullet;
            if (!playerGrounded && !playerCanDoubleJump)
                newBullet = (GameObject)Instantiate(bullet, transform.Find("BulletDownPos").transform.position, Quaternion.identity);
            else
                newBullet = (GameObject)Instantiate(bullet, transform.Find("BulletPos").transform.position, Quaternion.identity);

            // Decide the direction and velocity of the new bullet, based on the direction of the player

            // If the player is in a double jump
            if (!playerGrounded && !playerCanDoubleJump)
            {
                // Set the bullet y velocity to -1    
                newBullet.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -10);
                // Rotate the bullet down
                newBullet.transform.Rotate(new Vector3(0, 0, 270));
            }
            // Else, if the player is facing left
            else if (playerFlipped || playerTouchingRightWall)
            {
                // Set the bullet x velocity to -10
                newBullet.GetComponent<Rigidbody2D>().velocity = new Vector2(-10, 0);
                // Rotate the bullet left
                newBullet.transform.Rotate(new Vector3(0, 0, 180));              
            }
            // Else if the player is facing right
            else if (!playerFlipped || playerTouchingLeftWall)
            {
                // Set the bullet x velocity to 10
                newBullet.GetComponent<Rigidbody2D>().velocity = new Vector2(10, 0);

            }
            // Invoke the reset for the playerCanShoot
            Invoke("ResetCanShoot", playerReloadTime);
        }
        #endregion
        
        #region Rideable animals
        // If close enough to a rideable animal
        if (Vector2.Distance(GetNearestObjectInArray(GameObject.FindGameObjectsWithTag("RideableAnimal")).transform.position, transform.position) < 3)
        {
            // If pressing the Interact key
            if (Input.GetButtonDown("Interact"))
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

    void ResetCanShoot()
    {
        playerCanShoot = true;
    }
    void WaitForSlide()
    {
        playerCanSlide = true;
    }

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

    void OnCollider2DStay(Collision2D other)
    {
        /*
        if (other.contacts.Length > 0)
        {
            // The first contact point
            ContactPoint2D contact = other.contacts[0];
            // If the contact point is below the player
            if (Vector3.Dot(contact.normal, Vector3.up) > 0.5)
            {
                playerGrounded = true;
            }
        }*/      
    }
}
