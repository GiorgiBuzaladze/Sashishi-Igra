using UnityEngine;

public class HingeDoor : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioSource customAudioSource; // Optional: Drag your own AudioSource here
    public AudioClip closeSound;          // Sound for closing
    public AudioClip openSound;           // Sound for opening

    private AudioSource audioSource;      // Reference to the audio source used for sounds
    private HingeJoint hingeJoint;        // The hinge joint for the door
    private bool hasPlayedCloseSound = true; // Start as true to prevent initial play
    private bool hasPlayedOpenSound = false; // Flag to track if open sound has been played

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
                hasPlayedOpenSound = false; // Reset open sound flag when door is closed
            }
        }
        // Check if the door is opened (angle greater than a small threshold)
        else if (currentAngle > 2 && !hasPlayedOpenSound)
        {
            if (openSound != null)
            {
                audioSource.PlayOneShot(openSound); // Play the open sound once
                hasPlayedOpenSound = true; // Set the flag to prevent repeated sounds
                hasPlayedCloseSound = false; // Reset close sound flag when door is opened
            }
        }
    }
}
