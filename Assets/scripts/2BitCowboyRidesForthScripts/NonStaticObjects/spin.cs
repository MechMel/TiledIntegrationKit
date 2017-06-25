using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spin : MonoBehaviour {

    Rigidbody2D rb;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();

    }
    public float spinner;
    bool NO = false;
    public int[] reward;
    int I = 5;
    public GameObject ticker;
    // Update is called once per frame
    void Update() {
        //for (int i = 0; i < 10; i++) {
        //    if (transform.eulerAngles.z >  35f && transform.eulerAngles.z < 37f)
        //        ticker.transform.eulerAngles = new Vector3(0,0,-20);
        //    else
        //        ticker.transform.eulerAngles = Vector3.zero;
        //}
        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log(transform.rotation.z + "current");
            Debug.Log(spinner + "previous");
            GetComponent<Rigidbody2D>().AddTorque((transform.rotation.z*10 - spinner), ForceMode2D.Impulse);
            NO = true;
        }
        if (Input.GetMouseButton(0))
        {
            if (NO == false)
            {
                transform.LookAt(Input.mousePosition);
            }
        }
        else
        {
            GetComponent<Rigidbody2D>().AddTorque(-GetComponent<Rigidbody2D>().angularVelocity * 0.005f, ForceMode2D.Force);
        }
        if (NO == true)
        {
            if (GetComponent<Rigidbody2D>().angularVelocity == 0)
            {
                FindObjectOfType<PlayerController>().AddCoin(reward[(int)transform.localEulerAngles.z /36]);
                Invoke("KickOut", 1);
                NO = false;
            }
        }

        if (I == 0)
        {
            spinner = transform.rotation.z;
            I=5;
        }
        I--;
    }
    void KickOut()
    {
        transform.parent.gameObject.SetActive(false);
    }
}
