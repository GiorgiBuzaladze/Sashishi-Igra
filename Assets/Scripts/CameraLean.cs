using UnityEngine;

public class CameraLean : MonoBehaviour
{
    [Header("Lean Settings")]
    public float leanAngle = 15f;
    public float leanSpeed = 5f;
    public float leanDistance = 0.3f;

    private float targetLean = 0f;
    private float currentLean = 0f;
    private Vector3 originalPosition;
    private Vector3 targetPosition;

    private void Start()
    {
        // Capture the initial local position of the camera
        originalPosition = transform.localPosition;
    }

    private void Update()
    {
        HandleLeaning();
    }

    private void HandleLeaning()
    {
        // Set target lean angle and position based on input
        if (Input.GetKey(KeyCode.Q))
        {
            targetLean = leanAngle;
            targetPosition = originalPosition + new Vector3(-leanDistance, 0, 0);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            targetLean = -leanAngle;
            targetPosition = originalPosition + new Vector3(leanDistance, 0, 0);
        }
        else
        {
            targetLean = 0f;
            targetPosition = originalPosition;
        }

        // Smoothly interpolate the lean angle and position
        currentLean = Mathf.Lerp(currentLean, targetLean, Time.deltaTime * leanSpeed);
        transform.localRotation = Quaternion.Euler(0f, 0f, currentLean);

        // Interpolate position for a smoother effect
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * leanSpeed);
    }
}
