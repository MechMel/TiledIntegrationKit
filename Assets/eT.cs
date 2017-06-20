using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class eT : MonoBehaviour {

	// Use this for initialization
	void Start () {
        gameObject.GetComponentInChildren<Text>().text = gameObject.name;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
