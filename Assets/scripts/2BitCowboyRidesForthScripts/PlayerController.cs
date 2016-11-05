using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    // The PlayerController class is the basic controller for the player, and the base class for add-ons

    // The player speed
    [HideInInspector]
    public float playerSpeed = 1f;
    // The player jump speed
    [HideInInspector]
    public float playerJumpSpeed = 8f;
    // The player animation speed
    // NOT YET IMPLEMENTED
    [HideInInspector]
    public float playerAnimationSpeed = 8f;

    void Start ()
    {
	    
	}
	
	void Update ()
    {
	
	}
}
