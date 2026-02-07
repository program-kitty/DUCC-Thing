using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(RectTransform))]
public class playerReticle : MonoBehaviour {
    Camera mainCamera;
    Image reticleSprite;
    RectTransform transformData;
    bool isControllerConnected;

    [Header("Input Actions")]
    InputAction aimAction;
    InputAction attackAction;


    void Start() {
        reticleSprite = GetComponent<Image>();
        transformData = GetComponent<RectTransform>();
        mainCamera = Camera.main;
        if (mainCamera == null) { Debug.LogError("No main camera was found!"); }  // Make sure camera exists

        // Find the references to the "Look" and "Attack" actions
        aimAction = InputSystem.actions.FindAction("Look");
        attackAction = InputSystem.actions.FindAction("Attack");
    }

    void Update() {
        // Read and Process Inputs
        Vector2 aimCoordinate = aimAction.ReadValue<Vector2>();
        transformData.localPosition = new Vector3(aimCoordinate.x, aimCoordinate.y, 0); // Set position based on Right Stick input

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