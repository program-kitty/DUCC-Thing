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
    [Header("Game Settings")]
    [SerializeField] bool isLocalAim;  // Right-stick aim type
    public float aimSensitivity = 1f;
    public float globalAimSensitivityDivider = 15f;
    
    [Header("Objects")]
    [SerializeField] Transform gunMuzzle;  // Where to spawn the projectile
    [SerializeField] Transform gunOrigin;  // Gun itself
    [SerializeField] GameObject playerObject;
    [SerializeField] GameObject projectilePrefab;  // Object to shoot when fired
    Camera cam;
    
    Vector3 projectileVelocity;
    
    [Header("Components")]
    Image reticleSprite;
    RectTransform transformData;

    [Header("Input Actions")]
    InputAction aimAction;
    InputAction attackAction;
    InputAction toggleAimAction;

    [Header("Aiming System")]
    [SerializeField] float localSmoothTime = 0.1f;
    [SerializeField] float globalSmoothTime = 0.3f;
    Vector2 aimCoordinate;  // The on-screen coordinate of the reticle. Should be identical to transformData.position
    Vector3 targetPoint;  // World coordinate the reticle is pointing at
    private Vector2 velocity = Vector2.zero;  // Used by SmoothDamp, do not modify!
    private float projectileSpeed = 10.0f; //speed for bullets 


    #region Input Actions

    // public event Action Aim;
    // public event Action Attack;
    // public event Action ToggleAimMode;

    // void OnAim(InputAction.CallbackContext context) => Aim?.Invoke();
    // void OnAttack(InputAction.CallbackContext context) => Attack?.Invoke();
    // void OnToggleAimMode(InputAction.CallbackContext context) => ToggleAimMode?.Invoke();

    // Only called when input is detected, rather than every frame

    // public void OnAim(InputAction.CallbackContext context) {
    //     Vector2 aimInput = context.ReadValue<Vector2>();  // Read stick input

    //     // Set position based on Right Stick input
    //     if (isLocalAim) {
    //         aimCoordinate.x = aimInput.x * aimSensitivity + transformData.position.x; // Add value to X position
    //         aimCoordinate.y = aimInput.y * aimSensitivity + transformData.position.y; // Add value to Y position
    //     } else {
    //         aimCoordinate.x = aimInput.x * Screen.width  / 2; // Add value to X position
    //         aimCoordinate.y = aimInput.y * Screen.height / 2; // Add value to Y position
    //     }
    //     transformData.position = new Vector3(aimCoordinate.x, aimCoordinate.y, 0); // Directly set value
    //     // transformData.position = Vector3.MoveTowards(transformData.position, hit.point, aimSpeed * time.deltaTime);

    //     Debug.Log($"Reticle coordinates: {aimCoordinate}");
    // }
    
    void OnAttack(InputAction.CallbackContext context) {
        var direction = targetPoint - gunMuzzle.position;
        var rotation = Quaternion.LookRotation(direction);
        var projectile = Instantiate(projectilePrefab, gunMuzzle.position, rotation);
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
        projectileRb.linearVelocity = projectileRb.transform.forward * projectileSpeed; //makes sure all bullets are going same speed
        Destroy(projectile, 5f);  // Destroy after 5 seconds
    }

    void OnToggleAimMode(InputAction.CallbackContext context) {
        isLocalAim = !isLocalAim;
    }

    #endregion
    #region Functions

    void Awake() {
        targetPoint = Vector3.zero;  // Reset Target Point
        
        // Find the references to the InputSystem actions
        aimAction = InputSystem.actions.FindAction("Aim");
        attackAction = InputSystem.actions.FindAction("Attack");
        toggleAimAction = InputSystem.actions.FindAction("Toggle Aim Mode");
    }

    void OnEnable() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // aimAction.performed += OnAim;
        attackAction.performed += OnAttack;
        toggleAimAction.performed += OnToggleAimMode;
    }

    void OnDisable() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // aimAction.performed -= OnAim;
        attackAction.performed -= OnAttack;
        toggleAimAction.performed -= OnToggleAimMode;
    }

    void Start() {
        isLocalAim = true;

        cam = Camera.main;
        if (cam == null) { Debug.LogError("No main camera was found!"); }  // Make sure camera exists

        reticleSprite = GetComponent<Image>();
        transformData = GetComponent<RectTransform>();
    }

    void Update() {
        // Update Reticle Position
        Vector2 aimInput = aimAction.ReadValue<Vector2>();  // Read stick input

        // Set position based on Right Stick input
        Vector2 newAimCoordinate;
        // float smoothTime;
        if (isLocalAim) {
            aimCoordinate.x = aimInput.x * aimSensitivity + transformData.position.x; // Add value to X position
            aimCoordinate.y = aimInput.y * aimSensitivity + transformData.position.y; // Add value to Y position
        } else {
            newAimCoordinate.x = (aimInput.x / globalAimSensitivityDivider) * Screen.width  / 2 + Screen.width  / 2; // Add value to X position
            newAimCoordinate.y = (aimInput.y / globalAimSensitivityDivider) * Screen.height / 2 + Screen.height / 2; // Add value to Y position
            // smoothTime = globalSmoothTime;
            aimCoordinate = Vector2.SmoothDamp(aimCoordinate, newAimCoordinate, ref velocity, globalSmoothTime);
        }
        // aimCoordinate = Vector2.SmoothDamp(aimCoordinate, newAimCoordinate, ref velocity, smoothTime);

        // Clamp values to screen size
        aimCoordinate.x = Mathf.Clamp(aimCoordinate.x, 0, Screen.width);
        aimCoordinate.y = Mathf.Clamp(aimCoordinate.y, 0, Screen.height);

        transformData.position = new Vector3(aimCoordinate.x, aimCoordinate.y, 0); // Directly set value
        // transformData.position = Vector3.MoveTowards(transformData.position, hit.point, aimSpeed * time.deltaTime);
        //Debug.Log($"Reticle coordinates: {aimCoordinate}");


        // Cast ray from reticle
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(aimCoordinate);
        
        if (Physics.Raycast(ray, out hit)) {
            targetPoint = hit.point;  // Update targetPoint
            Transform objectHit = hit.transform;  // Get the Transform component of the hit object

            // Change reticle color based on target's faction
            switch (objectHit.tag) {
                case "Player":
                case "ally":
                    reticleSprite.color = Color.green;
                    break;
                case "enemy":
                case "crate":
                    reticleSprite.color = Color.red;
                    break;
                case "Skybox":
                    reticleSprite.color = Color.gray;
                    break;
                default:
                    reticleSprite.color = Color.white;
                    break;
            }

            // Gizmos
            Debug.DrawRay(ray.origin, ray.direction * 10, reticleSprite.color);
        } else {
            Debug.LogError("Raycast failed: Ensure map geometry is encased in a box so raycasts can still hit objects.");
        }

//WASD + JUMP using Space



    }


    void OnDrawGizmos() {
        if (reticleSprite != null) { Gizmos.color = reticleSprite.color; }
        Gizmos.DrawWireSphere(targetPoint, 0.1f);  // Draw wire sphere outline.
    }

    #endregion
    #region Public Callable Functions

    public Vector3 GetAimPos() { return targetPoint; }

    // public void SetAimSensitivity(float sensitivity) { aimSensitivity = sensitivity; }

    #endregion
}