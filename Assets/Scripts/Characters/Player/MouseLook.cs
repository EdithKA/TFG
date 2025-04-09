using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 1.5f; 
    public Transform cameraTransform;
    private float xRotation = 0f;
    public PlayerMove player;

    private void Start()
    {
        player = FindObjectOfType<PlayerMove>();
        Cursor.lockState = CursorLockMode.Locked; 
        Cursor.visible = false; 
    }



    private void Update()
    {
        if (!player.isInventoryOpen)
        {
            RotatePlayer();
            RotateCamera();
        }
        
    }

    void RotatePlayer()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        transform.Rotate(Vector3.up * mouseX); 
    }

    void RotateCamera()
    {
        
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY; 
        xRotation = Mathf.Clamp(xRotation, -20f, 40f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f); 
    }
}
