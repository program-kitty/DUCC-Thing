using UnityEngine;

public class VoidScript : MonoBehaviour
{
    [SerializeField] movement movement; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void OnCollisionEnter(Collision collision) 
    {
        if (collision.gameObject.tag == "Player")
        {
            //what to do when player hits it
            movement.gotBread(-100);
        }

        Destroy(this.gameObject); //for anything in the void
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
