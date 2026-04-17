using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class duccAnimator : MonoBehaviour {
    Animator animator;
    Rigidbody playerRigidbody;

    bool isGrounded;

    void Start() {
        animator = GetComponent<Animator>();  // Get the component from the GameObject
        playerRigidbody = GetComponent<Rigidbody>();  // Get Rigidbody of PlayerObject
    }

    void Update() {
        float speed = playerRigidbody.linearVelocity.x + playerRigidbody.linearVelocity.z;
        if (playerRigidbody.linearVelocity.y != 0) {
            isGrounded = false;
        } else {
            isGrounded = true;
        }

        // Apply variables to Animator
        animator.SetFloat("speed", speed);
        animator.SetBool("grounded", isGrounded);
    }
}
