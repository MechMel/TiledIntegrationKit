using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class healthBar : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
        Debug.Log(FindObjectOfType<PlayerController>().playerHealth);
    }
    void deh()
    {
        GetComponent<Image>().fillAmount = FindObjectOfType<PlayerController>().playerHealth / 4f;
    }
}
