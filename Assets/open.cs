using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class open : MonoBehaviour {

    public enum Buiding
    {
        SALOON,
        DOCTOR,
        CASINO,
        END
    }
    public Collider2D ot;
    public Buiding myBuilding;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            ot = other;
            #region end
            if (myBuilding == Buiding.END)
            {
                Debug.Log("hi");
                if (Input.GetKeyDown(KeyCode.E))
                {
                    Debug.Log("screwwwheeeehewwwwww you");
                    Invoke("DelayedStuff", 2);
                }
            }
            #endregion
        }
    }
    void DelayedStuff()
    {
        Debug.Log("goodbye world");
        ot.GetComponent<PlayerUIBehaviour>().end = true;
        ot.GetComponent<PlayerUIBehaviour>().Pause();
        Saver.Save(ot.gameObject);
    }
}
