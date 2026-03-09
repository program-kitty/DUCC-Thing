using System.Collections;
using System.Data.Common;
using NUnit.Framework.Constraints;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class movement : MonoBehaviour
{
    //[SerializeField] GameManagerScript gameManager; 
     Rigidbody rb;
    InputAction walkAction;
    int testCounter = 1; 
    InputAction jumpAction; 
    float speed = 5.0f; 
    bool isGrounded = true; 
    float jumpForce = 5f; 
    bool jumpNow = false; 
    public Vector3 playerMovement; //want to use as a public variable for camera movements
    public Vector3 finalMovements; 
    //Three Variables needed for Health and Money!! 
    int health = 3; 
    int maxHealth = 3;
    int money = 0; 
    public int stage = 0; 
    GameObject manager;
    GameManagerScript managerScript; 
    [SerializeField] Camera cam; 
    //once dying becomes an option, should use this (affected by checkpoints) to determine spawn location

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        manager = GameObject.FindWithTag("manager"); 
        managerScript = manager.GetComponent<GameManagerScript>();
        //setting up values using input system - new input system        
        walkAction = InputSystem.actions.FindAction("Move"); //in update - same as aim
        jumpAction = InputSystem.actions.FindAction("Jump"); //enable/disable - toggle
        //DontDestroyOnLoad(this.gameObject); 
        health = 3; 
         
    } 

    void Start()
    {
        stage = managerScript.stage; 
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; //prevents character from falling over when moving (was added when using velocity as motion, may not be required anymore but is good to have just in case)
        rb.transform.position = managerScript.spawnPoint; 
        cam.transform.position = new Vector3(managerScript.spawnPoint.x, 2.78f, managerScript.spawnPoint.z- 10f); 
    }

    void OnEnable() { //when you press the jump it doesn't repeat a million times
        jumpAction.started += OnJump; //performing
        //.started vs. .performed -- started is like onButtonDown - immediate response; .performed is whileButtonDown - takes second to respond
    }

    void OnDisable() { //when you press the jump it doesn't repeat a million times
        jumpAction.performed -= OnJump;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(isGrounded);
    //create raycast to detect floor below
    //uses Raycast to detect floor and distance from floor below - if troubleshooting is needed, click this link to see about collision-detecting instead: https://discussions.unity.com/t/detect-collision-from-bottom/12514
        //Debug.Log(health);
    //maybe try this: 
    //collision - check where collision is
    //see y location with respect to player y location 
    // if colliding with something below you, then you are grounded

//    RaycastHit hit; 
    //Ray downRay = new Ray(transform.position, -Vector3.up); //looks at below character
    // if (Physics.BoxCast(new Vector3(transform.position.x, transform.position.y - transform.localScale.y/2, transform.position.z), new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z), transform.up * -1, transform.rotation, 0.1f))
    //     {
    //         isGrounded = true;
    //         Debug.Log(isGrounded);
    //     } else
    //     {
    //         isGrounded = false;
    //     }

    // if (Physics.Raycast(downRay, out hit))
    //     {
    //         GameObject floor = hit.transform.gameObject; //used to see if we hit anything 
    //         if (floor != null) //if floor exists; just in case
    //         {
    //             if ((hit.distance)-0.05f <= transform.lossyScale.y/2 || rb.linearVelocity.y == 0) 
    //             //if distance from floor is over half the height of character (ray comes from halfway through character)
    //             {
    //                 isGrounded = true; //true if floor is close to body
    //             } else
    //             {
    //                 isGrounded = false; //false if floor is not close to body
    //             }
    //         } else //if floor doesn't exist
    //         {
    //             isGrounded = false; 
    //         }
    //     }
//(Vector3 center, Vector3 halfExtents, Quaternion orientation = Quaternion.identity,
//using this kind of jump because raycast wasn't detecting ground if the player wasn't directly over it (only checking below center of player)
    Collider[] groundCollisions = Physics.OverlapBox(new Vector3(transform.position.x,transform.position.y - transform.localScale.y/2, transform.position.z), new Vector3(transform.localScale.x/2-0.05f, 0.18f, transform.localScale.z/2-0.05f), transform.rotation);
    if (groundCollisions.Length > 2) //always contacting two parts: cylinder and player objects
        {
            isGrounded = true; //if more than that, then must be contacting floor
        } else 
        {
            isGrounded = false;
        }
        //walk -- using this to immediately get joystick data, not wait for change
        Vector2 walkInput = walkAction.ReadValue<Vector2>(); //2 coords on movement
        
        //the 0f here would mess up a velocity/physics-based motion, so have to use translate instead
        Vector3 playerMovement = new Vector3(walkInput.x * speed, 0f, walkInput.y * speed); //rb.linearVelocity.y doesn't work when jumping and moving at same time

        playerMovement = Vector3.ClampMagnitude(playerMovement, speed); 
        playerMovement.y = rb.linearVelocity.y; //if you add before clamp, then x/y coords mess up jump

        //stable movement speed + world movement -- not needed for velocity-based movement
        //playerMovement *= Time.deltaTime;  
        playerMovement = transform.TransformDirection(playerMovement); 
                        
        //transform.Translate(playerMovement, Space.World); //used transform.translate because impulse force would not work otherwise!
        //Debugging Notes: 
            //rb.MovePosition(playerMovement); //same with rb position; even with interpolation and kinematics on, ends up dropping block halfway through ground, cannot move/jump is severely limited
            //rb.AddForce(playerMovement, ForceMode.VelocityChange); //has trouble with 'friction' - can't move unless in air
        rb.linearVelocity = playerMovement; //X/Z works when you comment out deltaTime; 0f y velocity impedes jump
        //rb.AddForce(playerMovement, ForceMode.Impulse); 
       
        //maybe a sphere raycast to see what's nearby? Collision detection? 
        //Options: Sphere raycast detecting when to limit x/z axis movement in a certain direction; collision pushing back
    }

    void OnJump(InputAction.CallbackContext context) {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false; 
        }
    }

