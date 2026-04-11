using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class HealthMoneyScript : MonoBehaviour
{
    private Rigidbody rb; 
    private float bobHeight = 0.2f;
    private float counter = 0f;
    private float startHeight = 0.5f; 
    bool collided = false; 
    
    void Start()
    {
        counter += Random.Range(0f,10f); //so the bobs are randomized
        rb = gameObject.GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    { //once it hits the floor, will start doing 'bob' animation
    //does not actually test floor, just any collision - may need debugging when testing later
        if (collision.gameObject.tag != "crate" && collision.gameObject.tag != "money")
        {
            collided = true;
            startHeight = this.gameObject.transform.localPosition.y + 0.5f; 
            StartCoroutine(Despawn()); 
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (collided) //if it hits ground (or anything, really - maybe debug if needed here), will start bobbing animation
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            transform.position = new Vector3(transform.position.x, (Mathf.Sin(counter)*bobHeight)+startHeight, transform.position.z); 
        }
        if (this.transform.rotation.eulerAngles.x != 0 || this.transform.rotation.eulerAngles.z != 0)
        {
            transform.rotation = Quaternion.Euler(0,0,0); //had a habit of turning around in air when affected by physics; prevents rotations
        }

        counter+= 0.005f; //for slower and nicer animation of bobbing movement
        transform.Rotate(0f,0.1f,0f,Space.World); //rotates around world, so stops revolving around istelf if sideways
    }

    private IEnumerator Despawn()
    {
        yield return new WaitForSeconds(20); //to avoid overcrowding scene if players don't pick up everything
        Destroy(this.gameObject); 
    }
}
