using System;
using System.Collections;
using System.Data.Common;
using NUnit.Framework.Constraints;
using TMPro;
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
    bool dying = false;
    Vector3 movement; 
    float distance;
    bool isPunching;
    bool canMove = false; 
    bool isJumping = false;  
    float slowDownSpeed = 5f; 
    float maxDistance = 10f;
    public int health = 3; 
    bool canPunch = false; 
    float counter = 1; 
    bool isGrounded = true; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = this.gameObject.GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void Update()
    {
        Collider[] groundCollisions = Physics.OverlapBox(new Vector3(transform.position.x,transform.position.y - transform.localScale.y/2, transform.position.z), new Vector3(transform.localScale.x/2-0.05f, 0.18f, transform.localScale.z/2-0.05f), transform.rotation);
        if (groundCollisions.Length > 1) //always contacting two parts: cylinder and player objects
        {
            isGrounded = true; //if more than that, then must be contacting floor
        } else 
        {
            isGrounded = false;
        }
        Debug.Log(isGrounded);

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
//Punch Mechanic
        if (Physics.Raycast(transform.position, transform.forward, out hit, 2.5f)) //2.5 is maxdistance 
        {

            if (hit.transform.gameObject != null)
            {
                string tag = hit.transform.gameObject.tag; 
                if (tag == "Player")
                {
                    counter += Time.deltaTime; 
                    if (counter >= 2)
                    {
                        StartCoroutine(punch());
                        if (canPunch)
                        {
                            playerScript.gotBread(-1);
                            isPunching = true;
                            Debug.Log("Punch!");
                            counter = 0f; 
                            canPunch = false; 
                            isPunching = false;
                        }
                    } 
                } else if (tag == "crate")
                {
                    counter += Time.deltaTime; 
                    if (counter >= 2f)
                    {
                        BreakCrateScript crate = hit.transform.gameObject.GetComponent<BreakCrateScript>(); 
                        if (canPunch)
                        {
                            counter = 0f;
                            isPunching = true; 
                            crate.crateHit();
                            isPunching = false; 
                            canPunch = false; 
                        }
                    }
                } else 
                {
                    counter = 0.1f;
                }
            } else
            {
                counter = 0f; 
            }
        }

    }

    IEnumerator punch()
    {
        canMove = false; 
        Debug.Log("Not Moving");
        yield return new WaitForSeconds(2); 
        canPunch = true; 
        yield return new WaitForSeconds(1);
        canMove = true;

    }
    

    public void gotShot()
    {
        health -= 1; 
        if (health <= 0)
        {
            StartCoroutine(scrappyDeath());
        }
    }

    IEnumerator scrappyDeath()
    {
//ANIMATION FOR DYING HERE ***
//dying bool
        dying = true;
        yield return new WaitForSeconds(1.5f); 
        dying = false; 
        Destroy(this.gameObject);

    }

//ANIMATIONS NOTES ** 
//use canMove, cappys will move until they die
// bool isPunching shows when you punch -- keep in mind, this is super buggy as is, so animation may not look right, 
//but i can work on fixing the punch mechanic later

//bool isGrounded shows when you're grounded (using same technique as player)
//bool dying for scrappy death

}
