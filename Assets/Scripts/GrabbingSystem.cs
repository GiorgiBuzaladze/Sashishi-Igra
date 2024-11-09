using UnityEngine;
using UnityEngine.UI;

public class GrabSystem : MonoBehaviour
{
    public Transform holdPoint;            // Point where grabbed objects are held
    public float grabRange = 3f;           // Max distance to grab objects
    public float holdSmoothness = 10f;     // Smoothness for moving objects to hold point
    public LayerMask grabbableLayer;       // Layer for grabbable objects
    public Image grabIndicator;            // UI image for grab indicator

    private Rigidbody grabbedObject;
    private bool isGrabbing;

    void Start()
    {
        // Ensure the grab indicator is hidden at the start
        grabIndicator.enabled = false;
    }

    void Update()
    {
        // Check if there’s a grabbable object in range
        CheckForGrabbableObject();

        if (Input.GetMouseButtonDown(0))
        {
            AttemptGrab();
        }

        if (Input.GetMouseButtonUp(0) && isGrabbing)
        {
            ReleaseObject();
        }

        if (isGrabbing && grabbedObject != null)
        {
            MoveObject();
        }
    }

    private void CheckForGrabbableObject()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, grabRange, grabbableLayer))
        {
            // Show indicator if object is grabbable
            grabIndicator.enabled = true;
        }
        else
        {
            // Hide indicator if no grabbable object is in range
            grabIndicator.enabled = false;
        }
    }

    private void AttemptGrab()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, grabRange, grabbableLayer))
        {
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                grabbedObject = rb;
                grabbedObject.useGravity = false;
                grabbedObject.drag = 10;
                isGrabbing = true;
                grabIndicator.enabled = false; // Hide indicator when grabbing
            }
        }
    }

    private void ReleaseObject()
    {
        if (grabbedObject != null)
        {
            grabbedObject.useGravity = true;
            grabbedObject.drag = 1;
            grabbedObject = null;
        }
        isGrabbing = false;
    }

    private void MoveObject()
    {
        if (grabbedObject != null)
        {
            Vector3 targetPosition = holdPoint.position;
            Vector3 smoothPosition = Vector3.Lerp(grabbedObject.position, targetPosition, Time.deltaTime * holdSmoothness);
            grabbedObject.MovePosition(smoothPosition);
        }
    }
}
