using System.Collections;
using System.Data.Common;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public class movement : MonoBehaviour
{
    Rigidbody rb;
    InputAction walkAction;
    InputAction jumpAction; 
    float speed = 5.0f; 
    bool isGrounded = true; 
    float jumpForce = 5f; 
    bool jumpNow = false; 
    public Vector3 playerMovement; //want to use as a public variable for camera movements
    public Vector3 finalMovements; 


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        //setting up values using input system - new input system        
        walkAction = InputSystem.actions.FindAction("Move"); //in update - same as aim
        jumpAction = InputSystem.actions.FindAction("Jump"); //enable/disable - toggle
    } 
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; //prevents character from falling over when moving (was added when using velocity as motion, may not be required anymore but is good to have just in case)
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
//create raycast to detect floor below
//uses Raycast to detect floor and distance from floor below - if troubleshooting is needed, click this link to see about collision-detecting instead: https://discussions.unity.com/t/detect-collision-from-bottom/12514
    RaycastHit hit; 
    Ray downRay = new Ray(transform.position, -Vector3.up); //looks at below character

    if (Physics.Raycast(downRay, out hit))
        {
            GameObject floor = hit.transform.gameObject; //used to see if we hit anything 
            if (floor != null) //if floor exists; just in case
            {
                if ((hit.distance)-0.05f <= transform.lossyScale.y/2) 
                //if distance from floor is over half the height of character (ray comes from halfway through character)
                {
                    isGrounded = true; //true if floor is close to body
                } else
                {
                    isGrounded = false; //false if floor is not close to body
                }
            } else //if floor doesn't exist
            {
                isGrounded = false; 
            }
            if (rb.linearVelocity.y == 0)
            {
                isGrounded = true;
            }
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
        }
    }

}
