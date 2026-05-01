using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class BreakCrateScript : MonoBehaviour
{
    public int toastNumber = 0; 
    public int loafNumber = 0; 
    public int moneyNumber = 0; 
    [SerializeField] GameObject money; 
    [SerializeField] GameObject toast; 
    [SerializeField] GameObject loaf; 
    float intensity = 2f; 
    bool firstHit = true; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void crateHit()
    {
        Destroy(this.gameObject); //'breaks' the crate
        for (int i = 0; i < moneyNumber; i++) //This is for releasing each of the objects
        {
            var moneyDropped = Instantiate(money, transform.position, transform.rotation);
            Rigidbody moneyRb = moneyDropped.GetComponent<Rigidbody>(); 
            //add some randomness to the upwards/diagonal, so they move and are separate to each other
            Vector3 randomPush = new Vector3(Random.Range(-2f,2f), transform.up.y * intensity, Random.Range(-2f, 2f)); 
            moneyRb.AddForce(randomPush, ForceMode.Impulse);  //a little launch upwards
            
        } //Once Health models are officially added in (or if I want to make a temp spot), I can just copy/paste this same stuff over
        for (int i = 0; i < loafNumber; i++) //This is for releasing each of the objects
        {
            var loafDropped = Instantiate(loaf, transform.position, transform.rotation);
            Rigidbody loafRb = loafDropped.GetComponent<Rigidbody>(); 
            //add some randomness to the upwards/diagonal, so they move and are separate to each other
            Vector3 randomPush = new Vector3(Random.Range(-2f,2f), transform.up.y * intensity, Random.Range(-2f, 2f)); 
            loafRb.AddForce(randomPush, ForceMode.Impulse);  //a little launch upwards
        } 
        for (int i = 0; i < toastNumber; i++) //This is for releasing each of the objects
        {
            var toastDropped = Instantiate(toast, transform.position, transform.rotation);
            Rigidbody toastRb = toastDropped.GetComponent<Rigidbody>(); 
            //add some randomness to the upwards/diagonal, so they move and are separate to each other
            Vector3 randomPush = new Vector3(Random.Range(-2f,2f), transform.up.y * intensity, Random.Range(-2f, 2f)); 
            toastRb.AddForce(randomPush, ForceMode.Impulse);  //a little launch upwards
        } 
    }
}
