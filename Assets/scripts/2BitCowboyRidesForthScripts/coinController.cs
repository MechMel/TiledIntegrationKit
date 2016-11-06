using UnityEngine;
using System.Collections;

public class coinController : MonoBehaviour
{
    Collider2D myCollider;


    void Awake()
    {
        // Find this object's collider
        myCollider = this.GetComponent<Collider2D>();
    }

    
    // When something collides with this object
    void OnTriggerEnter2D(Collider2D myCollider)
    {
        // Destroy this object
        Destroy(this.gameObject);
    }
}
