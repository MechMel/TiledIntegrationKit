using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class open : MonoBehaviour
{

    public enum Buiding
    {
        SALOON,
        DOCTOR,
        CASINO,
        END
    }
    #region Doc
    public GameObject DocsShop;
    #endregion
    #region bar
    public GameObject saloon;
    #endregion
    #region end
    public bool raise;
    public Transform flag;
    public Collider2D ot;
    #endregion
    #region casino
    public GameObject casino;

    #endregion
    public Buiding myBuilding;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        #region end
        if (raise)
        {
            if (flag.localPosition.y < 2.35f)
            {
                flag.Translate(Vector2.up * 0.05125f);
            }
        }
        #endregion
    }
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            ot = other;
            if (Input.GetKeyDown(KeyCode.E))
            {
                GetComponent<BoxCollider2D>().enabled = false;
                GetComponent<SpriteRenderer>().enabled = true;
                #region end
                if (myBuilding == Buiding.END)
                {
                    other.gameObject.GetComponent<PlayerController>().enabled = false;
                    other.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                    raise = true;
                    //Invoke("DelayedStuff", 0.5f);
                    Invoke("DelayedEnd", 2);
                }
                #endregion
                #region gambling House
                if (myBuilding == Buiding.CASINO)
                {
                    casino.SetActive(true);

                }
                #endregion
                #region Doc'n'Marty
                if (myBuilding == Buiding.DOCTOR)
                {
                    DocsShop.SetActive(true);

                }
                #endregion
                #region bar
                if (myBuilding == Buiding.SALOON)
                {
                    saloon.SetActive(true);
                }
                #endregion

                //Debug.Log("hi");

                //Debug.Log("screwwwheeeehewwwwww you");

            }
        }

    }

    //void DelayedStuff()
    //{
    //    ot.gameObject.GetComponent<SpriteRenderer>().enabled = false;
    //    GetComponent<SpriteRenderer>().enabled = true;

    //}

    void DelayedEnd()
    {
        //Debug.Log("goodbye world");
        ot.GetComponent<PlayerUIBehaviour>().end = true;
        ot.GetComponent<PlayerUIBehaviour>().paused = false;
        ot.GetComponent<PlayerUIBehaviour>().Pause();
        Saver.Save(ot.gameObject);
    }
}
