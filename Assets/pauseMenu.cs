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
    public RectTransform contentRectTransform;
    public Transform veiwPortContent;
    //
    public GameObject uiObject;
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

    // When the pause menu is displayed this is called. This function updates and displays the bounties that have been collected
    public void DisplayPauseMenu()
    {
        // Used to get the Reward and Status texts
        Text[] bountyTexts;

        // Displays the bounties the player has collected
        foreach (ObjectTypes bountyToDisplay in playerPickUp.bounties.Keys)
        {
            // If a bounty object for this bounty type does not alread exist, create one
            if (!bounties.ContainsKey(bountyToDisplay))
            {
                // Create a new bounty object
                GameObject newBountyObject = Instantiate(blankBounty, veiwPortContent).gameObject;
                // Name this bounty object correctly
                newBountyObject.name = "Reward_" + bountyToDisplay.ToString();
                newBountyObject.GetComponent<Image>().sprite = uiObject.GetComponent<Notifications>().GetBountySpriteForObjectType(bountyToDisplay);
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

    private void Update()
    {
        if (bounties.Count >= 5)
        {
            //
            contentRectTransform.position += new Vector3(0, -100 * Input.GetAxis("Mouse ScrollWheel"), 0);
            //
            if (contentRectTransform.position.y < Screen.height / 4.6587f)
            {
                //
                contentRectTransform.position = new Vector3(
                    x: contentRectTransform.position.x,
                    y: Screen.height / 4.6587f,
                    z: contentRectTransform.position.z);
            }
            //
            else if (contentRectTransform.position.y > (bounties.Count * 60) - Screen.height / 3.625f)
            {
                //
                contentRectTransform.position = new Vector3(
                    x: contentRectTransform.position.x,
                    y: (bounties.Count * 60) - Screen.height / 3.625f,
                    z: contentRectTransform.position.z);
            }
        }
    }

    public void quit()
    {
        SceneManager.LoadScene(0);
    }
}
