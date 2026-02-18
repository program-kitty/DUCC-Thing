using UnityEngine;

public class cameraMovements : MonoBehaviour
{
    [SerializeField] GameObject player; 
    float speed = 2.0f; 

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
        transform.position = new Vector3(Mathf.Lerp(transform.position.x, player.transform.position.x, Time.deltaTime * speed), 2.78f, Mathf.Lerp(transform.position.z, player.transform.position.z - 10f, Time.deltaTime * speed)); //follows player, stays behind them directly, height does not change
    }
}
