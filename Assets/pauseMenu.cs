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
            // Get the reward and bounty completion status text components
            bountyTexts = bounties[bountyToDisplay].GetComponentsInChildren<Text>();
            // Set the reward text
            bountyTexts[0].text = playerPickUp.bounties[bountyToDisplay].reward.ToString() + "$";
            // If the player has not completed this bounty then display it normally
            if (playerPickUp.objectsDestroyed[bountyToDisplay] < playerPickUp.bounties[bountyToDisplay].numberRequirement)
            {
                bountyTexts[1].text = playerPickUp.objectsDestroyed[bountyToDisplay].ToString()
                    + " / "
                    + playerPickUp.bounties[bountyToDisplay].numberRequirement.ToString();
            }
            // If the player has not completed this bounty then display a check mark
            else
            {
                bountyTexts[1].text = "%";
            }
        }
    }

    private void Update()
    {
        // If more than 5 bounties have been collected then enable scrolling
        if (bounties.Count >= 5)
        {
            //  Enable scrolling with the mouse wheel
            contentRectTransform.localPosition += new Vector3(0, -100 * Input.GetAxis("Mouse ScrollWheel"), 0);
            // Stop the user from scrolling to far up
            if (contentRectTransform.localPosition.y < -300)
            {
                contentRectTransform.localPosition = new Vector3(
                    x: contentRectTransform.localPosition.x,
                    y: -300,
                    z: contentRectTransform.localPosition.z);
            }
            // Stop the user from scrolling to far up
            else if (contentRectTransform.localPosition.y > (bounties.Count * 60) - 520)
            {
                contentRectTransform.localPosition = new Vector3(
                    x: contentRectTransform.localPosition.x,
                    y: (bounties.Count * 60) - 520,
                    z: contentRectTransform.localPosition.z);
            }
        }
    }

    // When the quit button is pressed load the Main Menu
    public void quit()
    {
        SceneManager.LoadScene(0);
    }
}
