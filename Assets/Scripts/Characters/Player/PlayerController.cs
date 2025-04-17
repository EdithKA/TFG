using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @brief This class controls the interactions and movement of the player.
 */
public class PlayerController : MonoBehaviour
{

    public Animator camAnim; /// It is used to walk the camera walk.


    public float playerSpeed = 20f;
    float speed;
    private CharacterController characterController;
    
    public bool isWalking;

    private Vector3 inputVector;
    private Vector3 movementVector;
    private float Gravity = -10f;

    /**
    * @brief Player statistics that influence the game.
    */

    [Header("Player Stats")]
    public float stamina;
    public float currentStamina;


    [Header("Inventory Settings")]
    public InventoryManager inventoryManager;
    public bool isInventoryOpen = false;
    public bool closer = false;

    
    [Header("Hands Settings")]
    public Animator handAnim; 
    public bool LeftHandOn = false; /// Indicate if the left hand is being used.
    public bool RightHandOn = false; /// Indicate if the right hand is being used.
    public GameObject leftHand;
    public GameObject rightHand;




    /**
    * @brief Start of the game.
    */
    void Start()
    {
        inventoryManager = FindObjectOfType<InventoryManager>(); ///We assign the script that controls and manages the player's inventory.
        currentStamina = stamina; ///The player's stamina is restored to the maximum.
        characterController = GetComponent<CharacterController>(); ///We assign the component that controls the character.
    }

    void Update()
    {
        /// Pressing the I key and only if we have the phone, we can open the inventory.
        if (Input.GetKeyDown(KeyCode.I) && inventoryManager.HasItem("Mokia"))
        {
            isInventoryOpen = !isInventoryOpen; /// The inventory is open.
            StartCoroutine(OpenInventory()); /// We reproduce the animation to open the inventory.       
        }

        /// While the inventory is closed, we can move.
        if (!isInventoryOpen)
        {
            GetInput();
        }
        else /// If we open the inventory, we cannot move or interact with the outside.
        {
            speed = 0;
            isWalking = false;
            movementVector = Vector3.zero; 
        }

        MovePlayer();
        CheckForHeadBob();
        setAnimation();
    }

    /**
    * @brief This function assigns the player animation the animation (or animations) that corresponds based on the value of different variables that are modified throughout the code.
    */
    void setAnimation()
    {
        camAnim.SetBool("IsWalking", isWalking); /// Chamber movement.
        handAnim.SetBool("closer", closer); /// Bring the phone closer.
        handAnim.SetBool("OpenInventory", isInventoryOpen);/// Open the inventory.
        handAnim.SetBool("LObject", LeftHandOn);/// Remove or "save" your left hand.
        handAnim.SetBool("RObject", RightHandOn); /// Take out or "save" your right hand.

        leftHand.SetActive(LeftHandOn); /// Active or disabled left hand.
        rightHand.SetActive(RightHandOn); /// Right or disabled right hand.

    }

    /**
    * @brief This function determines whether the character is walking or not to reproduce the animation of the camera that simulates the way of walking the player.
    */
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

    /**
    * @brief Handle the user's input system.
    */
    void GetInput()
    {
        /// Run as long as you stamin.
        if (Input.GetKey(KeyCode.LeftShift) && currentStamina > 0)
        {
            speed = playerSpeed * 3;
            currentStamina -= 0.1f;
        }
        else 
        {
            speed = playerSpeed;
            if (currentStamina < stamina)
            {
                currentStamina += 0.1f;
            }
        }

        /// Bring the phone closer.
        if (Input.GetMouseButtonDown(1)) 
        {
            closer = true;
        }
        else if (Input.GetMouseButtonUp(1)) 
        {
            closer = false;
        }

        /// Player displacement (it is continuously calculated once the function is called).
        inputVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        inputVector.Normalize();
        inputVector = transform.TransformDirection(inputVector);
        movementVector = (inputVector * speed) + (Vector3.up * Gravity);
    }

    /**
    * @brief It is used to soften the animation to open the inventory. The inventory becomes visible or invisible a while after starting animation.
    * */
    IEnumerator OpenInventory()
    {

        //handAnim.SetBool("OpenInventory", isInventoryOpen);
        yield return new WaitUntil(() =>
            handAnim.GetCurrentAnimatorStateInfo(0).IsName("OpenInventory") &&
            handAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f
        );
        inventoryManager.ToggleInventory();
        isInventoryOpen = inventoryManager.IsInventoryOpen; // Sincronizar estado
    }

    /**
    * @brief Move the character.
    */
    void MovePlayer()
    {
        characterController.Move(movementVector * Time.deltaTime);
    }
}
