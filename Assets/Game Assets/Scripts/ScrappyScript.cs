using System;
using System.Collections;
using System.Data.Common;
using NUnit.Framework.Constraints;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ScrappyScript : MonoBehaviour
{
    [SerializeField] GameObject playerObject; 
    [SerializeField] movement playerScript; 
    Rigidbody rb;
    float speed = 3f;
    Vector3 movement; 
    float distance;
    bool canMove = false; 
    float slowDownSpeed = 5f; 
    float maxDistance = 10f;
    public int health = 3; 
    float counter = 1; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = this.gameObject.GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void Update()
    {
        Vector3 target = playerObject.transform.position; 
        Vector3 currentLocal = this.transform.position; 
        distance = Vector3.Distance(target,currentLocal);

        if (distance < maxDistance)
        {
            canMove = true; //so they start only after a certain distance
        }

        if (canMove && distance > 2 && health > 0)
        {
            movement = new Vector3(transform.forward.x, 0, transform.forward.z);
            movement *= speed;  
            movement = Vector3.ClampMagnitude(movement, speed); 
            movement.y = rb.linearVelocity.y;

            rb.linearVelocity = movement; 
        } else
        {
            movement.x = Mathf.Lerp(rb.linearVelocity.x, 0, Time.deltaTime * slowDownSpeed);
            movement.z = Mathf.Lerp(rb.linearVelocity.z, 0, Time.deltaTime * slowDownSpeed); 
            movement.y = rb.linearVelocity.y;
            rb.linearVelocity = (movement);
            //add damage/'scrapping' ability here! 
        }

        Vector3 look = new Vector3(playerObject.transform.position.x, this.transform.position.y, playerObject.transform.position.z);
        
        if (health > 0 && canMove)
        {
            transform.LookAt(look);
        }

        RaycastHit hit; 

        if (Physics.Raycast(transform.position, transform.forward, out hit, 2.5f)) //2.5 is maxdistance
        {
            if (hit.transform.gameObject != null)
            {
                if (hit.transform.gameObject.tag == "Player")
                {
                    counter += Time.deltaTime; 
                    if (counter >= 2)
                    {
                        playerScript.gotBread(-1);
                        Debug.Log("Punch!");
                        counter = 0; 
                    }
                } else
                {
                    counter = 2;
                }
            } else
            {
                counter = 2; 
            }
        }

    }

    public void gotShot()
    {
        health -= 1; 
        if (health <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
