using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @brief This class configures the player's view and the camera movement with the mouse.
 */
public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 1.5f; 
    public Transform cameraTransform;
    private float xRotation = 0f;
    public PlayerController player;


    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
        Cursor.lockState = CursorLockMode.Locked; 
        Cursor.visible = false; 
    }



    private void Update()
    {
        if (!player.isInventoryOpen) /// If the inventory is open, the camera cannot move.
        {
            RotatePlayer();
            RotateCamera();
        }
        
    }

    /**
     * @brief The player broken on the X axis towards the direction of the mouse.
     */
    void RotatePlayer()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        transform.Rotate(Vector3.up * mouseX); 
    }

    /**
     * @brief The camera can move up and down a certain angle.
     */
    void RotateCamera()
    {
        
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY; 
        xRotation = Mathf.Clamp(xRotation, -20f, 40f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f); 
    }
}
