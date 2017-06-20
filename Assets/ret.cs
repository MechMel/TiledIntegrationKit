using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ret : MonoBehaviour {

    public GameObject dowhatsit;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            dowhatsit.SetActive(true);
            gameObject.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(14);
        }
	}
}
