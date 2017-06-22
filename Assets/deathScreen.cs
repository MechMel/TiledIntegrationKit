using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class deathScreen : MonoBehaviour {

    int i;

	// Use this for initialization
	void OnEnable () {
        if (i == 1)
            Invoke("restart", 3);
	}
    void Start()
    {
        i++;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
