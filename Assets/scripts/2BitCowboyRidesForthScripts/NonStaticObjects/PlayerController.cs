using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    // The PlayerController class is the basic controller for the player, and the base class for add-ons

    #region Player
    // The player speed
    [HideInInspector]
    private float playerSpeed = 0.15f;
    // The player jump speed
    [HideInInspector]
    private float playerJumpSpeed = 1100f;
    // The player health
    [HideInInspector]
    public int playerHealth = 4;
    // Whether the player is grounded
    [HideInInspector]
    private bool playerGrounded = false;
    // Whether the player is touching a wall to the left
    [HideInInspector]
    private bool playerTouchingLeftWall = false;
    // Whether the player is touching a wall to the right
    [HideInInspector]
    private bool playerTouchingWall = false;
    // The player jump speed
    [HideInInspector]
    private bool playerCanDoubleJump = false;
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
    private float playerWaitToSlideTime = 0.7f;
    // The reload time of the player
    [HideInInspector]
    public float playerReloadTime = 0.7f;
    // The player animation speed
    // NOT YET IMPLEMENTED
    [HideInInspector]
    public float playerAnimationSpeed = 8f;
    // Whether the player is flipped or not
    [HideInInspector]
    public bool playerFacingLeft = true;
    // Whether the player is in water or not
    [HideInInspector]
    public bool playerIsInWater = false;
    // Whether the player was just in water or not as of last frame
    [HideInInspector]
    public bool playerWasInWaterPrevious = false;
    // Whether the player is touching the water line
    [HideInInspector]
    public bool playerTouchingWaterLine = false;
    // The upward velocity of the player
    private float playerVelocityY
    {
        get
        {
            return playerRigidBody2D.velocity.y;
        }
        set
        {
            playerRigidBody2D.velocity = new Vector2(playerRigidBody2D.velocity.x, value);
        }
    }
    #endregion
    #region Objects
    // The bullet prefab
    public GameObject bullet;
    // The nearest rideable animal
    [HideInInspector]
    public GameObject animalPlayerIsRiding;
    // Whether riding an animal or not
    [HideInInspector]
    public bool riding = false;
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
    // The midcheck GameObject, used for detecting the water line and such
    [HideInInspector]
    public GameObject midCheck;
    #endregion
   
    void Awake ()
    {
        // Hookup components
        anim = GetComponent<Animator>();
        playerRigidBody2D = GetComponent<Rigidbody2D>();
        sprRend = GetComponent<SpriteRenderer>();
        groundCheck = gameObject.transform.Find("GroundCheck").gameObject;
        rightWallCheck = gameObject.transform.Find("RightWallCheck").gameObject;
        leftWallCheck = gameObject.transform.Find("LeftWallCheck").gameObject;
        midCheck = gameObject.transform.Find("MidCheck").gameObject;
    }

    void Update()
    {
        // The built in update function in Unity runs every frame

        #region Input
        bool Left = Input.GetButton("Left") || Input.GetButton("dPadLeft");
        bool Right = Input.GetButton("Right") || Input.GetButton("dPadRight");
        bool Up = Input.GetButtonDown("Up") || Input.GetButtonDown("Jump");
        bool UpDown = Input.GetButton("Up") || Input.GetButton("Jump");
        bool Space = Input.GetButtonDown("Space");
        #endregion
        #region Collision Detection

        // Set the collisions by default off
        playerTouchingWall = false;
        playerGrounded = false;
        playerTouchingWaterLine = false;

        // Get the collision points
        Collider2D[] groundColliders = Physics2D.OverlapCircleAll(groundCheck.transform.position, 0.01f, 1 << LayerMask.NameToLayer("Solid"));
        Collider2D[] midColliders = Physics2D.OverlapCircleAll(midCheck.transform.position, 0.01f, 1 << LayerMask.NameToLayer("Solid"));
        Collider2D[] rightWallColliders = Physics2D.OverlapCircleAll(rightWallCheck.transform.position, 0.01f, 1 << LayerMask.NameToLayer("Solid"));

        // Make sure a collision exists
        if (groundColliders.Length > 0)
        {
            // Check for the player being grounded
            for (int i = 0; i < groundColliders.Length; i++)
            {
                if (groundColliders[i].gameObject != gameObject)
                {
                    playerGrounded = true;
                    break;
                }
            }
        }
        // Make sure a collision exists
        if (midColliders.Length > 0)
        {
            // Check for the player touching the water line
            for (int i = 0; i < midColliders.Length; i++)
            {
                if (midColliders[i].gameObject != gameObject && midColliders[i].gameObject.tag == "Water")
                {
                    playerTouchingWaterLine = true;
                    break;
                }
            }
        }
        // Make sure a collision exists
        if (rightWallColliders.Length > 0)
        {
            // Check for the player touching the right wall
            for (int i = 0; i < rightWallColliders.Length; i++)
            {
                if (rightWallColliders[i].gameObject != gameObject)
                {
                    playerTouchingWall = true;
                    playerCanDoubleJump = true;
                    break;
                }
            }
        }
        #endregion
        #region Movement
        // Update the flip of player
        if (playerFacingLeft)
            transform.localScale = new Vector3(-1, 1, 1);
        else 
            transform.localScale = new Vector3(1, 1, 1);
        // Update the playerIsInWater state
        if(playerIsInWater)
        {
            // If the player wasn't in water since last frame
            if (!playerWasInWaterPrevious)
                // We can assume the player has gotten out of the water, so reset the bool
                playerIsInWater = false;
            // If not pressing up
            if (!Up)
                // Set the upwards velocity to a minimal amount, to counter gravity and give floating effect
                playerRigidBody2D.velocity = new Vector2(playerRigidBody2D.velocity.x, -0.15f);
            // Update the animator

            // Reset the playerWasInWaterPrevious, so that we can catch next frame whether the player has gotten out of water
            playerWasInWaterPrevious = false;
        }

        // Update debug text
        //transform.Find("DebugText").GetComponent<TextMesh>().text = playerVelocityY.ToString();
        // If not riding
        if(!riding)
        {            
            // If pressing left
            if (Left)
            {
                // If in water
                if(playerIsInWater)
                {
                    // Move the player left at half speed
                    transform.Translate( (playerSpeed / 2) * transform.localScale.x, 0, 0);
                    // Flip the sprite
                    playerFacingLeft = true;
                }
                // If touching a wall, but not facing it, allow the player to move. Without this, the player could push against walls.
                else if (playerTouchingWall && !playerFacingLeft)
                {
                    // Move the player left
                    transform.Translate(playerSpeed * transform.localScale.x, 0, 0);
                    // Flip the sprite
                    playerFacingLeft = true;
                }
                // Otherwise, if not touching a wall
                else if(!playerTouchingWall)
                {
                    // Move the player left
                    transform.Translate(playerSpeed * transform.localScale.x, 0, 0);
                    // Flip the sprite
                    playerFacingLeft = true;
                }
                // If grounded, run the walking animation
                if (playerGrounded)
                    anim.SetInteger("AnimState", 1);
            }
            // If pressing right
            else if (Right)
            {
                // If in water
                if(playerIsInWater)
                {
                    // Move the player left at half speed
                    transform.Translate((playerSpeed / 2) * transform.localScale.x, 0, 0);
                    // Flip the sprite
                    playerFacingLeft = false;
                }
                // If touching a wall, but not facing it, allow the player to move. Without this, the player could push against walls.
                else if (playerTouchingWall && playerFacingLeft)
                {
                    // Move the player right
                    transform.Translate(playerSpeed * transform.localScale.x, 0, 0);
                    // Flip the sprite
                    playerFacingLeft = false;
                }
                // Otherwise, if not touching a wall
                else if (!playerTouchingWall)
                {
                    // Move the player right
                    transform.Translate(playerSpeed * transform.localScale.x, 0, 0);
                    // Flip the sprite
                    playerFacingLeft = false;
                }
                // If grounded, run the walking animation
                if (playerGrounded)
                    anim.SetInteger("AnimState", 1);
            }
            else if(!Up && playerGrounded)
            {
                // If not pressing any keys and the player is grounded, run the idle animation
                anim.SetInteger("AnimState", 0);
            }

            // If the player is under water
            if (playerIsInWater)
            {
                // If up is being held down
                if (UpDown)
                {
                    // Add force upwards to the rigidbody
                    playerRigidBody2D.AddForce(new Vector2(0, playerJumpSpeed / 4), ForceMode2D.Force);
                }
                else
                {
                    // Slowly fall
                    playerVelocityY -= 0.3f;
                }
            }
            // Jumping
            else if (Up)
            {                
                // Else if the player is grounded
                if (playerGrounded && !playerTouchingLeftWall && !playerTouchingWall)
                {
                    // Set the upwards velocity to zero, to counter the gravity
                    playerRigidBody2D.velocity = new Vector2(playerRigidBody2D.velocity.x, 0);
                    // Add force upwards to the rigidbody
                    playerRigidBody2D.AddForce(new Vector2(0, playerJumpSpeed), ForceMode2D.Force);
                    // Set the bool playerCanDoubleJump to true, since we have only jumped once
                    playerCanDoubleJump = true;
                }
                // Else if the playerCanDoubleJump is true
                else if (playerCanDoubleJump && !playerTouchingLeftWall && !playerTouchingWall)
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
            if (!playerGrounded && !playerCanDoubleJump)
                anim.SetInteger("AnimState", 3);
            else if (!playerGrounded && playerCanDoubleJump)
                anim.SetInteger("AnimState", 2);

            // If touching the ground, reset the wall sliding
            if (playerGrounded || !playerTouchingWall)
            {
                playerCanSlideOnce = false;
                playerCanSlide = false;
            }

            // Wall sliding
            if (playerTouchingWall && !playerGrounded)
            {
                // Set the animator state to wall sliding
                anim.SetInteger("AnimState", 4);
                // If the player can slide
                if (playerCanSlide)
                {
                    // Set the slide once to on, so the sliding doesn't continue to reset
                    playerCanSlideOnce = true;
                    // Set the sliding on the rigidBody2D
                    playerRigidBody2D.velocity = new Vector2(playerRigidBody2D.velocity.x, 0.7f);
                }
                // Else if the player can slide once, invoke the sliding to start
                else if(!playerCanSlideOnce)
                {
                    Invoke("WaitForSlide", playerWaitToSlideTime);
                    // Stop the falling
                    playerRigidBody2D.velocity = new Vector2(playerRigidBody2D.velocity.x, 1f);
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
            // Update the left and right input to the animal
            animalPlayerIsRiding.SendMessage("MoveLeft", Left, SendMessageOptions.DontRequireReceiver);
            animalPlayerIsRiding.SendMessage("MoveRight", Right, SendMessageOptions.DontRequireReceiver);
            

            // Jumping
            if (Up)
            {
                // If the player is grounded
                if (animalPlayerIsRidingGrounded)
                {
                    // Set the upwards velocity to zero, to counter the gravity
                    RigidBody2DOfAnimalPlayerIsRiding.velocity = new Vector2(RigidBody2DOfAnimalPlayerIsRiding.velocity.x, 0);
                    // Add force upwards to the rigidbody
                    RigidBody2DOfAnimalPlayerIsRiding.AddForce(new Vector2(0, playerJumpSpeed * 1.5f), ForceMode2D.Force);
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
                newBullet = Instantiate(bullet, new Vector2(transform.Find("BulletDownPos").transform.position.x, 
                    transform.Find("BulletDownPos").transform.position.y + Random.Range(-0.1f, 0.1f)), Quaternion.identity);
            else if(playerTouchingWall && !playerGrounded)
                newBullet = Instantiate(bullet, new Vector2(transform.Find("BulletPosOther").transform.position.x,
                    transform.Find("BulletPosOther").transform.position.y + Random.Range(-0.1f, 0.1f)), Quaternion.identity);
            else
                newBullet = Instantiate(bullet, new Vector2(transform.Find("BulletPos").transform.position.x,
                    transform.Find("BulletPos").transform.position.y + Random.Range(-0.1f, 0.1f)), Quaternion.identity);

            // Decide the direction and velocity of the new bullet, based on the direction of the player

            // If the player is in a double jump
            if (!playerGrounded && !playerCanDoubleJump)
            {
                // Set the bullet y velocity to -1    
                newBullet.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -10);
                // Rotate the bullet down
                newBullet.transform.Rotate(new Vector3(0, 0, 270));
            }     
            // Else if the player is touching the left wall
            else if (playerTouchingWall && playerFacingLeft && !playerGrounded)
            {
                // Set the bullet x velocity to 10
                newBullet.GetComponent<Rigidbody2D>().velocity = new Vector2(10, 0);
                // Add force upwards to the rigidbody
                playerRigidBody2D.AddForce(new Vector2(0, 300f), ForceMode2D.Force);
            }
            // Else if the player is touching the right wall
            else if (playerTouchingWall && !playerFacingLeft && !playerGrounded)
            {
                // Set the bullet x velocity to -10
                newBullet.GetComponent<Rigidbody2D>().velocity = new Vector2(-10, 0);
                // Rotate the bullet left
                newBullet.transform.Rotate(new Vector3(0, 0, 180));
                // Set the upwards velocity to zero, to counter the gravity
                playerRigidBody2D.velocity = new Vector2(playerRigidBody2D.velocity.x, 0);
            }
            // Else, if the player is facing left
            else if (playerFacingLeft)
            {
                // Set the bullet x velocity to -10
                newBullet.GetComponent<Rigidbody2D>().velocity = new Vector2(-10, 0);
                // Rotate the bullet left
                newBullet.transform.Rotate(new Vector3(0, 0, 180));
                // Move the player right as recoil
                transform.Translate(0.1f * -transform.localScale.x, 0, 0);
            }
            // Else if the player is facing right
            else if (!playerFacingLeft)
            {
                // Set the bullet x velocity to 10
                newBullet.GetComponent<Rigidbody2D>().velocity = new Vector2(10, 0);
                // Move the player left as recoil
                transform.Translate(0.1f * -transform.localScale.x, 0 , 0);
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
                riding = !riding;
                // Set the animal the player is riding
                animalPlayerIsRiding = GetNearestObjectInArray(GameObject.FindGameObjectsWithTag("RideableAnimal"));
            }
        }

        if (riding)
        {
            // Update position if riding       
            transform.position = animalPlayerIsRiding.transform.Find("RidePos").position;
            // Update the grounded check for the animal the player is riding
            animalPlayerIsRidingGrounded = CheckIfTouchingObject(animalPlayerIsRiding.transform.Find("GroundCheck").transform.position, -Vector2.up, 0f, "Tile");
        }
        #endregion
    }

    #region Wait Functions
    void ResetCanShoot()
    {
        playerCanShoot = true;
    }

    void WaitForSlide()
    {
        playerCanSlide = true;
    }
    #endregion
    #region Outside Invoke Functions
    void Hit(int damage)
    {
        Debug.Log("Player was just hit!");
        playerHealth -= damage;
    }
    void UpdateInWaterState(bool newWaterState)
    {
        // Updates the player's in water state

        // Set the newWaterState to the current water state and previous water state
        playerIsInWater = newWaterState;
        playerWasInWaterPrevious = newWaterState;
    }
    void HitWater()
    {
        // If the player's velocity is greater than or equal to 1
        if (playerRigidBody2D.velocity.y >= 1f)
        {
            // Translate upwards
            transform.Translate(0, 0.3f, 0);
            // Set the upwards velocity to zero, to counter the gravity
            playerRigidBody2D.velocity = new Vector2(playerRigidBody2D.velocity.x, 1.2f);
            // Add force upwards to the rigidbody
            playerRigidBody2D.AddForce(new Vector2(0, playerJumpSpeed), ForceMode2D.Force);
            // Set the bool playerCanDoubleJump to true, since we have only jumped once
            playerCanDoubleJump = true;
            // Update the in water state to false
            UpdateInWaterState(false);
        }        
    }

    void AddHealth(int amountOfHealthToAdd)
    {
        // Add the health, up to the max
        playerHealth = (int)Mathf.Clamp((playerHealth + amountOfHealthToAdd), 0f, 4f);
    }
    void AddCoin(int amountOfCoinToAdd)
    {
        // Add coins here
    }
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
