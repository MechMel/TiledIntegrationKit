using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerPickUp : MonoBehaviour
{
    public Dictionary<ObjectTypes, int> objectsDestroyed;
    public Dictionary<ObjectTypes, Bounty> bounties;

    /* When an collectable object is destoryed it calls this function. 
     * This function then incriments the number of that object that have 
     * been destroyed, and displays and bounties for that object*/
    public void AddDestoryedObject(ObjectTypes destroyedObjectType)
    {
        // Increase the number of this object that has been destroyed
        objectsDestroyed[destroyedObjectType]++;
        // If the player has a bounty for this object type display the status of that bounty
        if (bounties.Keys.Contains(destroyedObjectType))
        {
            // Tell the canvas to display the status of this bounty
            FindObjectOfType<Canvas>().GetComponent<Notifications>().DisplayBounty(bounties[destroyedObjectType]);
        }
    }
}
