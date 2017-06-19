using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerPickUp : MonoBehaviour {


    public class Bounty
    {

        // Bounty stats
        public enum ObjectTypes
        {
            CRATE,
            COIN_LARGE,
            BARREL,
            DOOR,
            COIN,
            BOTTLE,
            CRATE_COIN,
            CRATE_SCORPIAN,
            CRATE_HEALTH,
            CRATE_EXPLOSION,
            HEALTH,
            SHEEP,
            CHICKEN
        }

        public int numberRequirement;
        public int reward;
        public int objectType;
        public Bounty()
        {

        }
        public Bounty(int numberRequirement, int reward, int objectType)
        {
        
        }
    }

    

    public List<Bounty> bounties;
    //public bool bounty;

    //public enum ObjectTypes
    //{
    //    CRATE,
    //    COIN_LARGE,
    //    BARREL,
    //    DOOR,
    //    COIN,
    //    BOTTLE,
    //    CRATE_COIN,
    //    CRATE_SCORPIAN,
    //    CRATE_HEALTH,
    //    CRATE_EXPLOSION,
    //    HEALTH,
    //    SHEEP,
    //    CHICKEN
    //}

    //public Dictionary<ObjectTypes,int> objectsDestroyed;
    //public Dictionary<ObjectTypes, int> bountifulBounties;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        
	}
}
