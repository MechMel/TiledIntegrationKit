using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class genr8 : MonoBehaviour {

    public GameObject[] genies;
    public int setCount;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    void genes()
    {
        Instantiate(genies[Random.Range(0, genies.Length)]);
    }
}
