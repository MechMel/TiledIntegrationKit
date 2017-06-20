using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Notifications : MonoBehaviour
{
    public float secondsToDisplayNotification = 4;
    public Transform Bui;
    private enum NotificationState { HIDDEN, LOWERING, DOWN, RAISING };
    private NotificationState notificationState = NotificationState.HIDDEN;
    private int timer = 0;
    private Transform bountyUI;

    // This displays a notification
    public void DisplayBounty(Bounty bounty)
    {
        // If there is no notification being displayed then dispaly the new notification
        if (notificationState == NotificationState.HIDDEN)
        {
            //create notification
            bountyUI = Instantiate(Bui, new Vector3(247, 300, 0), Quaternion.Euler(Vector3.zero), FindObjectOfType<Canvas>().transform);
            // Pulldown bounty notification
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
                bountyUI.Translate(Vector3.down * 5f);
                if (bountyUI.position.y >= 225)
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
                bountyUI.Translate(Vector3.up * 5f);
                if (bountyUI.position.y <= 0)
                {
                    Destroy(bountyUI);
                    notificationState = NotificationState.HIDDEN;
                }
                break;

            default:
                break;
        }
    }
}
