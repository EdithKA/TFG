using UnityEngine;

/**
 * @brief Mouse look: rotate player and camera with mouse.
 */
public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 1.5f;         ///< Mouse sensitivity
    public Transform cameraTransform;             ///< Camera to rotate (vertical)
    float verticalRotation = 0f;                  ///< Up/down rotation
    public PlayerController playerController;     ///< Player ref
    InventoryManager inventoryManager;            ///< Inventory ref

    /**
     * @brief Get refs, lock cursor.
     */
    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        inventoryManager = FindObjectOfType<InventoryManager>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /**
     * @brief Rotate if inventory closed.
     */
    void Update()
    {
        if (!inventoryManager.isInventoryOpen)
        {
            HandleHorizontalRotation();
            HandleVerticalRotation();
        }
    }

    /**
     * @brief Rotate player left/right.
     */
    void HandleHorizontalRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        transform.Rotate(Vector3.up * mouseX);
    }

    /**
     * @brief Rotate camera up/down (clamped).
     */
    void HandleVerticalRotation()
    {
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -20f, 40f);
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }
}
