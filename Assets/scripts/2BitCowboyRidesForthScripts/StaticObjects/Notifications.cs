using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Notifications : MonoBehaviour
{
    public float secondsToDisplayNotification = 4;
    public Transform Bui;
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
    #endregion
    private enum NotificationState { HIDDEN, LOWERING, DOWN, RAISING };
    private NotificationState notificationState = NotificationState.HIDDEN;
    private Text bountyStatusText;
    private int timer = 0;
    private GameObject bountyUI;

    private void Awake()
    {
        // Create a game object for bounty notifications
        bountyUI = GameObject.Find("Bounty Notification");
        // Put the game object for bounty notifications in the right spot
        bountyUI.GetComponent<RectTransform>().position = new Vector3(Screen.width / 2, Screen.height * 1.1f, 0);
        // Find the bounty status text
        bountyStatusText = GameObject.Find("Bounty Status").GetComponent<Text>();
    }

    // This displays a notification
    public void DisplayBounty(Bounty bounty)
    {
        // If there is no notification being displayed then dispaly the new notification
        if (notificationState == NotificationState.HIDDEN)
        {
            // Give this notification the appropriate texture
            bountyUI.GetComponent<Image>().sprite = GetTextureForObjectType(bounty.objectType);
            // Give this bounty notification the appropriate status
            bountyStatusText.text = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPickUp>().objectsDestroyed[bounty.objectType].ToString()
                + " / "
                + bounty.numberRequirement.ToString();
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
                    notificationState = NotificationState.DOWN;
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

            // Raise the notification until it is out of the screen
            case NotificationState.RAISING:
                bountyUI.GetComponent<RectTransform>().position += new Vector3(0, Screen.height / 140, 0);
                if (bountyUI.GetComponent<RectTransform>().position.y >= Screen.height * 1.1f)
                {
                    notificationState = NotificationState.HIDDEN;
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
}
