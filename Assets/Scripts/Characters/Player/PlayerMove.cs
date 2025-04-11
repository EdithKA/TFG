using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float playerSpeed = 20f;
    float speed;
    private CharacterController characterController;
    public Animator camAnim;
    public bool isWalking;

    private Vector3 inputVector;
    private Vector3 movementVector;
    private float Gravity = -10f;

    public float stamina;
    public float currentStamina;

    public InventoryManager inventoryManager;
    public bool isInventoryOpen = false;

    [Header("Hands Settings")]
    //Left Hand
    public Animator LHAnim;
    public bool LActive = false;
    public bool isCloser = false;

    //Right Hand
    public Animator RHAnim;
    public bool RActive = false;
 



    void Start()
    {
        inventoryManager = FindObjectOfType<InventoryManager>();
        currentStamina = stamina;
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && inventoryManager.HasItem("Mokia"))
        {
            isInventoryOpen = !isInventoryOpen;
            StartCoroutine(OpenInventory());
            Debug.Log("Estado del inventario: " + isInventoryOpen);
            isCloser = !isCloser;
        }

        if (!isInventoryOpen)
        {
            GetInput();
        }
        else
        {
            speed = 0;
            isWalking = false;
            movementVector = Vector3.zero;
        }

        MovePlayer();
        CheckForHeadBob();

        

        // Actualizamos los parámetros de animación
        setAnimation();

       
    }

    void setAnimation()
    {
        camAnim.SetBool("IsWalking", isWalking);
        LHAnim.SetBool("isCloser", isCloser);
        LHAnim.SetBool("isActive", LActive);
        RHAnim.SetBool("isActive", RActive);



    }

    private void CheckForHeadBob()
    {
        if (characterController.velocity.magnitude > 0.1f)
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }
    }

    void GetInput()
    {
        // 1. Manejar el sprint
        if (Input.GetKey(KeyCode.LeftShift) && currentStamina > 0)
        {
            speed = playerSpeed * 3;
            currentStamina -= 0.1f;
        }
        else // Restablecer velocidad si NO se está presionando LeftShift
        {
            speed = playerSpeed;
            if (currentStamina < stamina)
            {
                currentStamina += 0.1f;
            }
        }

        // 2. Manejar el teléfono
        if (Input.GetMouseButtonDown(1)) // Acercar el teléfono
        {
            isCloser = true;
        }
        else if (Input.GetMouseButtonUp(1)) // Alejar el teléfono
        {
            isCloser = false;
        }

        // Movimiento (siempre se calcula)
        inputVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        inputVector.Normalize();
        inputVector = transform.TransformDirection(inputVector);
        movementVector = (inputVector * speed) + (Vector3.up * Gravity);
    }

    

    IEnumerator OpenInventory()
    {
        

        // Esperamos un pequeño tiempo para que la transición comience
        yield return new WaitForSeconds(0.1f);

        // Luego esperamos a que la animación se esté reproduciendo y termine
        string animName = isInventoryOpen ? "OpenInventory" : "CloseInventory";

        

        // Activar/desactivar el inventario
        inventoryManager.ToggleInventory();

        // Sincronizar estado
        isInventoryOpen = inventoryManager.IsInventoryOpen;

        
    }

    void MovePlayer()
    {
        characterController.Move(movementVector * Time.deltaTime);
    }
}
