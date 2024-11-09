using UnityEngine;
using UnityEngine.UI;

public class BatteryPickup : MonoBehaviour
{
    public float pickupRange = 3f; // Maximum range to pick up the battery
    public Text pickupIndicatorText; // Text for "Pick Up Battery" prompt
    public Image pickupIndicatorImage; // Image for the battery pickup prompt

    private Transform player;
    private FlashlightController flashlightController;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        flashlightController = player.GetComponent<FlashlightController>();
        ToggleText(pickupIndicatorText, false); // Start with text disabled
        ToggleImage(pickupIndicatorImage, false); // Start with image disabled
    }

    void Update()
    {
        DetectBatteryPickup();
    }

    private void DetectBatteryPickup()
    {
        Ray ray = new Ray(player.position, player.forward);
        RaycastHit hit;

        // Check if the player is looking directly at the battery within the pickup range
        if (Physics.Raycast(ray, out hit, pickupRange))
        {
            if (hit.transform == transform)
            {
                // Player is looking directly at the battery and within range
                ToggleText(pickupIndicatorText, true);
                ToggleImage(pickupIndicatorImage, true); // Show the image

                // Check for pickup input
                if (Input.GetKeyDown(KeyCode.E))
                {
                    PickUpBattery();
                }
                return; // Exit early if the player is looking at the battery
            }
        }

        // Disable the text and image if the player is not looking at the battery
        ToggleText(pickupIndicatorText, false);
        ToggleImage(pickupIndicatorImage, false);
    }

    private void PickUpBattery()
    {
        flashlightController.AddBattery();
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
