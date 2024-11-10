using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform mainCamera;

    void Start()
    {
        // Cache the main camera transform
        mainCamera = Camera.main.transform;
    }

    void Update()
    {
        // Get the direction to the camera, but only in the horizontal plane
        Vector3 direction = transform.position - mainCamera.position;
        direction.y = 0; // Ignore the vertical component

        // Set rotation to face the camera on the Y-axis only
        transform.rotation = Quaternion.LookRotation(direction);
    }
}
