using System.Collections;
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
    // Coin total
    private int Coins = 0;

    private void Awake()
    {
        pauseMenus = FindObjectOfType<pauseMenu>().gameObject;
        // Turn the pause menu off
        pauseMenus.SetActive(false);
        playerSpeed = originalSpeed;
        coinUI = FindObjectOfType<coinz>().GetComponent<Text>();
    }

    void Update()
	{
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
            Time.timeScale = 1;
            paused = false;
            
        }
        else
        {
            pauseMenus.SetActive(true);
            Time.timeScale = 0f;
            playerSpeed = 0;
            paused = true;
        }
    }

}
