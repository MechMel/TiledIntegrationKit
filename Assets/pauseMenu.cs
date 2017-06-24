using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class pauseMenu : MonoBehaviour
{
    //
    public GameObject blankBounty;
    public GameObject viewPort;
    public Transform veiwPortContent;
    //
    private PlayerPickUp playerPickUp;
    //
    private Dictionary<ObjectTypes, GameObject> bounties;

	// Use this for initialization
	void Awake()
    {
        //
        playerPickUp = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPickUp>();
        //
        bounties = new Dictionary<ObjectTypes, GameObject>();
	}

    //
    public void DisplayPauseMenu()
    {
        //
        Text[] bountyTexts;

        // Displays the bounties the player has collected
        foreach (ObjectTypes bountyToDisplay in playerPickUp.bounties.Keys)
        {
            // If a bounty object for this bounty type does not alread exist, create one
            if (!bounties.ContainsKey(bountyToDisplay))
            {
                // Create a new bounty object
                GameObject newBountyObject = Instantiate(blankBounty, veiwPortContent).gameObject;
                // NAme this bounty object correctly
                newBountyObject.name = "Reward_" + bountyToDisplay.ToString();
                // Place this bounty object at the correct position
                newBountyObject.GetComponent<RectTransform>().localPosition = new Vector3(-192, 300 - (bounties.Count * 60), 0);
                // Add this bounty object to the list of bounty objects
                bounties.Add(bountyToDisplay, newBountyObject);
            }
            //
            bountyTexts = bounties[bountyToDisplay].GetComponentsInChildren<Text>();
            //
            bountyTexts[0].text = playerPickUp.bounties[bountyToDisplay].reward.ToString() + "$";
            //
            if (playerPickUp.objectsDestroyed[bountyToDisplay] < playerPickUp.bounties[bountyToDisplay].numberRequirement)
            {
                bountyTexts[1].text = playerPickUp.objectsDestroyed[bountyToDisplay].ToString()
                    + " / "
                    + playerPickUp.bounties[bountyToDisplay].numberRequirement.ToString();
            }
            //
            else
            {
                bountyTexts[1].text = "%";
            }
        }
    }

    public void quit()
    {
        SceneManager.LoadScene(0);
    }
}
