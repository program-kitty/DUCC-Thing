// using System.Collections;
// using UnityEngine;

// public class WeaponSystem : MonoBehaviour {
//     [Header("Objects")]
//     private Camera cam;
//     [SerializeField] GameObject gunMuzzle;
//     [SerializeField] GameObject gunOrigin;
//     [SerializeField] GameObject playerObject;
    
//     [Header("Aiming System")]
//     [SerializeField] Transform targetPoint;  // What to shoot at
//     [SerializeField] float targetDistance;
//     [SerializeField] float SMOOTH_TIME = 0.2f;  // How snappy is target acquisition?
//     [SerializeField] Vector2 AIM_LIMIT = new Vector2(50f, 20f);  // Where on the screen can the player aim?
//     [SerializeField] float aimSpeed = 10f;

//     [Header("Projectile")]
//     [SerializeField] GameObject projectilePrefab;  // Object to shoot when fired
//     [SerializeField] Transform firePoint;  // Where to spawn the projectile

//     [Header("Projectile")]
//     Vector3 velocity;
//     Vector2 aimOffset;

//     // void Awake() {
//     //     // Find the references to the InputSystem actions
//     //     aimAction = InputSystem.actions.FindAction("Aim");
//     //     attackAction = InputSystem.actions.FindAction("Attack");
//     //     toggleAimAction = InputSystem.actions.FindAction("Toggle Aim Mode");

//     //     // aimAction.performed += OnAim;
//     //     attackAction.performed += OnAttack;
//     //     // toggleAimAction.performed += OnAimTypeToggle;
//     // }

//     // void Start() {
//     //     // Hide mouse
//     //     Cursor.lockState = CursorLockMode.Locked;
//     //     Cursor.visible = false;

//     //     cam = Camera.main;
//     //     if (cam == null) { Debug.LogError("No main camera was found!"); }  // Make sure camera exists
//     // }

//     void Update() {
//         // Cast ray from cursor
//         Vector2 aimCoordinate = aimAction.ReadValue<Vector2>();

//         // Cast ray from cursor
//         RaycastHit hit;
//         Ray ray = cam.ScreenPointToRay(aimCoordinate);
        
//         if (Physics.Raycast(ray, out hit)) {
//             Transform objectHit = hit.transform;  // Get the Transform component of the hit object (this is like... the soul of a Unity object, kinda separate from itself?)
            
//             // Determine hit object's faction


//             // Change reticle color based on target's faction
//             if (objectHit.tag == "enemy") {
//                 reticleSprite.color = Color.red;
//             } else if (objectHit.tag == "ally") {
//                 reticleSprite.color = Color.green;
//             } else {
//                 reticleSprite.color = Color.white;
//             }
//         }

//         StartCoroutine( VisualBeam(gunMuzzle.transform.position, hit.point) );









//         Vector3 targetPosition = transform.position + transform.forward * targetDistance;
//         Vector3 localPos = transform.InverseTransformPoint(targetPosition);
//     }

//     // void OnAttack() {
//     //     var direction = targetPoint.position - firePoint.position;
//     //     var rotation = Quaternion.LookRotation(direction);
//     //     var projectile = Instantiate(projectilePrefab, firePoint.position, rotation);
//     //     Destroy(projectile, 5f);  // Destroy after 5 seconds
//     // }

//     // void OnDestroy() {
//     //     input.Attack -+ OnFire;
//     // }




//     Vector3 RayCastLookingAt() {  // Returns the point the center of the camera is looking at
//         Vector2 cameraSpot = new Vector2(cam.pixelWidth/2, cam.pixelHeight/2);
//         return RayCastLookingAt(cameraSpot);
//     }
    
//     Vector3 RayCastLookingAt(Vector2 cameraSpot) {  // Returns the point the selected pixel of the camera is looking at
//         Vector3 point = new Vector3(cameraSpot.x, cameraSpot.y, 0);
//         Ray ray = cam.ScreenPointToRay(point);
//         RaycastHit hit;
//         if (Physics.Raycast(ray, out hit)) {
//             return hit.point;
//         } else { return Vector3.forward; }
//     }

    
//     IEnumerator VisualBeam(Vector3 startPoint, Vector3 endPoint) {  // Display a line between two points. Useful for debugging Raycasts.
//         LineRenderer lineRenderer = gunMuzzle.AddComponent<LineRenderer>(); // create raygun beam object
        
//         // Apply settings
//         lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
//         lineRenderer.startColor = Color.red;
//         lineRenderer.endColor = Color.yellow;
//         lineRenderer.startWidth = 0.2f;
//         lineRenderer.endWidth = 0.1f;
//         lineRenderer.positionCount = 2; // Set line to be only two points

        
//         lineRenderer.SetPosition(0, startPoint);
//         lineRenderer.SetPosition(1, endPoint);

//         yield return new WaitForSeconds(0.05f);
        
//         Destroy(lineRenderer);
//     }
// }
