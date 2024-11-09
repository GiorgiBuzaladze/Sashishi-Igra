using UnityEngine;
using UnityEngine.UI;

public class GrabSystem : MonoBehaviour
{
    public Transform holdPoint;            // Point where grabbed objects are held
    public float grabRange = 3f;           // Max distance to grab objects
    public float holdSmoothness = 10f;     // Smoothness for moving objects to hold point
    public float throwForce = 10f;         // Force applied to objects when thrown
    public LayerMask grabbableLayer;       // Layer for grabbable objects
    public Image grabIndicator;            // UI image for grab indicator

    private Rigidbody grabbedObject;
    private bool isGrabbing;
    private float initialDrag;
    private Collider playerCollider;       // Collider of the player to ignore collisions with grabbed objects
    private float maxGrabDistance = 3f;    // Maximum distance the player can move away while still holding an object

    void Start()
    {
        // Ensure the grab indicator is hidden at the start
        grabIndicator.enabled = false;
        playerCollider = GetComponent<Collider>();
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

            // Release object if it's out of range
            if (Vector3.Distance(holdPoint.position, grabbedObject.position) > maxGrabDistance)
            {
                ReleaseObject();
            }
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
                initialDrag = grabbedObject.drag;
                grabbedObject.drag = 10;
                grabbedObject.collisionDetectionMode = CollisionDetectionMode.Continuous;

                // Check if the object is a door based on tag
                if (!hit.collider.CompareTag("Door"))
                {
                    // Ignore collisions between the player and the grabbed object (only if not a door)
                    Physics.IgnoreCollision(grabbedObject.GetComponent<Collider>(), playerCollider, true);
                }

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
            grabbedObject.drag = initialDrag;
            grabbedObject.collisionDetectionMode = CollisionDetectionMode.Discrete;

            // Stop ignoring collisions between the player and the object (only if not a door)
            if (!grabbedObject.CompareTag("Door"))
            {
                Physics.IgnoreCollision(grabbedObject.GetComponent<Collider>(), playerCollider, false);
            }

            // Apply throw force based on mouse movement
            Vector3 throwVelocity = Camera.main.transform.forward * throwForce;
            grabbedObject.velocity = throwVelocity;

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