// void OnCollisionExit(Collision collision)
//     {
//         GameObject floor = collision.gameObject; 
//         if (collision.gameObject.transform.position.y < rb.transform.position.y-rb.transform.localScale.y)
//         {
//             isGrounded = false; 
//         } 
//         if (collision.gameObject.tag == "world")
//         {
//             isGrounded = false; 
//         }
//     }

    void OnCollisionEnter(Collision collision) //for colliding with money
    {//couldn't use on money or health itself as it wouldn't allow prefabs to access scripts inside player
    //raycast sphere? 
        if (collision.gameObject.tag == "money")
        {
            gotMoney();
            Destroy(collision.gameObject);
        } else if (collision.gameObject.tag == "loaf") //test these
        {
            gotBread(3);
            Destroy(collision.gameObject);
        } else if (collision.gameObject.tag == "slice") //test these
        {
            gotBread(1);
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.tag == "world")
        {
            isGrounded = true; 
        }
        // else if (collision.gameObject.transform.position.y < rb.transform.position.y-rb.transform.localScale.y)
        // {
        //     Debug.Log("IsTrue");
        //     isGrounded = true;
        // }
        
    }

    // void OnCollisionStay(Collision collision)
    // {
    //     // if (collision.gameObject.transform.position.y < rb.transform.position.y-rb.transform.localScale.y)
    //     // {
    //     //     isGrounded = true;
    //     // }
    //     // if (collision.gameObject.tag == "world")
    //     // {
    //     //     isGrounded = true;
    //     // } 
    // }


    public void gotMoney()
    {
        money += 1;
    }

    public void gotBread(int x) //test this
    {
        health += x; 
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        if (health <= 0)
        {
            StartCoroutine(Death());
        }
    }

    private IEnumerator Death()
    {
        yield return new WaitForSeconds(0);
        SceneManager.LoadScene(1);
        testCounter += 1;
    }
}
