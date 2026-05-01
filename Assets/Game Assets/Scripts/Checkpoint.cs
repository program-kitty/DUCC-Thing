using UnityEditor.SceneManagement;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] movement movement; 
    [SerializeField] GameObject loaf; 
    //[SerializeField] GameManagerScript gameManagerScript; 
    public bool firstTimeHit; 
    float intensity = 2;
    int loafNumber = 3; 
    GameObject manager; 
    GameManagerScript gameManagerScript;
    public int checkPointNumber; 
    Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        firstTimeHit = true; //needs to be assigned value here, otherwise doesn't work
        manager = GameObject.FindWithTag("manager"); 
        gameManagerScript = manager.GetComponent<GameManagerScript>(); 
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Checked()
    {
        Debug.Log("CHECKPOINT"); //Marco was here.
        if (firstTimeHit)
        {
            if (gameManagerScript.stage < checkPointNumber)
            {
                gameManagerScript.updateCheck(checkPointNumber); //what stage they're at 
                gameManagerScript.newLocation(transform.position + transform.forward); //spawnpoint for character
            }
            //also add in healtlh impulse here! \/ \/ \/ \/
            for (int i = 0; i < loafNumber; i++) //This is for releasing each of the objects
            {
                Vector3 firstLocal = transform.position + transform.forward; //shoots the loaves out of vending machine
                firstLocal.y += 0.5f; 
                var loafDropped = Instantiate(loaf, firstLocal, transform.rotation);
                Rigidbody loafRb = loafDropped.GetComponent<Rigidbody>();             
                loafRb.AddForce(transform.forward, ForceMode.Impulse);  //a little launch upwards
            }

            firstTimeHit = false; //can only shoot once to gain rewards
            animator.SetBool("isPlayingAnimation", true); //leave the variable here, doesn't work well on bullet side
            //Marco was here
        }
        
    }


}
