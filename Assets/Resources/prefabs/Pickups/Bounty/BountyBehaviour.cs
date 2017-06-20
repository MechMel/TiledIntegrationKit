using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class BountyBehaviour : MonoBehaviour
{
    public Bounty bounty;


    // On awake get this bounties data from this object's properites
    private void Awake()
    {
        // Refrence to this object's properties
        PDKMap.PDKCustomProperties objectProperties = GetComponent<PDKObjectProperties>().objectProperties;

        // Get this bounty's requirement
        bounty.numberRequirement = int.Parse(objectProperties["Requirement"]);
        // Get this bounty's reward
        bounty.reward = int.Parse(objectProperties["Reward"]);
        // Get this bounty's object type
        switch (objectProperties["Type"])
        {
            case "Crate":
                bounty.objectType = ObjectTypes.CRATE;
                break;
            case "Coin_Large":
                bounty.objectType = ObjectTypes.COIN_LARGE;
                break;
            case "Barrel":
                bounty.objectType = ObjectTypes.BARREL;
                break;
            case "Bottle":
                bounty.objectType = ObjectTypes.BOTTLE;
                break;
            case "Sheep":
                bounty.objectType = ObjectTypes.SHEEP;
                break;
            case "Chicken":
                bounty.objectType = ObjectTypes.CHICKEN;
                break;
            case "Snake":
                bounty.objectType = ObjectTypes.SNAKE;
                break;
            case "Rattle_Snake":
                bounty.objectType = ObjectTypes.RATTLE_SNAKE;
                break;
            case "Spider":
                bounty.objectType = ObjectTypes.SPIDER;
                break;
            case "Tarantula":
                bounty.objectType = ObjectTypes.TARANTULA;
                break;
            case "Bat":
                bounty.objectType = ObjectTypes.BAT;
                break;
            case "Scorpion":
                bounty.objectType = ObjectTypes.SCORPION;
                break;
            case "Crawdad":
                bounty.objectType = ObjectTypes.CRAWDAD;
                break;
            case "Lizard":
                bounty.objectType = ObjectTypes.LIZARD;
                break;
            case "Vulture":
                bounty.objectType = ObjectTypes.VULTURE;
                break;
            case "Piranha":
                bounty.objectType = ObjectTypes.PIRANHA;
                break;
            case "Rat":
                bounty.objectType = ObjectTypes.RAT;
                break;
            case "Armadillo":
                bounty.objectType = ObjectTypes.ARMADILLO;
                break;
            case "Bandit":
                bounty.objectType = ObjectTypes.BANDIT;
                break;
        }
    }


    // If the player collides with this object, give the player this bounty and destroy this object
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            // Tell the canvas to display the notification
            FindObjectOfType<Canvas>().GetComponent<Notifications>().DisplayBounty(bounty);
            // Give the player this bounty
            other.gameObject.GetComponent<playerPickUp>().bounties.Add(bounty);
            // Destry this object
            Destroy(this);
        }
    }
}
