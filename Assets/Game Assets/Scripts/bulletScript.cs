using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class bulletScript : MonoBehaviour
{
    bool firstCollide = true; 
    Rigidbody rb; 
    string shootingName;
    GameObject whoShot;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = this.GetComponent<Rigidbody>(); 
    }


    void OnTriggerEnter(Collider other) //when bullets collide with crate, tells crate to 'break'
    {
        GameObject hit = other.gameObject; //more difficult to have crate detect; can detect a ton of things
        if (firstCollide)
        {
            shootingName = hit.tag;
            whoShot = hit.gameObject;
            firstCollide = false; //to know who shot the bullet! 
        }

//this part needs to be cleaned up :(
        if (hit.tag == "enemy" && !(hit.tag == shootingName)) //if hits an enemy (and an enemy didn't shoot)
        {
            //decrease enemy health
            Debug.Log("Enemy Hit!");
        } 
        if (hit.tag == "enemy" && !(hit.gameObject == whoShot)) //
        {
            Destroy(this.gameObject); //if enemy hits other enemy- should we allow friendly fire?
        }
        if (hit.tag == "player" && !(hit.tag == shootingName))
        {
            //decrease player health
            Debug.Log("Player hit!");
        }

        if (hit.tag == "bullet")
        {
            Destroy(hit.gameObject); //destroys bullets on collision
        }
        if (hit.tag == "crate") //crates prefab will have crate tag
        {
            BreakCrateScript hitScript = hit.GetComponent<BreakCrateScript>();
            hitScript.crateHit(); 
        } else if (hit.tag == "checkpoint" && !(shootingName == "enemy")) //if player hits a checkpoint
        {
            Checkpoint checkpointScript = hit.GetComponent<Checkpoint>();
            checkpointScript.Checked(); //checkpoint ups stage counter on movememnt (unable to do that here)
        }
        if (hit.tag != shootingName && hit.tag != "money" && hit.tag != "loaf" && hit.tag != "slice") 
        {
            Destroy(this.gameObject);
        }
        
        //likely similar lines of code for enemies, walls, etc. 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
