using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class clicketyClickety : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
public void forklieftEm(int levnum)
    {
        SceneManager.LoadScene(levnum+1);
    }
}
