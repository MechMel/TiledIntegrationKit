using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dropCoins : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    public GameObject coinAlpha;
    public GameObject coinControl;
    public int coinCount;

    // OnDestroy is called once per destruction of Gameobject
    void OnDestroy()
    {
        Debug.Log(coinCount);
        while (coinCount > 0)
        {
            coinControl = Instantiate(coinAlpha, transform.position, transform.rotation);
            coinControl.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(1f, -1f), Random.Range(2f,10f));
            coinCount--;
        }
    }
}
