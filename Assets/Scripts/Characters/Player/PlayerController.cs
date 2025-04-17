using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @brief This class controls the player's movement, stamina, inventory, and animation logic.
 */
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float playerSpeed = 20f;           /// Base movement speed of the player.
    float speed;                              /// Current speed (may be affected by sprinting).
    private CharacterController characterController; /// Reference to the CharacterController component.
    public Animator camAnim;                  /// Animator for the camera (head bobbing).
    public bool isWalking;                    /// Indicates if the player is currently walking.

    private Vector3 inputVector;              /// Stores player input direction.
    private Vector3 movementVector;           /// Final movement vector applied to the character.
    private float Gravity = -10f;             /// Gravity applied to the player.

    [Header("Stamina Settings")]
    public float stamina;                     /// Maximum stamina.
    public float currentStamina;              /// Current stamina.

    [Header("Inventory Settings")]
    public InventoryManager inventoryManager; /// Reference to the inventory manager.
    public bool isInventoryOpen = false;      /// Indicates if the inventory is currently open.

    [Header("Hands Settings")]
    // Left Hand
    public Animator LHAnim;                   /// Animator for the left hand.
    public bool LActive = false;              /// Is the left hand active?
    public bool isCloser = false;             /// Is the phone being held closer?

    // Right Hand
    public Animator RHAnim;                   /// Animator for the right hand.
    public bool RActive = false;              /// Is the right hand active?

    /**
     * @brief Initialization of components and player stats.
     */
    void Start()
    {
        inventoryManager = FindObjectOfType<InventoryManager>(); /// Assign the inventory manager.
        currentStamina = stamina;                                /// Restore stamina to maximum.
        characterController = GetComponent<CharacterController>();/// Assign the CharacterController component.
    }

    /**
     * @brief Main update loop. Handles input, movement, and animation updates.
     */
    void Update()
    {
        // Toggle inventory if "I" is pressed and the player has the "Mokia" item.
        if (Input.GetKeyDown(KeyCode.I) && inventoryManager.HasItem("Mokia"))
        {
            isInventoryOpen = !isInventoryOpen;
            StartCoroutine(OpenInventory());
            Debug.Log("Inventory state: " + isInventoryOpen);
            isCloser = !isCloser; // Toggle phone proximity
        }

        // Only allow movement if inventory is closed.
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

        // Update all animation parameters.
        setAnimation();
    }

    /**
     * @brief Updates all animation parameters for camera and hands.
     */
    void setAnimation()
    {
        camAnim.SetBool("IsWalking", isWalking);      /// Camera walk animation.
        LHAnim.SetBool("isCloser", isCloser);         /// Phone proximity animation.
        LHAnim.SetBool("isActive", LActive);          /// Left hand active/inactive.
        RHAnim.SetBool("isActive", RActive);          /// Right hand active/inactive.
    }

    /**
     * @brief Checks if the player is walking to trigger head bob animation.
     */
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

    /**
     * @brief Handles player input for movement, sprinting, and phone interaction.
     */
    void GetInput()
    {
        // 1. Handle sprinting
        if (Input.GetKey(KeyCode.LeftShift) && currentStamina > 0)
        {
            speed = playerSpeed * 3;
            currentStamina -= 0.1f;
        }
        else // Reset speed if not sprinting
        {
            speed = playerSpeed;
            if (currentStamina < stamina)
            {
                currentStamina += 0.1f;
            }
        }

        // 2. Handle phone proximity
        if (Input.GetMouseButtonDown(1)) // Bring phone closer
        {
            isCloser = true;
        }
        else if (Input.GetMouseButtonUp(1)) // Move phone away
        {
            isCloser = false;
        }

        // 3. Calculate movement vector
        inputVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        inputVector.Normalize();
        inputVector = transform.TransformDirection(inputVector);
        movementVector = (inputVector * speed) + (Vector3.up * Gravity);
    }

    /**
     * @brief Handles the inventory opening animation and state.
     */
    IEnumerator OpenInventory()
    {
        // Wait a short time for the animation transition to start
        yield return new WaitForSeconds(0.1f);

        // Determine the animation name based on inventory state
        string animName = isInventoryOpen ? "OpenInventory" : "CloseInventory";

        // Toggle inventory visibility
        inventoryManager.ToggleInventory();

        // Synchronize state with inventory manager
        isInventoryOpen = inventoryManager.IsInventoryOpen;
    }

    /**
     * @brief Moves the player character using the calculated movement vector.
     */
    void MovePlayer()
    {
        characterController.Move(movementVector * Time.deltaTime);
    }
}
