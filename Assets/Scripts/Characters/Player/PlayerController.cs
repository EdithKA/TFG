using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls core player functionality including movement, interactions, and inventory management.
/// Designed for a 3D survival horror experience with physics-based movement.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Movement Configuration")]
    public float baseSpeed = 5f;              /// Base movement speed in m/s
    public float sprintMultiplier = 1.5f;     /// Sprint speed multiplier
    public float gravity = -9.81f;            /// Custom gravity value for movement system
    private float currentSpeed;               /// Current calculated movement speed
    private CharacterController controller;   /// Reference to CharacterController component

    [Header("Stamina System")]
    public float maxStamina = 100f;           /// Maximum stamina capacity
    public float staminaDrainRate = 0.1f;     /// Stamina consumption per frame while sprinting
    public float staminaRegenRate = 0.05f;    /// Stamina recovery rate when not sprinting
    private float currentStamina;             /// Current stamina value

    [Header("Inventory System")]
    public InventoryManager inventory;        /// Reference to inventory management system
    public bool isInventoryOpen;              /// Inventory panel visibility state
    public UITextController uiFeedback;       /// UI text display controller

    [Header("Hand Animations")]
    public Animator leftHandAnimator;         /// Left hand animation controller
    public Animator rightHandAnimator;        /// Right hand animation controller
    public Transform leftHandSlot;            /// Left hand item attachment point
    public Transform rightHandSlot;           /// Right hand item attachment point
    private bool isAiming;                    /// Right mouse button hold state

    [Header("Interaction System")]
    public float interactionRadius = 3f;      /// Detection radius for interactable objects
    public LayerMask interactableMask;        /// Layer filter for interactable objects
    private List<IInteractable> nearbyInteractables = new List<IInteractable>(); /// Nearby interactable objects

    private Vector3 movementInput;            /// Raw input vector from player
    private Vector3 calculatedMovement;       /// Final movement vector with physics

    /// <summary>
    /// Initializes components and sets up interaction collider.
    /// </summary>
    private void Start()
    {
        controller = GetComponent<CharacterController>();
        inventory = FindObjectOfType<InventoryManager>();
        uiFeedback = FindObjectOfType<UITextController>();
        currentStamina = maxStamina;

        SphereCollider interactionCollider = gameObject.AddComponent<SphereCollider>();
        interactionCollider.isTrigger = true;
        interactionCollider.radius = interactionRadius;
    }

    /// <summary>
    /// Main game loop handling input processing and state updates.
    /// </summary>
    private void Update()
    {
        HandleInventoryToggle();
        ProcessPlayerInput();
        UpdateMovement();
        UpdateAnimations();
        HandleInteraction();
    }

    /// <summary>
    /// Physics-based movement update using CharacterController.
    /// </summary>
    private void FixedUpdate()
    {
        ApplyMovement();
    }

    /// <summary>
    /// Processes all player input including movement, sprint, and aiming.
    /// </summary>
    private void ProcessPlayerInput()
    {
        if (!isInventoryOpen)
        {
            HandleMovementInput();
            HandleSprintInput();
            HandleAimInput();
        }
    }

    /// <summary>
    /// Reads movement input from player.
    /// </summary>
    private void HandleMovementInput()
    {
        movementInput = new Vector3(
            Input.GetAxis("Horizontal"),
            0,
            Input.GetAxis("Vertical")
        ).normalized;
    }

    /// <summary>
    /// Handles sprint logic and stamina consumption.
    /// </summary>
    private void HandleSprintInput()
    {
        bool isSprinting = Input.GetKey(KeyCode.LeftShift) && currentStamina > 0;
        currentSpeed = isSprinting ? baseSpeed * sprintMultiplier : baseSpeed;

        if (isSprinting)
            currentStamina = Mathf.Max(0, currentStamina - staminaDrainRate);
        else
            currentStamina = Mathf.Min(maxStamina, currentStamina + staminaRegenRate);
    }

    /// <summary>
    /// Handles aiming state (right mouse button).
    /// </summary>
    private void HandleAimInput()
    {
        isAiming = Input.GetMouseButton(1);
    }

    /// <summary>
    /// Calculates final movement vector incorporating speed and gravity.
    /// </summary>
    private void UpdateMovement()
    {
        Vector3 transformedInput = transform.TransformDirection(movementInput);
        calculatedMovement = (transformedInput * currentSpeed) + (Vector3.up * gravity);
    }

    /// <summary>
    /// Applies movement to the CharacterController.
    /// </summary>
    private void ApplyMovement()
    {
        if (!isInventoryOpen)
        {
            controller.Move(calculatedMovement * Time.deltaTime);
        }
    }

    /// <summary>
    /// Handles interaction with nearby interactable objects.
    /// </summary>
    private void HandleInteraction()
    {
        if (Input.GetKeyDown(KeyCode.E) && !isInventoryOpen)
        {
            TryInteractWithNearest();
        }
    }

    /// <summary>
    /// Finds and interacts with the nearest interactable object.
    /// </summary>
    private void TryInteractWithNearest()
    {
        if (nearbyInteractables.Count == 0) return;

        IInteractable nearest = FindNearestInteractable();
        GameObject heldItem = inventory?.GetObjectOnHand();
        nearest?.Interact(heldItem);
    }

    /// <summary>
    /// Finds the closest interactable in range.
    /// </summary>
    private IInteractable FindNearestInteractable()
    {
        IInteractable closest = null;
        float closestDistance = Mathf.Infinity;
        Vector3 position = transform.position;

        foreach (IInteractable interactable in nearbyInteractables)
        {
            if (interactable == null) continue;

            float distance = Vector3.Distance(
                position,
                (interactable as MonoBehaviour).transform.position
            );
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = interactable;
            }
        }

        return closest;
    }

    /// <summary>
    /// Adds interactables to the list when entering trigger.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & interactableMask) != 0)
        {
            IInteractable interactable = other.GetComponent<IInteractable>();
            if (interactable != null && !nearbyInteractables.Contains(interactable))
            {
                nearbyInteractables.Add(interactable);
            }
        }
    }

    /// <summary>
    /// Removes interactables from the list when exiting trigger.
    /// </summary>
    private void OnTriggerExit(Collider other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null)
        {
            nearbyInteractables.Remove(interactable);
        }
    }

    /// <summary>
    /// Handles toggling of the inventory UI and cursor state.
    /// </summary>
    private void HandleInventoryToggle()
    {
        if (Input.GetKeyDown(KeyCode.I) && inventory.HasItem("Mobile"))
        {
            StartCoroutine(ToggleInventory());
        }
    }

    /// <summary>
    /// Coroutine for toggling inventory with a small delay.
    /// </summary>
    private IEnumerator ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        isAiming = false; // Reset aim state
        yield return new WaitForSeconds(0.1f);
        inventory.ToggleInventory();
    }

    /// <summary>
    /// Updates all animator parameters based on current player state.
    /// </summary>
    private void UpdateAnimations()
    {
        UpdateMovementAnimation();
        UpdateHandStates();
    }

    /// <summary>
    /// Updates movement animation (example: walking).
    /// </summary>
    private void UpdateMovementAnimation()
    {
        bool isMoving = controller.velocity.magnitude > 0.1f;
        // Example: camAnim.SetBool("IsWalking", isMoving);
    }

    /// <summary>
    /// Updates hand animator states based on whether hands are holding items.
    /// </summary>
    private void UpdateHandStates()
    {
        bool leftHandOccupied = leftHandSlot.childCount > 0;
        bool rightHandOccupied = rightHandSlot.childCount > 0;

        leftHandAnimator.SetBool("isActive", leftHandOccupied);
        rightHandAnimator.SetBool("isActive", rightHandOccupied);
        leftHandAnimator.SetBool("isAiming", isAiming);
    }
}
