using UnityEngine;

public class cameraMovements : MonoBehaviour
{
    [SerializeField] GameObject player; 
    float speed = 2.0f; 

    float targetX; 
    float targetY; 
    float targetZ; 
    public bool endGame = false; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(player.transform); //looks at player -- should be easy to try to adapt it to also be influenced by reticle? 
//ask about reticle

        //mathfLerps to make it a little more graceful

        if (endGame)
        {
            targetZ = player.transform.position.z - 3f;
            targetY = 0.4f; 
  
        } else
        {
            targetY = 2.78f + player.transform.position.y;
            targetZ = player.transform.position.z - 10f; //typical setup
        }
        targetX = player.transform.position.x;

        transform.position = new Vector3(Mathf.Lerp(transform.position.x, targetX, Time.deltaTime * speed), Mathf.Lerp(transform.position.y, targetY, Time.deltaTime * speed), Mathf.Lerp(transform.position.z, targetZ, Time.deltaTime * speed)); //follows player, stays behind them directly, height does not change
    }
}
