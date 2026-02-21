using UnityEngine;

public class bulletScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    void OnTriggerEnter(Collider other) //when bullets collide with crate, tells crate to 'break'
    {
        GameObject hit = other.gameObject; //more difficult to have crate detect; can detect a ton of things
        if (hit.tag == "crate") //crates prefab will have crate tag
        {
            BreakCrateScript hitScript = hit.GetComponent<BreakCrateScript>();
            hitScript.crateHit(); 
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
