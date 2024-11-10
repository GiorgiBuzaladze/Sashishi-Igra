using UnityEngine;
using UnityEngine.UI;

public class BatteryPickup : MonoBehaviour
{
    public float pickupRange = 3f; // Maximum range to pick up the battery
    public Text pickupIndicatorText; // Text for "Pick Up Battery" prompt
    public Image pickupIndicatorImage; // Image for the battery pickup prompt
    private Transform player;
    private FlashlightController flashlightController;
    private bool isPickedUp = false; // Flag to track if the battery has been picked up

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        flashlightController = player.GetComponent<FlashlightController>();
        ToggleText(pickupIndicatorText, false); // Start with text disabled
        ToggleImage(pickupIndicatorImage, false); // Start with image disabled
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isPickedUp && other.CompareTag("Player"))
        {
            // Show prompt when player enters trigger
            ToggleText(pickupIndicatorText, true);
            ToggleImage(pickupIndicatorImage, true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Hide prompt when player exits trigger
            ToggleText(pickupIndicatorText, false);
            ToggleImage(pickupIndicatorImage, false);
        }
    }

    void Update()
    {
        if (!isPickedUp && Vector3.Distance(player.position, transform.position) <= pickupRange)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                PickUpBattery();
            }
        }
    }

    private void PickUpBattery()
    {
        flashlightController.AddBattery();
        isPickedUp = true; // Set the flag to true when the battery is picked up
        ToggleText(pickupIndicatorText, false); // Optionally hide the text immediately
        ToggleImage(pickupIndicatorImage, false); // Hide the image immediately
        Destroy(gameObject); // Remove battery from scene after pickup
    }

    private void ToggleText(Text textObject, bool state)
    {
        if (textObject != null)
        {
            textObject.enabled = state;
        }
    }

    private void ToggleImage(Image imageObject, bool state)
    {
        if (imageObject != null)
        {
            imageObject.enabled = state;
        }
    }
}
