using UnityEngine;

/**
 * @brief Handles first-person mouse look for both the player body and the camera.
 * Disables look controls when the inventory is open.
 */
public class MouseLook : MonoBehaviour
{
    /**
     * @brief Sensitivity multiplier for mouse movement.
     */
    public float mouseSensitivity = 1.5f;

    /**
     * @brief Reference to the camera's transform.
     */
    public Transform cameraTransform;

    /**
     * @brief Tracks the camera's up/down rotation.
     */
    float verticalRotation = 0f;

    /**
     * @brief Reference to PlayerController (to check inventory state).
     */
    public PlayerController playerController;
    InventoryManager inventoryManager;

    /**
     * @brief Initializes references and locks the cursor at the start of the game.
     */
    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        inventoryManager = FindObjectOfType<InventoryManager>();
        // Lock the cursor to the center of the screen and hide it
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /**
     * @brief Handles mouse input and applies rotation only if inventory is closed.
     */
    void Update()
    {
        // Only allow mouse look when the inventory is closed
        if (!inventoryManager.isInventoryOpen)
        {
            HandleHorizontalRotation();
            HandleVerticalRotation();
        }
    }

    /**
     * @brief Rotates the player horizontally (left/right) based on mouse X movement.
     */
    void HandleHorizontalRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        transform.Rotate(Vector3.up * mouseX);
    }

    /**
     * @brief Rotates the camera vertically (up/down) based on mouse Y movement, with clamping.
     */
    void HandleVerticalRotation()
    {
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -20f, 40f); // Prevents looking too far up/down
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }
}
