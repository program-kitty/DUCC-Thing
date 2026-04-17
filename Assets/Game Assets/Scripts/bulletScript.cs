using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.MPE;
using UnityEngine;
using UnityEngine.UIElements;

public class bulletScript : MonoBehaviour
{
    bool firstCollide = true; 
    Rigidbody rb; 
    public string shootingName;
    public GameObject whoShot;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = this.GetComponent<Rigidbody>(); 
    }


    void OnTriggerEnter(Collider other) //when bullets collide with crate, tells crate to 'break'
    {

        //sorry this script's a little messy :(
        
        GameObject hit = other.gameObject; //more difficult to have crate detect; can detect a ton of things
        // if (firstCollide)
        // {
        //     shootingName = hit.tag;
        //     whoShot = hit.gameObject;
        //     firstCollide = false; //to know who shot the bullet! 
        // }
        // Debug.Log(shootingName); //New mechanic - annything that instantiates this will also manually input tags and gameobject for whoShot and shootingName
//this part needs to be cleaned up :(
        if (hit.tag == "enemy" && !(hit.tag == shootingName)) //if hits an enemy (and an enemy didn't shoot)
        {
            //decrease enemy health
            //get script component, maybe name same type of function in both
            ShootingEnemyScript shootingScript = hit.gameObject.GetComponent<ShootingEnemyScript>();
            ScrappyScript scrappyScript = hit.gameObject.GetComponent<ScrappyScript>(); 
            if (shootingScript != null)
            {
                shootingScript.gotShot(); 
            } else if (scrappyScript!= null)
            {
                scrappyScript.gotShot(); 
            } //if you get more unique characters - make a variable for their script, check if null or not, same public void
            //is there a more efficient way to do this? unable to make a variable for script without mentioning specific script
        } 
        if (hit.tag == "enemy" && !(hit.gameObject == whoShot)) //
        {
            Destroy(this.gameObject); //if enemy hits other enemy- should we allow friendly fire?
        }
   
        if (hit.tag == "Player" && !(hit.tag == shootingName))
        {
            //decrease player health
            movement playerScript = hit.gameObject.GetComponent<movement>();
            if (playerScript == null)
            {
                playerScript = hit.transform.parent.GetComponent<movement>(); //when it hits cylinder, gives error - this fixes that
            }
            playerScript.gotBread(-1);    
        }

        if (hit.tag == "bullet")
        {
            Destroy(hit.gameObject); //destroys bullets on collision
        }
        if (hit.tag == "crate") //crates prefab will have crate tag
        {
            BreakCrateScript hitScript = hit.GetComponent<BreakCrateScript>();
            hitScript.crateHit(); 
        }
        if (hit.tag == "checkpoint" && !(shootingName == "enemy")) //if player hits a checkpoint
        {
            Checkpoint checkpointScript = hit.GetComponent<Checkpoint>();
//            Debug.Log(hit.GetComponent<Checkpoint>());  -- even for 'bugged' checkpoints it still finds the script
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
