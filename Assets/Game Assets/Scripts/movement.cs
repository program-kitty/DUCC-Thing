using System.Collections;
using System.Data.Common;
using NUnit.Framework.Constraints;
using Unity.Mathematics;
using Unity.VisualScripting;
using Unity.VisualScripting.ReorderableList.Element_Adder_Menu;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class movement : MonoBehaviour
{
    //[SerializeField] GameManagerScript gameManager; 
     Rigidbody rb;
     int rotationStage = 0; 
    InputAction walkAction;
    int testCounter = 1; 
    InputAction jumpAction; 
    float speed = 5.0f; 
    bool isGrounded = true; 
    float jumpForce = 5f; 
    bool canMove = true;
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

    GameObject firstBullet; 
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
        rb = GetComponent<Rigidbody>();
         
    } 

    void Start()
    {
        stage = managerScript.stage; 
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
      
    Collider[] groundCollisions = Physics.OverlapBox(new Vector3(transform.position.x,transform.position.y - transform.localScale.y/2, transform.position.z), new Vector3(transform.localScale.x/2-0.05f, 0.18f, transform.localScale.z/2-0.05f), transform.rotation);
    if (groundCollisions.Length >= 2) //always contacting two parts: cylinder and player objects //UPDATE: now >= as bazooka doesn't have collider
        {
            isGrounded = true; //if more than that, then must be contacting floor
        } else 
        {
            isGrounded = false;
            for (int i=0; i<groundCollisions.Length; i++) //for everything in collision, checking if any of those we're 'standing' on is a bullet
            {
                Debug.Log(groundCollisions[i]);
            }
        }

        for (int i=0; i<groundCollisions.Length; i++) //for everything in collision, checking if any of those we're 'standing' on is a bullet
        {
            if (groundCollisions[i].gameObject.tag == "bullet")
            {
                if (groundCollisions[i].gameObject.GetComponent<bulletScript>().whoShot.gameObject.tag != this.gameObject.tag) //if its not your own bullets (bullet script has that info)
                {
                    if (firstBullet == null || firstBullet != groundCollisions[i].gameObject) //so you don't get propelled many times from one collision
                    {
                        rb.AddForce(Vector3.up*10f, ForceMode.Impulse);
                        isGrounded = false;//same stuff from jump function below (added more force because otherwise jumps were weak)
                        firstBullet = groundCollisions[i].gameObject; //makes sure only happens once per bullet
                    } 
                }
            }
        }


        //walk -- using this to immediately get joystick data, not wait for change
        Vector2 walkInput = walkAction.ReadValue<Vector2>(); //2 coords on movement
        

        //no longer using this now that rotation is implemented
        // //the 0f here would mess up a velocity/physics-based motion, so have to use translate instead
        // Vector3 playerMovement = new Vector3(walkInput.x * speed, 0f, walkInput.y * speed); //rb.linearVelocity.y doesn't work when jumping and moving at same time

        // playerMovement = Vector3.ClampMagnitude(playerMovement, speed); 
        // playerMovement.y = rb.linearVelocity.y; //if you add before clamp, then x/y coords mess up jump

        // //stable movement speed + world movement -- not needed for velocity-based movement
        // //playerMovement *= Time.deltaTime;  
        // playerMovement = transform.TransformDirection(playerMovement); 

                        
        //transform.Translate(playerMovement, Space.World); //used transform.translate because impulse force would not work otherwise!
        //Debugging Notes: 
            //rb.MovePosition(playerMovement); //same with rb position; even with interpolation and kinematics on, ends up dropping block halfway through ground, cannot move/jump is severely limited
            //rb.AddForce(playerMovement, ForceMode.VelocityChange); //has trouble with 'friction' - can't move unless in air
        
        
        
        //REMOVE THIS COMMENT BELOW FOR RB.LINEARVELOCITY = PLAYERMOVEMENT IN ORDER TO GET MOVEMENT BACK
        // rb.linearVelocity = playerMovement; //X/Z works when you comment out deltaTime; 0f y velocity impedes jump

        //rb.AddForce(playerMovement, ForceMode.Impulse); 

        //I hate to hardcode the rotations but: 
//Walkinput X, walkInput y
/* 

-X = Left (-90)
X = Right (90)
-Y = Backwards
Y = Forwards

Sideways --> 45 degrees add/subtract

*/

//*____________________________________________________________________________ROTATION AND MOVEMENT
float newRotationY = 0; 

    if (walkInput.x < 0f) //-X
        {
            if (walkInput.y == 0) //-X, 0Y 
            {
                newRotationY = -90f; //LEFT
                rotationStage = 6;
            } else if (walkInput.y >0 ) //-X, +Y
            {
                newRotationY = -45f; //LEFT-FORWARD
                rotationStage = 7;
            } else //-X, -Y
            {
                newRotationY = -135f; //LEFT-BACKWARD
                rotationStage = 5;
            }
        } else if (walkInput.x > 0) //+X
        {
            if (walkInput.y == 0) //+X, 0Y
            {
                newRotationY = 90f; //RIGHT
                rotationStage = 2;
            } else if (walkInput.y > 0) //+X, +Y
            {
                newRotationY = 45f; //RIGHT-FORWARD
                rotationStage = 1;
            } else //+X, -Y
            {
                newRotationY = 135f; //RIGHT-BACKWARD
                rotationStage = 3;
            }
            
        } else //0X
        {
            if (walkInput.y < 0) //0X, -Y
            {
                newRotationY = 180f; //BACKWARDS
                rotationStage = 4;
            } else if (walkInput.y > 0) //0X, +Y
            {
                newRotationY = 0f; //FOREWARDS
                rotationStage = 0; 
            } else //0X, 0Y
            {
                newRotationY = rotationStage * 45; //rotation stage used to figure out last position
                //Figure out how to leave them staying in their current rotation; 
            }
        }

        transform.rotation = Quaternion.Euler(0, newRotationY, 0);


        if (canMove)
        {
        if (walkInput.x == walkInput.y && walkInput.y == 0)
        {
            rb.linearVelocity = new Vector3(0,rb.linearVelocity.y,0);
        } else
        {
            Vector3 forward = new Vector3(transform.forward.x * speed, 0, transform.forward.z * speed); 
            forward = Vector3.ClampMagnitude(forward, speed); 
            forward = new Vector3(forward.x, rb.linearVelocity.y, forward.z); 

            rb.linearVelocity = forward;
        } 
        } else
        {
            float changeX = Mathf.Lerp(rb.linearVelocity.x, 0, Time.deltaTime);
            float changeZ = Mathf.Lerp(rb.linearVelocity.z, 0, Time.deltaTime);

            rb.linearVelocity = new Vector3(changeX, rb.linearVelocity.y, changeZ);
        }

       
        //maybe a sphere raycast to see what's nearby? Collision detection? 
        //Options: Sphere raycast detecting when to limit x/z axis movement in a certain direction; collision pushing back

        //this.transform.rotation = Quaternion.LookRotation(playerMovement,Vector3upwards = Vector3.up);

//        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(playerMovement), Time.deltaTime * 40f);
//alien.transform.rotation = Quaternion.Slerp (alien.transform.rotation, Quaternion.LookRotation (movementDirection), Time.deltaTime * 40f);
        
    }

    void OnJump(InputAction.CallbackContext context) {
        if (isGrounded)
        {
             Debug.Log("jump!");
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false; 
        }
    }


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
        
    }


    public void gotMoney()
    {
        money += 1;
    }

    public void gotBread(int x) //test this
    {
        Debug.Log("activated!" + x);
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

    public IEnumerator Death()
    {
        yield return new WaitForSeconds(0);
        SceneManager.LoadScene(1);
        testCounter += 1;
    }
    
    public IEnumerator spikeHit()
    {
        canMove = false; 
        yield return new WaitForSeconds(1f);
        canMove = true;

    }
}
