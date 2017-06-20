using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    CHICKEN,
    SNAKE,
    RATTLE_SNAKE,
    SPIDER,
    TARANTULA,
    BAT,
    SCORPION,
    CRAWDAD,
    LIZARD,
    VULTURE,
    PIRANHA,
    RAT,
    ARMADILLO,
    BANDIT
}

public class playerPickUp : MonoBehaviour
{
    public Dictionary<ObjectTypes,int> objectsDestroyed;
    public List<Bounty> bounties;
}
