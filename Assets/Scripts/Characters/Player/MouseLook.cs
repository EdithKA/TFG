using UnityEngine;

/// <summary>
/// Handles first-person mouse look for both the soundPlayer body and the camera.
/// Disables look controls when the inventory is open.
/// </summary>
public class MouseLook : MonoBehaviour
{
    [Header("Mouse Look Settings")]
    /// <summary>
    /// Sensitivity multiplier for mouse movement.
    /// </summary>
    public float mouseSensitivity = 1.5f;

    /// <summary>
    /// Reference to the camera's transform.
    /// </summary>
    public Transform cameraTransform;

    /// <summary>
    /// Tracks the camera's up/down rotation.
    /// </summary>
    private float verticalRotation = 0f;

    /// <summary>
    /// Reference to PlayerController (to check inventory state).
    /// </summary>
    public PlayerController playerController;

    /// <summary>
    /// Initializes references and locks the cursor at the start of the game.
    /// </summary>
    private void Start()
    {
        // Find the soundPlayer controller in the scene if not set in the Inspector
        if (playerController == null)
            playerController = FindObjectOfType<PlayerController>();

        // Lock the cursor to the center of the screen and hide it
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /// <summary>
    /// Handles mouse input and applies rotation only if inventory is closed.
    /// </summary>
    private void Update()
    {
        // Only allow mouse look when the inventory is closed
        if (!playerController.isInventoryOpen)
        {
            HandleHorizontalRotation();
            HandleVerticalRotation();
        }
    }

    /// <summary>
    /// Rotates the soundPlayer horizontally (left/right) based on mouse X movement.
    /// </summary>
    private void HandleHorizontalRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        transform.Rotate(Vector3.up * mouseX);
    }

    /// <summary>
    /// Rotates the camera vertically (up/down) based on mouse Y movement, with clamping.
    /// </summary>
    private void HandleVerticalRotation()
    {
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -20f, 40f); // Prevents looking too far up/down
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }
}
