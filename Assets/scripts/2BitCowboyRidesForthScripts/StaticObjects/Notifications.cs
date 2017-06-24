using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Notifications : MonoBehaviour
{
    public float secondsToDisplayNotification = 4;
    #region Bounty Notification Textures
    // Bounty notification textures
    public Sprite armadilloTexture;
    public Sprite batTexture;
    public Sprite chickenTexture;
    public Sprite crawfishTexture;
    public Sprite lizardTexture;
    public Sprite piranhaTexture;
    public Sprite ratTexture;
    public Sprite rattlesnakeTexture;
    public Sprite scorpionTexture;
    public Sprite sheepTexture;
    public Sprite snakesTexture;
    public Sprite spiderTexture;
    public Sprite tarantulaTexture;
    public Sprite vultureTexture;
    public Sprite blankTexture;
    #endregion
    private enum NotificationState { HIDDEN, LOWERING, DOWN, COMPLETED, RAISING };
    private NotificationState notificationState = NotificationState.HIDDEN;
    private Text bountyRewardText;
    private Text bountyStatusText;
    private Text bountyCoinCountText;
    private PlayerPickUp playerPickUp;
    private int timer = 0;
    private int coinsToCount = -1;
    private GameObject bountyUI;
    private LinkedList<Bounty> bountyNotificationsToDisplay;

    private void Awake()
    {
        // Create a game object for bounty notifications
        bountyUI = GameObject.Find("Bounty Notification");
        // Find the bounty texts
        bountyRewardText = GameObject.Find("Bounty Reward").GetComponent<Text>();
        bountyStatusText = GameObject.Find("Bounty Status").GetComponent<Text>();
        bountyCoinCountText = GameObject.Find("Bounty Coin Count").GetComponent<Text>();
        // Find the player pick up
        playerPickUp = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPickUp>();
        // Put the game object for bounty notifications in the right spot
        bountyUI.GetComponent<RectTransform>().position = new Vector3(Screen.width / 2, Screen.height * 1.1f, 0);
        // Instatitiate the list of bounty notifications to display
        bountyNotificationsToDisplay = new LinkedList<Bounty>();
    }

    // This displays a notification
    public void DisplayBounty(Bounty bounty)
    {
        // Add this bounty to the list of bounties to display
        bountyNotificationsToDisplay.AddLast(new LinkedListNode<Bounty>(bounty));
        
        // If there is no notification being displayed then immediately dispaly the new notification
        if (notificationState == NotificationState.HIDDEN)
        {
            // Setup the next notification
            SetUpNextNotification();
            // Lower the bounty notification
            notificationState = NotificationState.LOWERING;
        }
    }

    private void FixedUpdate()
    {
        // When appropriate lower, wait, and raise the notification
        switch (notificationState)
        {
            // Lowering the notification until it is all the way down
            case NotificationState.LOWERING:
                bountyUI.GetComponent<RectTransform>().position -= new Vector3(0, Screen.height / 140, 0);
                if (bountyUI.GetComponent<RectTransform>().position.y <= Screen.height * 0.875f)
                {
                    // If this bounty has not been complete display it normally
                    if (playerPickUp.objectsDestroyed[bountyNotificationsToDisplay.First.Value.objectType] < bountyNotificationsToDisplay.First.Value.numberRequirement)
                    {
                        notificationState = NotificationState.DOWN;
                    }
                    // If this bounty has been completed Display the completion animation
                    else if (playerPickUp.objectsDestroyed[bountyNotificationsToDisplay.First.Value.objectType] == bountyNotificationsToDisplay.First.Value.numberRequirement)
                    {
                        notificationState = NotificationState.COMPLETED;
                    }
                }
                break;

            // Incriment the timer until it is time to raise the notification
            case NotificationState.DOWN:
                timer++;
                if (timer >= secondsToDisplayNotification * 30)
                {
                    timer = 0;
                    notificationState = NotificationState.RAISING;
                }
                break;

            // Display the animation for the bounty completed notification
            case NotificationState.COMPLETED:
                // Display the bounty completed notification
                if (timer == 30)
                {
                    // Give this bounty notification the appropriate texture
                    bountyUI.GetComponent<Image>().sprite = blankTexture;
                    // Turn off unnessecary text
                    bountyRewardText.text = "";
                    bountyStatusText.text = "";
                    // Display the appropriate text for this state
                    bountyCoinCountText.text = "COMPLETED!";
                }
                // Set up the notification for counting down coins
                else if (timer == 90)
                {
                    // Prepare to start the reward coin count down
                    coinsToCount = bountyNotificationsToDisplay.First.Value.reward;
                    bountyCoinCountText.text = coinsToCount.ToString() + "$";
                }
                // Count down the coins for the reward
                else if (coinsToCount > 0)
                {
                    // Decrease the number of coins to counnt
                    coinsToCount--;
                    // Display the new number of coins to count
                    bountyCoinCountText.text = coinsToCount.ToString() + "$";
                }
                else if (coinsToCount == 0)
                {
                    // Reset the timer and coins to count
                    timer = 90;
                    coinsToCount = -1;
                }
                else if (timer == 120)
                {
                    // Reset the timer
                    timer = 0;
                    // Raise this notification
                    notificationState = NotificationState.RAISING;
                }
                // Inriment the timer
                timer++;
                break;

            // Raise the notification until it is out of the screen
            case NotificationState.RAISING:
                bountyUI.GetComponent<RectTransform>().position += new Vector3(0, Screen.height / 140, 0);
                if (bountyUI.GetComponent<RectTransform>().position.y >= Screen.height * 1.1f)
                {
                    /* This bounty notification has been displayed so remove it form the list
                     * of notifications to display */
                    bountyNotificationsToDisplay.Remove(bountyNotificationsToDisplay.First);
                    // If there is another notification to display set up the next bounty and lower it
                    if (bountyNotificationsToDisplay.First != null)
                    {
                        // Setup the next notification
                        SetUpNextNotification();
                        // Lower the bounty notification
                        notificationState = NotificationState.LOWERING;
                    }
                    // If there is not another notification to display, then do nothing
                    else
                    {
                        notificationState = NotificationState.HIDDEN;
                    }
                }
                break;

            default:
                break;
        }
    }

    // This returns the texture for the bounty notification for a object type
    private Sprite GetTextureForObjectType(ObjectTypes objectTypeToGetTextureFor)
    {
        // Return the appropriate texture for this object type
        switch (objectTypeToGetTextureFor)
        {
            case ObjectTypes.CRATE:
                return crawfishTexture;
            case ObjectTypes.COIN_LARGE:
                return crawfishTexture;
            case ObjectTypes.BARREL:
                return crawfishTexture;
            case ObjectTypes.BOTTLE:
                return crawfishTexture;
            case ObjectTypes.SHEEP:
                return sheepTexture;
            case ObjectTypes.CHICKEN:
                return chickenTexture;
            case ObjectTypes.SNAKE:
                return snakesTexture;
            case ObjectTypes.RATTLESNAKE:
                return rattlesnakeTexture;
            case ObjectTypes.SPIDER:
                return spiderTexture;
            case ObjectTypes.TARANTULA:
                return tarantulaTexture;
            case ObjectTypes.BAT:
                return batTexture;
            case ObjectTypes.SCORPION:
                return scorpionTexture;
            case ObjectTypes.CRAWFISH:
                return crawfishTexture;
            case ObjectTypes.LIZARD:
                return lizardTexture;
            case ObjectTypes.VULTURE:
                return vultureTexture;
            case ObjectTypes.PIRANHA:
                return piranhaTexture;
            case ObjectTypes.RAT:
                return ratTexture;
            case ObjectTypes.ARMADILLO:
                return armadilloTexture;
            case ObjectTypes.BANDIT:
                return crawfishTexture;
            default:
                return null;
        }
    }


    // This sets up the notifications for a given bounty
    private void SetUpNextNotification()
    {
        // Give this bounty notification the appropriate texture
        bountyUI.GetComponent<Image>().sprite = GetTextureForObjectType(bountyNotificationsToDisplay.First.Value.objectType);
        // Give this bounty notification the appropriate reward
        bountyRewardText.text = bountyNotificationsToDisplay.First.Value.reward.ToString();
        // Give this bounty notification the appropriate coimpletion status
        bountyStatusText.text = playerPickUp.objectsDestroyed[bountyNotificationsToDisplay.First.Value.objectType].ToString()
            + " / "
            + bountyNotificationsToDisplay.First.Value.numberRequirement.ToString();
    }
}
