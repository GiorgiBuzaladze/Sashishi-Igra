using UnityEngine;

public class HingeDoor : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioSource customAudioSource; // Optional: Drag your own AudioSource here
    public AudioClip closeSound;         // Sound for closing

    private AudioSource audioSource;     // Reference to the audio source used for sounds
    private HingeJoint hingeJoint;       // The hinge joint for the door
    private bool hasPlayedCloseSound = false; // Flag to track if close sound has been played

    void Start()
    {
        // If a custom AudioSource is not assigned, get the AudioSource attached to the door
        audioSource = customAudioSource != null ? customAudioSource : GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError("No AudioSource found for the door!");
        }

        hingeJoint = GetComponent<HingeJoint>();
    }

    void Update()
    {
        float currentAngle = hingeJoint.transform.eulerAngles.y; // Assuming the door rotates around the Y-axis

        // Check if the door is fully closed (angle is around 0)
        if (currentAngle <= 2 && !hasPlayedCloseSound)
        {
            if (closeSound != null)
            {
                audioSource.PlayOneShot(closeSound); // Play the close sound once
                hasPlayedCloseSound = true; // Set the flag to prevent repeated sounds
            }
        }
        else if (currentAngle > 2)
        {
            hasPlayedCloseSound = false; // Reset flag if the door is opened again
        }
    }
}
