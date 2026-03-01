using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] movement movement; 
    [SerializeField] GameObject loaf; 
    public bool firstTimeHit; 
    float intensity = 2;
    int loafNumber = 3; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        firstTimeHit = true; //needs to be assigned value here, otherwise doesn't work
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Checked()
    {
        if (firstTimeHit)
        {
            movement.stage += 1; //hopefully something to use to save the stage progress
            //also add in healtlh impulse here! \/ \/ \/ \/
            for (int i = 0; i < loafNumber; i++) //This is for releasing each of the objects
            {
                Vector3 firstLocal = transform.position + transform.forward; //shoots the loaves out of vending machine
                firstLocal.y += 0.5f; 
                var loafDropped = Instantiate(loaf, firstLocal, transform.rotation);
                Rigidbody loafRb = loafDropped.GetComponent<Rigidbody>();             
                loafRb.AddForce(transform.forward, ForceMode.Impulse);  //a little launch upwards
                
            }
            //leave the variable here, doesn't work well on bullet side
        }
        firstTimeHit = false; //can only shoot once to gain rewards
    }


}
