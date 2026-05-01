using System.Collections;
using System.Data.Common;
using System.Runtime.Serialization.Formatters;
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
    bool isGrounded = false; 
    bool isDying = false; 
    float jumpForce = 7f; //Marco was here. Buffing the jump height based on playtesting 
    bool jumpNow = false; 
    public Vector3 playerMovement; //want to use as a public variable for camera movements
    public Vector3 finalMovements; 
    //Three Variables needed for Health and Money!! 
    int health = 3; 
    int maxHealth = 3;
    bool canStomp = true;
    int money = 0; 
    public int stage = 0; 
    int rotationStage; 
    public bool canMove = true;
    GameObject manager;
    GameManagerScript managerScript; 
    [SerializeField] Camera cam; 

    Animator animator;
bool isStomping = false; 
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
        animator = GetComponentInChildren<Animator>();  // Get the component from the model's GameObject
    } 

    void Start()
    {
        Debug.Log(health);
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

        if (isDying)
        {
            transform.Rotate(0,0.5f,0);
            transform.localScale *= 0.999f;
            canMove = false; 
        }
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
        for (int i=0; i<groundCollisions.Length; i++) //for everything in collision, checking if any of those we're 'standing' on is a bullet
        {
            Debug.Log(groundCollisions[i] + "Number"+ groundCollisions.Length);
            if (groundCollisions[i].gameObject.tag == "bullet")
            {
                if (groundCollisions[i].gameObject.GetComponent<bulletScript>().shootingName != this.gameObject.tag) //if its not your own bullets (bullet script has that info)
                {
                    if (firstBullet == null || firstBullet != groundCollisions[i].gameObject) //so you don't get propelled many times from one collision
                    {
                        rb.AddForce(Vector3.up*10f, ForceMode.Impulse);
                        isGrounded = false;//same stuff from jump function below (added more force because otherwise jumps were weak)
                        firstBullet = groundCollisions[i].gameObject; //makes sure only happens once per bullet
                    } 
                }
            } else if (groundCollisions[i].gameObject.tag == "enemy" && isStomping)
            {
                ScrappyScript scrappyscript = groundCollisions[i].GetComponent<ScrappyScript>();
                ShootingEnemyScript shootingScript = groundCollisions[i].GetComponent<ShootingEnemyScript>();
                if (scrappyscript != null)
                {
                    scrappyscript.gotShot();
                } else if (shootingScript != null)
                {
                    shootingScript.gotShot();
                }
                isStomping = false; 
            } else if (groundCollisions[i].gameObject.tag == "crate" && isStomping)
            {
                BreakCrateScript crateScript = groundCollisions[i].GetComponent<BreakCrateScript>(); 
                crateScript.crateHit(); 
                isStomping = false; 
            }
        }
        if (groundCollisions.Length > 1) //always contacting player objects - bazooka is excluded
        {
            isGrounded = true; //if more than that, then must be contacting some sort of floor
            isStomping = false; 
        } else 
        {
            isGrounded = false;
        }


        //walk -- using this to immediately get joystick data, not wait for change
//         Vector2 walkInput = walkAction.ReadValue<Vector2>(); //2 coords on movement
        
        //this section has been removed because walking is now based on rotation/forward motion, not just force applied to a side
        // //the 0f here would mess up a velocity/physics-based motion, so have to use translate instead
         //Vector3 playerMovement = new Vector3(walkInput.x * speed, 0f, walkInput.y * speed); //rb.linearVelocity.y doesn't work when jumping and moving at same time

         //playerMovement = Vector3.ClampMagnitude(playerMovement, speed); 
         //playerMovement.y = rb.linearVelocity.y; //if you add before clamp, then x/y coords mess up jump

        // //stable movement speed + world movement -- not needed for velocity-based movement
        // //playerMovement *= Time.deltaTime;  
        // playerMovement = transform.TransformDirection(playerMovement); 
                        
        //transform.Translate(playerMovement, Space.World); //used transform.translate because impulse force would not work otherwise!
        //Debugging Notes: 
            //rb.MovePosition(playerMovement); //same with rb position; even with interpolation and kinematics on, ends up dropping block halfway through ground, cannot move/jump is severely limited
            //rb.AddForce(playerMovement, ForceMode.VelocityChange); //has trouble with 'friction' - can't move unless in air
        //rb.linearVelocity = playerMovement; //X/Z works when you comment out deltaTime; 0f y velocity impedes jump
        //rb.AddForce(playerMovement, ForceMode.Impulse); 
       
        //maybe a sphere raycast to see what's nearby? Collision detection? 
        //Options: Sphere raycast detecting when to limit x/z axis movement in a certain direction; collision pushing back

        //this.transform.rotation = Quaternion.LookRotation(playerMovement,Vector3upwards = Vector3.up);

//        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(playerMovement), Time.deltaTime * 40f);
//alien.transform.rotation = Quaternion.Slerp (alien.transform.rotation, Quaternion.LookRotation (movementDirection), Time.deltaTime * 40f);
        // the 0.01f is your input deadzone.

        Vector2 walkInput = walkAction.ReadValue<Vector2>(); //2 coords on movement
        
        if (! isDying)
        {
            if (walkInput.sqrMagnitude > 0.01) //model rotation
                {
                    Vector3 walkingCoord = new Vector3(walkInput.x, 0, walkInput.y);
                    Quaternion targetRotation = Quaternion.LookRotation(walkingCoord); //look at input
                    float rotationSpeed = 720f; // degrees per second

                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime); //rotates slowly rather than immediate movement
                }
        }

        // float newRotationY = 0;
