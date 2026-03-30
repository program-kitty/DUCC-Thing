using UnityEngine;

public class BazookaScript : MonoBehaviour
{

    [SerializeField] playerReticle Reticle;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Reticle.targetPoint);
        transform.Rotate(-90, 0, -90); //to hopefully account for initial rotation
    }
}
