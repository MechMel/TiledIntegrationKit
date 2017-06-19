using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sticketyStok : MonoBehaviour
{
    public GameObject thingymabob;
    public Vector3[] posers;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        for (int i = 0; i < posers.LongLength; i++)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (transform.position == posers[i])
                {
                    SceneManager.LoadScene(i + 2);
                }
            }
        }
        if (transform.position.x < 397)
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                transform.Translate(Vector3.right * 100);
            }
        if (transform.position.x > 97)
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                transform.Translate(-Vector3.right * 100);
            }
        if (transform.position.y < 298)
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                transform.Translate(Vector3.up * 100);
            }
        
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
            if (transform.position.y > 98)
            {
                transform.Translate(-Vector3.up * 100);
            }
            else
            {
                gameObject.SetActive(false);
                thingymabob.SetActive(true);
            }
            }
    }
}
