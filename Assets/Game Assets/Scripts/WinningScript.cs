using System.Collections;
using System.Data.Common;
using NUnit.Framework.Constraints;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class WinningScript : MonoBehaviour
{

    [SerializeField] movement movement; 
    [SerializeField] cameraMovements cameraScript;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void OnTriggerEnter(Collider hit)
    {
        Debug.Log("You Win!");
        if (hit.gameObject.tag == "Player")
        {
            movement.gotBread(100000);
            StartCoroutine(Win());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator Win()
    {
        cameraScript.endGame = true; 
        MeshRenderer mesh = this.GetComponent<MeshRenderer>(); 
        mesh.enabled = false; 
        // Destroy(this.gameObject);
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene(0);


    }
}
