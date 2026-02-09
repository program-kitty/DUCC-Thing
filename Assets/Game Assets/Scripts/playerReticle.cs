/* A script to control the on-screen reticle with a controller or mouse.
 * There are two control modes: Local and Global. Local is default, but can switch to Global by pressing R1 / RB.
 * Local  aim will move the reticle in the direction the stick is pushed. The further it is pushed, the faster it moves.
 * Global aim will sync the current direction of the movement stick with the reticle's position. When the stick is let go of, the curson will return to the center.
 *
 *
 *
 */
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;
using System.Collections;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(RectTransform))]
public class playerReticle : MonoBehaviour {
    [Header("Objects")]
    Camera cam;
    [SerializeField] GameObject gunMuzzle;
    [SerializeField] GameObject gunOrigin;
    [SerializeField] GameObject playerObject;
    
    [Header("Components")]
    Image reticleSprite;
    RectTransform transformData;
    
    [Header("Game Settings")]
    [SerializeField] bool isLocalAim = true;  // Right-stick aim type
    public int aimSensitivity = 1;

    [Header("Input Actions")]
    InputAction aimAction;
    InputAction attackAction;
    InputAction toggleAimAction;

    [Header("Aiming System")]
    [SerializeField] Transform targetPoint;  // What to shoot at
    [SerializeField] float targetDistance;
    [SerializeField] float SMOOTH_TIME = 0.2f;  // How snappy is target acquisition?
    [SerializeField] Vector2 AIM_LIMIT = new Vector2(50f, 20f);  // Where on the screen can the player aim?
    [SerializeField] float aimSpeed = 10f;

    [Header("Projectile")]
    [SerializeField] GameObject projectilePrefab;  // Object to shoot when fired
    [SerializeField] Transform firePoint;  // Where to spawn the projectile

    [Header("???")]
    Vector3 velocity;
    Vector2 aimOffset;


    #region Input Actions

    public event Action Aim;
    public event Action Attack;
    public event Action ToggleAimMode;

    void OnAttack(InputAction.CallbackContext context) => Attack?.Invoke();
    void OnToggleAimMode(InputAction.CallbackContext context) => ToggleAimMode?.Invoke();

    void OnAttack() {
        var direction = targetPoint.position - firePoint.position;
        var rotation = Quaternion.LookRotation(direction);
        var projectile = Instantiate(projectilePrefab, firePoint.position, rotation);
        Destroy(projectile, 5f);  // Destroy after 5 seconds
    }

    void OnToggleAimMode() {
        isLocalAim = !isLocalAim;
    }

    #endregion
    #region Functions

    void Awake() {
        // Find the references to the InputSystem actions
        aimAction = InputSystem.actions.FindAction("Aim");
        attackAction = InputSystem.actions.FindAction("Attack");
        toggleAimAction = InputSystem.actions.FindAction("Toggle Aim Mode");

        // aimAction.performed += OnAim;
        attackAction.performed += OnAttack;
        toggleAimAction.performed += OnToggleAimMode;
    }


    void OnEnable() {
        // aimAction.performed += OnAim;
        attackAction.performed += OnAttack;
        toggleAimAction.performed += OnToggleAimMode;
    }


    void OnDisable() {
        // aimAction.performed -= OnAim;
        attackAction.performed -= OnAttack;
        toggleAimAction.performed -= OnToggleAimMode;
    }


    void Start() {
        // Hide mouse
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        cam = Camera.main;
        if (cam == null) { Debug.LogError("No main camera was found!"); }  // Make sure camera exists

        reticleSprite = GetComponent<Image>();
        transformData = GetComponent<RectTransform>();
    }


    void Update() {
        // Read and Process Inputs
        Vector2 aimCoordinate = aimAction.ReadValue<Vector2>();  // Read stick input
        Debug.Log($"Aim coordinates: {aimCoordinate}");
        
        Vector2 cursorLocation = new Vector2(cam.pixelWidth * aimCoordinate.x, cam.pixelHeight * aimCoordinate.y);  // Set Cursor pixel display coordinates


        // if (toggleAimAction.ReadValue<float>() > 0) {isLocalAim = !isLocalAim;}

        // Set position based on Right Stick input
        // if (isLocalAim) {
        //     aimCoordinate.x = aimCoordinate.x * aimSensitivity + transformData.localPosition.x; // Add value to X position
        //     aimCoordinate.y = aimCoordinate.y * aimSensitivity + transformData.localPosition.y; // Add value to Y position
        // } else {
        //     aimCoordinate.x = aimCoordinate.x * Screen.width  / 2; // Add value to X position
        //     aimCoordinate.y = aimCoordinate.y * Screen.height / 2; // Add value to Y position
        // }
        
        // Cast ray from cursor
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(cursorLocation);
        
        if (Physics.Raycast(ray, out hit)) {
            Transform objectHit = hit.transform;  // Get the Transform component of the hit object (this is like... the soul of a Unity object, kinda separate from itself?)
            targetPoint = hit.transform;
            // Determine hit object's faction


            // Change reticle color based on target's faction
            if (objectHit.tag == "enemy") {
                reticleSprite.color = Color.red;
            } else if (objectHit.tag == "ally") {
                reticleSprite.color = Color.green;
            } else {
                reticleSprite.color = Color.white;
            }
        }


        transformData.localPosition = new Vector3(cursorLocation.x, cursorLocation.y, 0); // Directly set value
        // transformData.position = Vector3.MoveTowards(transformData.position, hit.point, aimSpeed * time.deltaTime);
        // StartCoroutine( VisualBeam(transformData.localPosition, hit.point) );
        // Debug.DrawRay(transform.position, Vector3.forward, Color.green);
    }
    #endregion
    #region Subroutines

    // Vector3 RayCastLookingAt() {  // Returns the point the center of the camera is looking at
    //     Vector2 cameraSpot = new Vector2(cam.pixelWidth/2, cam.pixelHeight/2);
    //     return RayCastLookingAt(cameraSpot);
    // }
    
    // Vector3 RayCastLookingAt(Vector2 cameraSpot) {  // Returns the point the selected pixel of the camera is looking at
    //     Vector3 point = new Vector3(cameraSpot.x, cameraSpot.y, 0);
    //     Ray ray = cam.ScreenPointToRay(point);
    //     RaycastHit hit;
    //     if (Physics.Raycast(ray, out hit)) {
    //         return hit.point;
    //     } else { return Vector3.forward; }
    // }

    
    // IEnumerator VisualBeam(Vector3 startPoint, Vector3 endPoint) {  // Display a line between two points. Useful for debugging Raycasts.
    //     LineRenderer lineRenderer = gunMuzzle.AddComponent<LineRenderer>(); // create raygun beam object
        
    //     // Apply settings
    //     lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
    //     lineRenderer.startColor = Color.red;
    //     lineRenderer.endColor = Color.yellow;
    //     lineRenderer.startWidth = 0.2f;
    //     lineRenderer.endWidth = 0.1f;
    //     lineRenderer.positionCount = 2; // Set line to be only two points

        
    //     lineRenderer.SetPosition(0, startPoint);
    //     lineRenderer.SetPosition(1, endPoint);

    //     yield return new WaitForSeconds(0.05f);
        
    //     Destroy(lineRenderer);
    // }
    #endregion
}