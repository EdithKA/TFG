using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @brief Handles mouse look functionality for player and camera rotation.
 */
public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 1.5f; /// Sensitivity multiplier for mouse movement.
    public Transform cameraTransform; /// Reference to the camera's transform.
    private float xRotation = 0f; /// Current vertical rotation value for the camera.
    public PlayerController player; /// Reference to the PlayerController to check inventory state.

    /**
     * @brief Initializes references and locks the cursor at the start.
     */
    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
        Cursor.lockState = CursorLockMode.Locked; /// Lock the cursor to the center of the screen.
        Cursor.visible = false; /// Hide the cursor.
    }

    /**
     * @brief Handles mouse input and applies rotation if inventory is closed.
     */
    private void Update()
    {
        if (!player.isInventoryOpen)
        {
            RotatePlayer();
            RotateCamera();
        }
    }

    /**
     * @brief Rotates the player horizontally based on mouse X movement.
     */
    void RotatePlayer()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        transform.Rotate(Vector3.up * mouseX);
    }

    /**
     * @brief Rotates the camera vertically based on mouse Y movement, with clamping.
     */
    void RotateCamera()
    {
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -20f, 40f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
