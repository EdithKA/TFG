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

    public Animator handAnim;

    public InventoryManager inventoryManager;
    public bool isInventoryOpen = false;


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
            Debug.Log(isInventoryOpen);
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
        camAnim.SetBool("IsWalking", isWalking);
    }

    private void CheckForHeadBob()
    {
        if(characterController.velocity.magnitude > 0.1f)
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
            handAnim.SetBool("closer", true);
        }
        else if (Input.GetMouseButtonUp(1)) // Alejar el teléfono
        {
            handAnim.SetBool("closer", false);
        }

        

        // Movimiento (siempre se calcula)
        inputVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        inputVector.Normalize();
        inputVector = transform.TransformDirection(inputVector);
        movementVector = (inputVector * speed) + (Vector3.up * Gravity);
    }


    IEnumerator OpenInventory()
    {

        handAnim.SetBool("OpenInventory", isInventoryOpen);
   
        // Esperar a que termine la animación (no usar WaitForSeconds)
        yield return new WaitUntil(() =>
            handAnim.GetCurrentAnimatorStateInfo(0).IsName("OpenInventory") &&
            handAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f
        );

        inventoryManager.ToggleInventory();
        isInventoryOpen = inventoryManager.IsInventoryOpen; // Sincronizar estado
    }

    void MovePlayer()
    {
        characterController.Move(movementVector * Time.deltaTime);
    }
}
