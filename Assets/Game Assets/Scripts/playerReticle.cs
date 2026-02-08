/* A script to control the on-screen reticle with a controller or mouse.
 * There are two control modes: Local and Global. Local is default, but can switch to Global by pressing R1 / RB.
 * Local  aim will move the reticle in the direction the stick is pushed. The further it is pushed, the faster it moves.
 * Global aim will sync the current direction of the movement stick with the reticle's position. When the stick is let go of, the curson will return to the center.
 *
 *
 *
 */
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(RectTransform))]
public class playerReticle : MonoBehaviour {
    public bool displayMouse = false;  // Whether to replace the mouse with a reticle
    bool displayMouseDelta = true;  // Value of displayMouse from last frame
    [Header("Game Settings")]
    [SerializeField] bool isLocalAim = true;  // Right-stick aim type
    public int aimSensitivity = 1;
    // bool isControllerConnected;
    
    Camera mainCamera;
    Image reticleSprite;
    RectTransform transformData;


    [Header("Input Actions")]
    InputAction aimAction;
    InputAction attackAction;
    InputAction toggleAimAction;


    void Start() {
        reticleSprite = GetComponent<Image>();
        transformData = GetComponent<RectTransform>();
        mainCamera = Camera.main;
        if (mainCamera == null) { Debug.LogError("No main camera was found!"); }  // Make sure camera exists

        // Find the references to the InputSystem actions
        aimAction = InputSystem.actions.FindAction("Aim");
        attackAction = InputSystem.actions.FindAction("Attack");
        toggleAimAction = InputSystem.actions.FindAction("Toggle Aim Mode");
    }

    void Update() {
        // Show / Hide mouse
        if (displayMouse && !displayMouseDelta) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            displayMouseDelta = true;

        } else if (!displayMouse && displayMouseDelta) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            displayMouseDelta = false;
        }

        if (toggleAimAction.ReadValue<float>() > 0) {isLocalAim = !isLocalAim;}


        // Read and Process Inputs
        Vector2 aimCoordinate = aimAction.ReadValue<Vector2>();

        // Set position based on Right Stick input
        if (isLocalAim) {
            aimCoordinate.x = aimCoordinate.x * aimSensitivity + transformData.localPosition.x; // Add value to X position
            aimCoordinate.y = aimCoordinate.y * aimSensitivity + transformData.localPosition.y; // Add value to Y position
        } else {
            aimCoordinate.x = aimCoordinate.x * Screen.width  / 2; // Add value to X position
            aimCoordinate.y = aimCoordinate.y * Screen.height / 2; // Add value to Y position
        }
        transformData.localPosition = new Vector3(aimCoordinate.x, aimCoordinate.y, 0); // Directly set value

        if (attackAction.IsPressed()) {
            // shoot
        }
        
        
        
        
        
        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(aimCoordinate);
        
        if (Physics.Raycast(ray, out hit)) {
            Transform objectHit = hit.transform;  // Get the Transform component of the hit object (this is like... the soul of a Unity object, kinda separate from itself?)
            
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
    }
}