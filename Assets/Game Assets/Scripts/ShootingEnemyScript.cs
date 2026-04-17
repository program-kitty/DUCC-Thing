using System.Collections;
using UnityEngine;

public class ShootingEnemyScript : MonoBehaviour
{
    [SerializeField] GameObject player; 
    [SerializeField] GameObject projectilePrefab; 

    float counter = 0; 
    float projectileSpeed = 10; 
    float rRange = 8; 
    public float timer = 3f; 
    float distance; 
    float maxDistance = 10f; 
    bool dying = false; 
    bool isShooting = false; 
    Vector3 look; 
    bool startShooting = false; 
    public int health = 1; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 target = player.transform.position; 
        Vector3 currentLocal = this.transform.position; 
        distance = Vector3.Distance(target,currentLocal);
        if (distance <= maxDistance)
        {
            startShooting = true;
        }

        if (startShooting)
        {
            look = new Vector3(player.transform.position.x, this.transform.position.y, player.transform.position.z);
            transform.LookAt(look); //looks at player without moving creature up and down

            counter += Time.deltaTime; 
            if (counter >= timer) //timer
            {
                counter = 0; 
                Shoot(); //shoot function - instantiating projectile
            }
        }

    }
    void Shoot()
    {
        isShooting = true; 
        //ANIMATION FOR SHOOTING HERE? (do you need a timer for it)
        //isShooting bool
        //try having randomized angles on shooting     
        Vector3 randomRotation = new Vector3 (Random.Range(-1,1), Random.Range(-rRange,rRange), 0);
        transform.Rotate(randomRotation); //x is vertical, y is horizontal, z is sideways rotation 

        //instantiating bullets - taken mostly from playerReticle
        var projectile = Instantiate(projectilePrefab, transform.position, transform.rotation); 
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
        bulletScript bulletScript = projectileRb.GetComponent<bulletScript>();
        bulletScript.shootingName = "enemy"; 
        bulletScript.whoShot = this.gameObject; 
        projectileRb.linearVelocity = projectileRb.transform.forward * projectileSpeed; 
        Destroy(projectile,5f); //Destroy after 5 seconds
        isShooting = false; 
    }


    public void gotShot()
    {
        health -= 1; 
        if (health <= 0)
        {
            //probably use an ienumerator
            StartCoroutine(cappyDeath());
        }
    }

    IEnumerator cappyDeath()
    {

//ANIMATION FOR DYING HERE ***
//dying bool
        dying = true;
        yield return new WaitForSeconds(1.5f); 
        Destroy(this.gameObject);
        dying = false; 

    }
}
