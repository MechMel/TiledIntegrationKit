using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerPickUp : MonoBehaviour
{
    public Dictionary<ObjectTypes, int> objectsDestroyed;
    public Dictionary<ObjectTypes, Bounty> bounties;

    private void Awake()
    {
        // Instatiate the variabels
        objectsDestroyed = new Dictionary<ObjectTypes, int>();
        bounties = new Dictionary<ObjectTypes, Bounty>();

        #region Fill in Objects Destroyed with Empty Slots
        // Fill in Objects Destroyed with empty slots
        objectsDestroyed.Add(ObjectTypes.CRATE, 0);
        objectsDestroyed.Add(ObjectTypes.COIN_LARGE, 0);
        objectsDestroyed.Add(ObjectTypes.BARREL, 0);
        objectsDestroyed.Add(ObjectTypes.BOTTLE, 0);
        objectsDestroyed.Add(ObjectTypes.SHEEP, 0);
        objectsDestroyed.Add(ObjectTypes.CHICKEN, 0);
        objectsDestroyed.Add(ObjectTypes.SNAKE, 0);
        objectsDestroyed.Add(ObjectTypes.RATTLESNAKE, 0);
        objectsDestroyed.Add(ObjectTypes.SPIDER, 0);
        objectsDestroyed.Add(ObjectTypes.TARANTULA, 0);
        objectsDestroyed.Add(ObjectTypes.BAT, 0);
        objectsDestroyed.Add(ObjectTypes.SCORPION, 0);
        objectsDestroyed.Add(ObjectTypes.CRAWFISH, 0);
        objectsDestroyed.Add(ObjectTypes.LIZARD, 0);
        objectsDestroyed.Add(ObjectTypes.VULTURE, 0);
        objectsDestroyed.Add(ObjectTypes.PIRANHA, 0);
        objectsDestroyed.Add(ObjectTypes.RAT, 0);
        objectsDestroyed.Add(ObjectTypes.ARMADILLO, 0);
        objectsDestroyed.Add(ObjectTypes.BANDIT, 0);
        #endregion
    }

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
