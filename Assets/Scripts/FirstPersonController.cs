using UnityEngine;
using UnityEngine.UI;

public class FirstPersonController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float crouchSpeed = 2.5f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;

    [Header("Mouse Settings")]
    public float mouseSensitivity = 100f;

    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    public float staminaDrainRate = 10f;
    public float staminaRegenRate = 5f;
    public Slider staminaSlider;
    public AudioClip exhaustionSound; // Audio for exhaustion
    public AudioSource audioSource; // AudioSource for exhaustion sound

    [Header("Crouch Settings")]
    public float crouchHeight = 1f;
    public float standingHeight = 2f;
    public float crouchTransitionSpeed = 5f;

    private CharacterController characterController;
    private Transform cameraTransform;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isSprinting;
    private bool isCrouching;
    private float stamina;
    private float xRotation = 0f;
    private float targetHeight;
    private Vector3 targetCenter;
    private bool isExhausted = false; // To track exhaustion state

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        cameraTransform = Camera.main.transform;
        Cursor.lockState = CursorLockMode.Locked;

        stamina = maxStamina;
        staminaSlider.maxValue = maxStamina;
        staminaSlider.value = stamina;

        targetHeight = standingHeight;
        targetCenter = characterController.center;

        if (audioSource != null && exhaustionSound != null)
        {
            audioSource.clip = exhaustionSound;
            audioSource.loop = true; // Loop the exhaustion sound
            audioSource.volume = 0f; // Start with no sound
        }
        else
        {
            Debug.LogWarning("AudioSource or exhaustionSound is not assigned.");
        }
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
        HandleSprintAndStamina();
        HandleCrouch();
        UpdateCameraPosition();
        HandleExhaustionSound(); // Handle audio based on stamina
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f); // Only vertical rotation
        transform.Rotate(Vector3.up * mouseX);
    }

    private void HandleMovement()
    {
        isGrounded = characterController.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        float currentSpeed = isCrouching ? crouchSpeed : (isSprinting ? sprintSpeed : walkSpeed);
        characterController.Move(move * currentSpeed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded && !isCrouching)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            stamina -= 10f;
            stamina = Mathf.Clamp(stamina, 0, maxStamina);
        }

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    private void HandleSprintAndStamina()
    {
        bool forwardInput = Input.GetAxis("Vertical") > 0;

        if (Input.GetKey(KeyCode.LeftShift) && forwardInput && stamina > 0 && !isCrouching && isGrounded)
        {
            isSprinting = true;
            stamina -= staminaDrainRate * Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0, maxStamina);

            if (stamina <= 0)
            {
                isSprinting = false;
            }
        }
        else
        {
            isSprinting = false;

            if (stamina < maxStamina && !isSprinting)
            {
                stamina += staminaRegenRate * Time.deltaTime;
                stamina = Mathf.Clamp(stamina, 0, maxStamina);
            }
        }

        staminaSlider.value = stamina;
    }

    private void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (isCrouching)
            {
                if (!IsObstacleAbove())
                {
                    isCrouching = false;
                    targetHeight = standingHeight;
                    targetCenter = new Vector3(0, standingHeight / 2, 0);
                }
            }
            else
            {
                isCrouching = true;
                targetHeight = crouchHeight;
                targetCenter = new Vector3(0, crouchHeight / 2, 0);
            }
        }

        characterController.height = Mathf.Lerp(characterController.height, targetHeight, Time.deltaTime * crouchTransitionSpeed);
        characterController.center = Vector3.Lerp(characterController.center, targetCenter, Time.deltaTime * crouchTransitionSpeed);

        if (Mathf.Abs(characterController.height - targetHeight) < 0.01f)
        {
            characterController.height = targetHeight;
            characterController.center = targetCenter;
        }
    }

    private bool IsObstacleAbove()
    {
        float checkDistance = (standingHeight - crouchHeight) / 2 + 0.5f;
        Vector3 origin = transform.position + Vector3.up * (crouchHeight / 2);
        return Physics.Raycast(origin, Vector3.up, checkDistance);
    }

    private void HandleExhaustionSound()
    {
        if (audioSource != null)
        {
            if (stamina <= 0)
            {
                audioSource.volume = 1f; // Play at full volume when stamina reaches 0
                if (!audioSource.isPlaying)
                {
                    audioSource.Play(); // Start playing the exhaustion sound
                }
            }
            else if (stamina <= maxStamina * 0.3f)
            {
                // Ease the volume in as stamina goes below 30%
                audioSource.volume = Mathf.Lerp(audioSource.volume, 1f, Time.deltaTime);
                if (!audioSource.isPlaying)
                {
                    audioSource.Play(); // Start playing the exhaustion sound if not already playing
                }
            }
            else
            {
                // Fade out the sound when stamina is above 30%
                audioSource.volume = Mathf.Lerp(audioSource.volume, 0f, Time.deltaTime);
            }
        }
    }

    private void UpdateCameraPosition()
    {
        // Only interpolate the Y position for smooth crouch transition
        Vector3 targetCameraPos = new Vector3(cameraTransform.position.x, characterController.transform.position.y + characterController.center.y + characterController.height / 3, cameraTransform.position.z);

        // Smooth the Y-axis position for crouching and leave X and Z axis unchanged
        cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetCameraPos, Time.deltaTime * crouchTransitionSpeed);
    }
}
