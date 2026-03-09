using Mono.Cecil.Cil;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    public int stage = 0; 
    [SerializeField] GameObject player; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static GameManagerScript instance; 
    public Vector3 spawnPoint; 
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        
        if (instance == null)
        {
            instance = this; 
        } 
        if (this != instance) 
        {
            Destroy(this.gameObject); //makes sure no duplicate scripts other than this 
        }
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(stage);
    }
    public void updateCheck(int checkPointNumber)
    {
        stage = checkPointNumber; //if they skip a checkpoint
    }
    public void newLocation(Vector3 location)
    {
        spawnPoint = location; //for character movement to know where to spawn
    }
}
