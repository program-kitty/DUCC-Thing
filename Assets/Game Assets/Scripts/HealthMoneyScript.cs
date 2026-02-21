using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class HealthMoneyScript : MonoBehaviour
{
    private float bobHeight = 0.2f;
    private float counter = 0f;
    private float startHeight = 0.5f; 
    bool collided = false; 
    
    void Start()
    {
        counter += Random.Range(0f,10f); //so the bobs are randomized
    }

    void OnCollisionEnter(Collision collision)
    { //once it hits the floor, will start doing 'bob' animation
    //does not actually test floor, just any collision - may need debugging when testing later
        if (collision.gameObject.tag != "crate" && collision.gameObject.tag != "money")
        {
            collided = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (collided) //if it hits ground (or anything, really - maybe debug if needed here), will start bobbing animation
        {
            transform.position = new Vector3(transform.position.x, (Mathf.Sin(counter)*bobHeight)+startHeight, transform.position.z); 
        }

        counter+= 0.005f; //for slower and nicer animation of bobbing movement
        transform.Rotate(0f,0.1f,0f,Space.Self); //rotates
    }


}
