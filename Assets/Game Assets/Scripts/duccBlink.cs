using UnityEngine;
using System.Collections;

public class duccBlink : MonoBehaviour {
    [SerializeField] Material leftEye;  // Material for Left Eye
    [SerializeField] Material rightEye; // Material for Right Eye
    
    [Header("Intervals")]
    float blinkTimer = 5f;
    [SerializeField] float minBlinkInterval = 3f;   // Minimum time before DUCC Thing blinks
    [SerializeField] float maxBlinkInterval = 4f;   // Maximum time before DUCC Thing blinks
    [SerializeField] float blinkDuration = 0.1f;    // How long do blinks last?

    [Header("Textures")]
    [SerializeField] Texture EYE;
    [SerializeField] Texture EYE_BLINK;

    void Start() {
        if (!leftEye) {}
        if (!rightEye) {}
    }


    void Update() {
        blinkTimer -= Time.deltaTime;
        if (blinkTimer < 0f) {
            blinkTimer = Random.Range(minBlinkInterval, maxBlinkInterval);  // Random float for when to next blink
            StartCoroutine(Blink());  // Make DUCC Thing blink
        }
    }

    IEnumerator Blink() {
        leftEye.mainTexture = EYE_BLINK;
        rightEye.mainTexture = EYE_BLINK;

        yield return new WaitForSeconds(blinkDuration);

        leftEye.mainTexture = EYE;
        rightEye.mainTexture = EYE;
    }
}
