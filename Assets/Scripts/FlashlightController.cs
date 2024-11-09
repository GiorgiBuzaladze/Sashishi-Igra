using UnityEngine;
using UnityEngine.UI;

public class FlashlightController : MonoBehaviour
{
    public Light flashlightLight;
    public AudioSource audioSource;
    public AudioClip turnOnSound;
    public AudioClip turnOffSound;

    public float maxBatteryLife = 100f;
    private float currentBatteryLife;
    public float batteryDrainRate = 10f;
    public int batteriesCarried = 0;

    public Slider batteryIndicatorSlider; // Slider for battery life
    public Text batteryCountText; // Text to show number of batteries
    public Text reloadBatteryText; // Text for "Reload Battery" prompt
    public Text findBatteriesText; // Text for "Find Batteries" prompt

    private bool isOn = false;
    private float originalIntensity; // Store the original intensity of the flashlight

    void Start()
    {
        isOn = false;
        flashlightLight.enabled = false;
        audioSource.playOnAwake = false;
        currentBatteryLife = maxBatteryLife; // Initialize to max battery life
        originalIntensity = flashlightLight.intensity; // Store the original intensity
        ToggleText(reloadBatteryText, false);
        ToggleText(findBatteriesText, false);
        UpdateBatteryUI();

        // Ensure the flashlight is off if there are no batteries at the start
        if (batteriesCarried <= 0 && currentBatteryLife <= 0)
        {
            flashlightLight.enabled = false;
            isOn = false;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleFlashlight();
        }

        if (isOn)
        {
            DrainBattery();
            AdjustFlashlightIntensity(); // Adjust the flashlight intensity based on battery life
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            TryReloadBattery();
        }
    }

    private void ToggleFlashlight()
    {
        // Only toggle the flashlight if there is battery life available
        if (currentBatteryLife > 0 || batteriesCarried > 0) // Allow toggling if there are batteries to reload
        {
            isOn = !isOn;
            flashlightLight.enabled = isOn;
            audioSource.PlayOneShot(isOn ? turnOnSound : turnOffSound);
        }
        else
        {
            flashlightLight.enabled = false; // Ensure flashlight is off
            isOn = false; // Make sure the toggle state reflects that it's off
        }
    }

    private void DrainBattery()
    {
        currentBatteryLife -= batteryDrainRate * Time.deltaTime;
        currentBatteryLife = Mathf.Max(0, currentBatteryLife);

        if (currentBatteryLife == 0)
        {
            flashlightLight.enabled = false;
            isOn = false;
            UpdateBatteryText();
        }

        UpdateBatteryUI();
    }

    private void AdjustFlashlightIntensity()
    {
        // Check battery percentage
        float batteryPercentage = currentBatteryLife / maxBatteryLife;

        // If battery life is below 20%, start reducing intensity slightly
        if (batteryPercentage < 0.2f)
        {
            // Clamp the intensity to be between 10% of the original intensity and the original intensity
            flashlightLight.intensity = Mathf.Lerp(originalIntensity * 0.1f, originalIntensity, (batteryPercentage - 0.1f) / 0.1f);
        }
        else
        {
            flashlightLight.intensity = originalIntensity; // Reset to original intensity if above 20%
        }
    }

    private void TryReloadBattery()
    {
        if (batteriesCarried > 0 && currentBatteryLife == 0)
        {
            batteriesCarried--; // Use one battery
            currentBatteryLife = maxBatteryLife;
            ToggleText(reloadBatteryText, false);
            UpdateBatteryUI();
        }
        else if (batteriesCarried == 0 && currentBatteryLife == 0)
        {
            UpdateBatteryText();
        }
    }

    private void UpdateBatteryUI()
    {
        batteryIndicatorSlider.value = currentBatteryLife / maxBatteryLife;
        batteryCountText.text = batteriesCarried.ToString();
    }

    private void UpdateBatteryText()
    {
        if (batteriesCarried > 0 && currentBatteryLife == 0)
        {
            ToggleText(reloadBatteryText, true);
            ToggleText(findBatteriesText, false);
        }
        else if (batteriesCarried == 0 && currentBatteryLife == 0)
        {
            ToggleText(findBatteriesText, true);
            ToggleText(reloadBatteryText, false);
        }
        else
        {
            ToggleText(reloadBatteryText, false);
            ToggleText(findBatteriesText, false);
        }
    }

    private void ToggleText(Text textObject, bool state)
    {
        if (textObject != null)
        {
            textObject.enabled = state;
        }
    }

    // Called by BatteryPickup when player picks up a battery
    public void AddBattery()
    {
        batteriesCarried++;
        UpdateBatteryText();
        UpdateBatteryUI();
    }
}