//    if (walkInput.x < 0f) //-X
//        {
//            if (walkInput.y == 0) //-X, 0Y
//            {
//                newRotationY = 270; //LEFT
//                rotationStage = 6;
//            } else if (walkInput.y >0 ) //-X, +Y
//            {
//                newRotationY = 315; //LEFT-FORWARD
//                rotationStage = 7;
//            } else //-X, -Y
//            {
//                newRotationY = 225; //LEFT-BACKWARD
//                rotationStage = 5;
//            }
//        } else if (walkInput.x > 0) //+X
//        {
//            if (walkInput.y == 0) //+X, 0Y
//            {
//                newRotationY = 90f; //RIGHT
//                rotationStage = 2;
//            } else if (walkInput.y > 0) //+X, +Y
//            {
//                newRotationY = 45f; //RIGHT-FORWARD
//                rotationStage = 1;
//            } else //+X, -Y
//            {
//                newRotationY = 135f; //RIGHT-BACKWARD
//                rotationStage = 3;
//            }
          
//        } else //0X
//        {
//            if (walkInput.y < 0) //0X, -Y
//            {
//                newRotationY = 180f; //BACKWARDS
//                rotationStage = 4;
//            } else if (walkInput.y > 0) //0X, +Y
//            {
//                if (transform.localRotation.eulerAngles.y > 180)
//                {
//                    newRotationY = 360;
//                } else
//                {
//                    newRotationY = 0f; //FOREWARDS
//                }
//                rotationStage = 0;
//            } else //0X, 0Y
//            {
//                newRotationY = transform.localRotation.eulerAngles.y; //rotation stage used to figure out last position
//                //Figure out how to leave them staying in their current rotation;
//            }
//        }  
      
//        if (Mathf.Abs(transform.localRotation.eulerAngles.y - newRotationY) >= 180)
//        {
          
//        }

       //transitionRotation = Mathf.Lerp(transform.localRotation.eulerAngles.y, newRotationY, Time.deltaTime * 10);
       //transform.rotation = Quaternion.Euler(0,newRotationY, 0);

//____________________________________________________________________________________________________________________________________________________________ MOVEMENT
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
        
       
       
       
        #region Animation Controller
        bool isWalking = false;
        if (walkInput.x != 0 || walkInput.y != 0) { isWalking = true;}

        // Apply variables to Animator
        animator.SetBool("walking", isWalking);
        animator.SetBool("grounded", isGrounded);
        #endregion
    }

    void OnJump(InputAction.CallbackContext context) {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); //jump up, works if you're grounded
            isGrounded = false; 
        } else
        { //ground pound like Mario
            if (canStomp)
            {
                StartCoroutine(stompCooldown());
                isStomping = true; 
                rb.AddForce(Vector3.up * -jumpForce, ForceMode.Impulse); //if not grounded, then 'second jump' is actually a faster fall
            }

        }
    }

    public IEnumerator stompCooldown()
    {
        canStomp = false; 
        yield return new WaitForSeconds(1); 
        canStomp = true;
    }

    void OnCollisionEnter(Collision collision) //for colliding with money
    {//couldn't use on money or health itself as it wouldn't allow prefabs to access scripts inside player
    //raycast sphere? 
        if (collision.gameObject.tag == "money") //get money
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
        else if (collision.gameObject.tag == "world") //additional isGrounded check
        {
            isGrounded = true; 
        }
        
    }

    public IEnumerator spikeHit()
    {
        canMove = false; 
        yield return new WaitForSeconds(1.5f);
        canMove = true; 
    }
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

    public IEnumerator Death()
    {
        canMove = false; 
        isDying = true;
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(1);
        testCounter += 1;
    }
}
