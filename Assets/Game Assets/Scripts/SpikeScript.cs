using UnityEditor.Callbacks;
using UnityEngine;


public class SpikeScript : MonoBehaviour
{
   // Start is called once before the first execution of Update after the MonoBehaviour is created
   void Start()
   {
      
   }


   void OnCollisionEnter(Collision collision)
   {
       GameObject hit = collision.gameObject;
       if (hit.tag =="Player")
       {
           movement movement = hit.GetComponent<movement>();
           if (movement.canMove) //as in, they weren't hit yet -- Needed because if you run hard enough, it deels more damage (up to 3 or more points of damage)
            {
                StartCoroutine(movement.spikeHit());
                movement.gotBread(-1);
                Rigidbody rb = hit.GetComponent<Rigidbody>();  //add this in once you figure out how to send player backwards when interacting with spike
                if (rb != null) {
                    Vector3 direction = rb.transform.forward;
                    direction *= -1f;
                    direction.y = 1f;
                    rb.AddForce(direction * 3f, ForceMode.Impulse);
                 }
            }
       } else if (hit.tag == "enemy")
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>(); 
            Vector3 pushDirection = Vector3.forward; 
            pushDirection = new Vector3(pushDirection.x, 1f, pushDirection.z);
            rb.AddForce(transform.up, ForceMode.Impulse);
            rb.AddForce(pushDirection * 2f, ForceMode.Impulse);        
        }
   }


   // Update is called once per frame
   void Update()
   {
      
   }
}