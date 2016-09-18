using UnityEngine;
using System.Collections;

public class RamTestTextureSwap : MonoBehaviour {

    public Sprite sprite1;
    public Sprite sprite2;
    private int updateTimer;
    private int secondsTimer;
    private SpriteRenderer myRenderer;


	// Use this for initialization
	void Awake()
    {
        myRenderer = GetComponent<SpriteRenderer>();
    }
	
	// Update is called once per frame
	void FixedUpdate()
    {
        updateTimer += 1;

        if (updateTimer / 30 == 3)
        {
            if (myRenderer.sprite == sprite1)
            {
                myRenderer.sprite = sprite2;
            }
            else
            {
                myRenderer.sprite = sprite1;
            }
            updateTimer = 0;
        }
    }
}
