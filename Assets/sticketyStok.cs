using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sticketyStok : MonoBehaviour
{
    public GameObject thingymabob;
    public float distFromButton;

    // Use this for initialization
    void Start()
    {
    }
    public GameObject whatsitface;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(int.Parse(whatsitface.name) + 1);
        }
        if (transform.position.x < 397)
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                transform.Translate(Vector3.right * distFromButton);
            }
        if (transform.position.x > 238)
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                transform.Translate(-Vector3.right * distFromButton);
            }
        if (transform.position.y < 270)
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                transform.Translate(Vector3.up * distFromButton);
            }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (transform.position.y > 121)
            {
                transform.Translate(-Vector3.up * distFromButton);
            }
            else
            {
                gameObject.SetActive(false);
                thingymabob.SetActive(true);
            }
        }
    }
    void OnTriggerStay2D(Collider2D other)
    {
        whatsitface = other.gameObject;
    }
}
