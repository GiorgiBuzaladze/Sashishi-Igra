using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    public Light flashlightLight;
    public AudioSource audioSource;
    public AudioClip turnOnSound;
    public AudioClip turnOffSound;

    private bool isOn = false;

    void Start()
    {
        // Ensure flashlight is off at start without modifying rotation
        isOn = false;
        flashlightLight.enabled = false;
        audioSource.playOnAwake = false; // Ensure audio doesn't auto-play
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleFlashlight();
        }
    }

    private void ToggleFlashlight()
    {
        isOn = !isOn;
        flashlightLight.enabled = isOn;

        // Play sound based on flashlight state
        audioSource.PlayOneShot(isOn ? turnOnSound : turnOffSound);
    }
}
