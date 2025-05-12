using UnityEngine;

/**
 * @brief Handles first-person mouse look for both the player body and the camera.
 *        Disables look controls when the inventory is open.
 */
public class MouseLook : MonoBehaviour
{
    [Header("Mouse Look Settings")]
    public float mouseSensitivity = 1.5f;     /// Sensitivity multiplier for mouse movement.
    public Transform cameraTransform;         /// Reference to the camera's transform.
    private float verticalRotation = 0f;      /// Tracks the camera's up/down rotation.
    public PlayerController playerController; /// Reference to PlayerController (to check inventory state).

    /**
     * @brief Initializes references and locks the cursor at the start of the game.
     */
    private void Start()
    {
        // Find the player controller in the scene if not set in the Inspector
        if (playerController == null)
            playerController = FindObjectOfType<PlayerController>();

        // Lock the cursor to the center of the screen and hide it
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /**
     * @brief Handles mouse input and applies rotation only if inventory is closed.
     */
    private void Update()
    {
        // Only allow mouse look when the inventory is closed
        if (!playerController.isInventoryOpen)
        {
            HandleHorizontalRotation();
            HandleVerticalRotation();
        }
    }

    /**
     * @brief Rotates the player horizontally (left/right) based on mouse X movement.
     */
    private void HandleHorizontalRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        transform.Rotate(Vector3.up * mouseX);
    }

    /**
     * @brief Rotates the camera vertically (up/down) based on mouse Y movement, with clamping.
     */
    private void HandleVerticalRotation()
    {
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -20f, 40f); // Prevents looking too far up/down
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }
}
