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

public class ScrappyScript : MonoBehaviour {
    [SerializeField] GameObject playerObject; 
    [SerializeField] movement playerScript; 
    Rigidbody rb;
    Animator animator;
    
    float speed = 3f;
    Vector3 movement; 
    float distance;
    bool canMove = false; 
    float slowDownSpeed = 5f; 
    float maxDistance = 10f;
    public int health = 3; 
    bool canPunch = true; 
    float counter = 1;


    void Start() {
        rb = this.gameObject.GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();  // Get the component from the model's GameObject
    }

    void Update() {
        Vector3 target = playerObject.transform.position; 
        Vector3 currentLocal = this.transform.position; 
        distance = Vector3.Distance(target,currentLocal);

        if (distance < maxDistance && canPunch) {
            canMove = true; //so they start only after a certain distance
            animator.SetBool("walking", true);  // Play Animation
        } else {
            animator.SetBool("walking", false);
        }

        if (canMove && distance > 2 && health > 0) {
            movement = new Vector3(transform.forward.x, 0, transform.forward.z);
            movement *= speed;  
            movement = Vector3.ClampMagnitude(movement, speed); 
            movement.y = rb.linearVelocity.y;

            rb.linearVelocity = movement; 
        } else {
            movement.x = Mathf.Lerp(rb.linearVelocity.x, 0, Time.deltaTime * slowDownSpeed);
            movement.z = Mathf.Lerp(rb.linearVelocity.z, 0, Time.deltaTime * slowDownSpeed); 
            movement.y = rb.linearVelocity.y;
            rb.linearVelocity = (movement);
            //add damage/'scrapping' ability here! 
        }

        Vector3 look = new Vector3(playerObject.transform.position.x, this.transform.position.y, playerObject.transform.position.z);
        
        if (health > 0 && canMove) {
            transform.LookAt(look);
        }

        RaycastHit hit; 
        //Punch Mechanic
        if ( canPunch && Physics.Raycast(transform.position, transform.forward, out hit, 2.5f) ) { //2.5 is maxdistance 
            if (hit.transform.gameObject != null) {  // If object is in the way
                canPunch = false;  // Apply punch cooldown
                canMove = false;  // Stop the enemy from moving
                animator.SetTrigger("Attack");  // Play Animation
                string tag = hit.transform.gameObject.tag;  // Get tag of obstacle

                switch (tag) {
                    case "Player":
                        counter += Time.deltaTime; 
                        if (counter >= 2) {
                            StartCoroutine(punch());  // Attempt to hit the player with a punch
                        } else {
                            canMove = true;
                            canPunch = true;  // Allow the Scrappybara to punch again.
                        }
                        break;
                    case "crate":
                        counter += Time.deltaTime; 
                        if (counter >= 2f) {
                            BreakCrateScript crate = hit.transform.gameObject.GetComponent<BreakCrateScript>(); 
                            counter = 0f;
                            crate.crateHit();
                        }
                        StartCoroutine(punch());
                        break;
                    default:
                        StartCoroutine(punch());
                        counter = 0.1f;
                        break;
                }
            } else {
                canPunch = true;  // Allow the Scrappybara to punch again.
                counter = 0f;
            }
        }
    }

    IEnumerator punch() {  // Attempt to hit the player with a punch
        
        Debug.Log("Not Moving");
        yield return new WaitForSeconds(0.8f);
        
        // Test if punch landed.
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 2.5f) ) { //2.5 is maxdistance 
            if (hit.transform.gameObject != null) {  // If object is in the way
                string tag = hit.transform.gameObject.tag;  // Get tag of obstacle
                if (tag == "Player") {
                    playerScript.gotBread(-1);
                    Debug.Log("Took Damage!");
                }
            }
        }
        yield return new WaitForSeconds(1f);
        canMove = true;
        canPunch = true;  // Allow the Scrappybara to punch again.
    }
    

    public void gotShot() {
        animator.SetTrigger("Hit");  // Play Animation
        health -= 1; 
        if (health <= 0)
        {
            animator.SetTrigger("Die");  // Play Animation
            Destroy(this.gameObject);
        }
    }
}
