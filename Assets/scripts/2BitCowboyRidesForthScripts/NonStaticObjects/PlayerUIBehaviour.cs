﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerUIBehaviour : MonoBehaviour {

	//[HideInInspector]
	//public Transform bountyUI;
	//[HideInInspector]
	//public Transform Bui;
	[HideInInspector]
	public Transform canvas;
	[HideInInspector]
	public bool paused;
	[HideInInspector]
	bool itsTime = false;

    private GameObject pauseMenus;

    private float originalSpeed = 0.15f;

    private float playerSpeed;

    private Text coinUI;

    private Text coinGUI;
    // Coin total
    public int Coins = 0;

    public bool end;

    private void Awake()
    {
        Coins = PlayerPrefs.GetInt("gold");
        coinUI = GameObject.FindGameObjectWithTag("pause coins").GetComponent<Text>();
        pauseMenus = FindObjectOfType<pauseMenu>().gameObject;
        // Turn the pause menu off
        pauseMenus.SetActive(false);
        playerSpeed = originalSpeed;
        coinGUI = FindObjectOfType<coinz>().GetComponent<Text>();
    }

    void Update()
	{
        coinUI.text = Coins.ToString();
        coinGUI.text = Coins.ToString();

        if (Input.GetKeyDown(KeyCode.Escape))
			Pause();
	}

	//void OnTriggerEnter2D(Collider2D other)
	//{
	//	////check for bounty
	//	//if (other.gameObject.GetComponent<bounty>())
	//	//{
	//	//	Bounty newBounty = new Bounty(
	//	//		int.Parse(gameObject.GetComponent<PDKObjectProperties>().objectProperties["numberRequirement"]), 
	//	//		int.Parse(gameObject.GetComponent<PDKObjectProperties>().objectProperties["numberRequirement"]), 
	//	//		int.Parse(gameObject.GetComponent<PDKObjectProperties>().objectProperties["objtypes"]));

	//	//	gameObject.GetComponent<playerPickUp>().bounties.Add(newBounty);
	//	//	//create notifications
	//	//	bountyUI = Instantiate(Bui, new Vector3(247, 300, 0), Quaternion.Euler(Vector3.zero), canvas);
	//	//	Invoke("pull", 0);
	//	//	Destroy(other.gameObject);
	//	//}
	//}
	//void PullNote()
	//{
	//	// Pulldown bounty notification
	//	if (itsTime == false)
	//	{
	//		if (bountyUI.position.y > 225)
	//		{
	//			bountyUI.Translate(Vector3.down * 5f);

	//			Invoke("pull", 0.05f);
	//		}
	//		else
	//		{
	//			itsTime = true;
	//			Invoke("pull", 1);
	//		}
	//	}
	//	else
	//	{
	//		if (bountyUI.position.y < 300)
	//		{
	//			bountyUI.Translate(-Vector3.down * 5f);
	//		}
	//		else
	//		{
	//			Destroy(bountyUI.gameObject);
	//		}
	//		Invoke("pull", 0.05f);
	//	}
	//}
    public void Pause()
    {
        
        if (paused == true)
        {
            playerSpeed = originalSpeed;
            pauseMenus.SetActive(false);
            GetComponent<PlayerController>().playerSpeed = 0.15f;
            Time.timeScale = 1;
            paused  = false;
            
        }
        else
        {
            pauseMenus.SetActive(true);
            pauseMenus.GetComponent<pauseMenu>().DisplayPauseMenu();

            if (end)
                pauseMenus.GetComponentInChildren<play>().gameObject.SetActive(false);
            Time.timeScale = 0f;
            GetComponent<PlayerController>().playerSpeed = 0;
            paused = true;
        }
    }

}
