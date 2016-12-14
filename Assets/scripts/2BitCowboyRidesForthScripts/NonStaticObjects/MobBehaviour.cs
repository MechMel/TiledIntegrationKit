using UnityEngine;
using System.Collections;

public class MobBehaviour : MonoBehaviour
{
    // The MobBehaviour class is the base class for all mobs

    // The MobType holds the possible mob types, which will be set in the Unity inspector
    public enum MobType
    {
        PATROL,
        CHASE,
        BLOCK
    }
    public MobType mobType;

    // Basic health
    public float health;
    // Basic damage
    public float damage;
    // Basic speed
    public float speed;
    // The check for whether the mob is affected by gravity
    public bool isAffectedByGravity;

    // With a PATROL mob, this will need to be set
    [Tooltip("The two points that the mob will patrol between. NOTE: Only necessary to set if the mob is a PATROL type.")]
    public Transform patrolPoint1;
    [Tooltip("The two points that the mob will patrol between. NOTE: Only necessary to set if the mob is a PATROL type.")]
    public Transform patrolPoint2;

    void Start()
    {

    }

    void Update()
    {

    }
}
