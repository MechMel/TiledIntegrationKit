using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStart : MonoBehaviour
{

	// When the game starts move the player to this position
	void Awake()
    {
        GameObject.FindGameObjectWithTag("Player").transform.position = transform.position;
	}
}
