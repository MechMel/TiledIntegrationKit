﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vehicleTransport : MonoBehaviour
{

    public bool riding = false;
    public GameObject player;
    public enum dire { ONWARDS, STOP, BACKWARDS };
    public dire dir = dire.STOP;
    public float speed = 0.25f;
    private ParticleSystem.EmissionModule pe1;
    private ParticleSystem.EmissionModule pe2;
    private bool onGround=true;
    public Transform target;
    void Start()
    {
        pe1 = GetComponentInChildren<ParticleSystem>().emission;
        pe1.enabled = false;
        pe2 = GetComponentInChildren<ParticleSystem2>().GetComponent<ParticleSystem>().emission;
        pe2.enabled = false;
        //player = FindObjectOfType<PlayerController>().gameObject;
    }
    // Update is called once per frame
    void Update()
    {
        // Debug.DrawLine(new Vector3(transform.position.x, transform.position.y - 1.01f, transform.position.z), Vector3., Color.blue, 1);
        Debug.Log(dir);

        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y - 1.01f, transform.position.z), target.position, 0.1f))
        {
            if (riding)
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    GetComponent<Rigidbody2D>().velocity = (Vector2.up * 7);
                }

            if (Physics2D.Raycast(new Vector3(transform.position.x, transform.position.y - 1.01f, transform.position.z), target.position, 1).transform.gameObject.tag != "track")

            {


                //Debug.Log(Physics2D.Raycast(new Vector3(transform.position.x, transform.position.y - 1.01f, transform.position.z), -transform.up, 1).transform.name);

                if (speed > 0)
                {
                    if (dir == dire.BACKWARDS)
                    {
                        pe2.enabled = false;
                        pe1.enabled = true;
                    }
                    if (dir == dire.ONWARDS)
                    {
                        pe2.enabled = true;
                        pe1.enabled = false;
                    }
                    speed -= 0.003f;
                }
                else {
                    speed = 0;
                    pe2.enabled = false;
                    pe1.enabled = false;
                }
            }
        }

        if (dir != dire.STOP)
        {
            if (dir == dire.ONWARDS)
                transform.Translate(Vector3.right * speed * Time.timeScale);
            if (dir == dire.BACKWARDS)
                transform.Translate(Vector3.left * speed * Time.timeScale);
        }


        if (riding)
        {
            player.transform.parent = transform;
            player.transform.eulerAngles = Vector3.zero;
            player.GetComponent<PlayerController>().enabled = false;
            player.transform.localPosition = new Vector3(0f, 0.25f, 1);
            player.GetComponent<Rigidbody2D>().simulated = false;
            player.transform.localScale = transform.localScale;

            //  transform.

            if (Input.GetKey(KeyCode.E))
                player.transform.localPosition = new Vector3(2, 0.35f, 1);
            player.GetComponent<PlayerController>().enabled = true;
            player.GetComponent<Rigidbody2D>().simulated = true;
            player.transform.parent = null;
            riding = false;








            if (dir == dire.STOP)
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    transform.localScale = new Vector3(-1, 1, 1);
                    dir = dire.ONWARDS;
                    player.transform.localScale = transform.localScale;
                }
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    transform.localScale = new Vector3(1, 1, 1);
                    dir = dire.BACKWARDS;
                    player.transform.localScale = transform.localScale;
                }
            }
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name == "Player")
        {
            player = other.gameObject;
            riding = true;
        }
        else if (other.tag == "track")
        {
            onGround = true;
        }
        else
        {

        dir = dire.STOP;
        }
        //try detect player
        Debug.Log("pizza");
    }
    //void OnTriggerExit2D(Collider2D other)
    //{
    //    if (other.tag == "track")
    //    {
    //        onGround = false;
    //    }
    //}
}