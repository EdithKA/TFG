using System.Collections;
using UnityEngine;

/**
 * @brief This script handles the player's controls.
 */
public class PlayerController : MonoBehaviour
{
    // Movement settings
    public float playerSpeed = 20f; ///< Player's base movement speed
    public float stamina; ///< Player's maximum stamina
    public float interactDistance = 3f; ///< Distance to interact with objects
    public LayerMask interactMask; ///< Layer mask for interactable objects
    public LayerMask doorsMask; ///< Layer mask for doors

    // References
    public Animator camAnim; ///< Camera animator
    public Animator LHAnim; ///< Left hand animator
    public Animator RHAnim; ///< Right hand animator
    public Transform leftHand; ///< Left hand transform
    public Transform rightHand; ///< Right hand transform
    public InventoryManager inventoryManager; ///< Reference to InventoryManager
    public UITextController textController; ///< Reference to UITextController
    public GameManager gameManager; ///< Reference to GameManager
    CharacterController characterController; ///< CharacterController component

    // Sounds
    public AudioSource stepsPlayer; ///< AudioSource for footsteps
    public AudioClip stepSound; ///< AudioClip for a single footstep
    public float stepInterval = 1f; ///< Interval between footsteps

    Camera mainCamera; ///< Reference to the main camera
    float currentStamina; ///< Current stamina value
    float speed; ///< Current movement speed
    Vector3 movementVector; ///< Movement vector

    bool isWalking; ///< Whether the player is currently walking
    bool isCloser; ///< Whether the left hand is closer (for mobile phone)
    IInteractable currentInteractable; ///< Currently detected interactable object
    float stepTimer = 0f; ///< Timer for footsteps

    /**
     * @brief Initializes references at the start.
     */
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        mainCamera = Camera.main;
        currentStamina = stamina;
        inventoryManager = FindObjectOfType<InventoryManager>();
        textController = FindObjectOfType<UITextController>();
        gameManager = FindObjectOfType<GameManager>();
    }

    /**
     * @brief Handles input, animations, and footsteps every frame.
     */
    void Update()
    {
        GetInputs();
        SetAnimation();
        HandleFootsteps();
    }

    /**
     * @brief Handles all player inputs.
     */
    void GetInputs()
    {
        // Bring left hand closer (to better see the mobile phone)
        if (Input.GetMouseButtonDown(1) && !inventoryManager.isInventoryOpen)
            isCloser = true;
        else if (Input.GetMouseButtonUp(1))
            isCloser = false;

        // Open/close the inventory
        if (Input.GetKeyDown(KeyCode.I) && inventoryManager.HasItem("Mobile"))
            StartCoroutine(OpenInventory());

        // Movement
        if (!inventoryManager.isInventoryOpen)
        {
            // If stamina is available, the player can run
            if (Input.GetKey(KeyCode.LeftShift) && currentStamina > 0)
            {
                speed = playerSpeed * 3;
                stepInterval = 0.25f;
                currentStamina -= 0.1f;
            }
            else
            {
                stepInterval = 0.95f;
                speed = playerSpeed;
                if (currentStamina < stamina) currentStamina += 0.1f;
            }

            Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
            movementVector = transform.TransformDirection(input) * speed + Vector3.up * -10f;
            characterController.Move(movementVector * Time.deltaTime);
            isWalking = characterController.velocity.magnitude > 0.1f;
        }
        else
        {
            speed = 0;
            isWalking = false;
            movementVector = Vector3.zero;
        }

        // Detects the object that can be interacted with
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        IInteractable newInteractable = null;

        if (Physics.Raycast(ray, out hit, interactDistance, interactMask, QueryTriggerInteraction.Collide))
            newInteractable = hit.collider.GetComponent<IInteractable>();
        else if (Physics.Raycast(ray, out hit, interactDistance, doorsMask, QueryTriggerInteraction.Collide))
            newInteractable = hit.collider.GetComponent<IInteractable>();

        if (newInteractable != currentInteractable)
        {
            if (currentInteractable != null)
                currentInteractable.OnHoverExit();
            currentInteractable = newInteractable;
            if (currentInteractable != null)
                currentInteractable.OnHoverEnter(textController);
        }

        // Interacts with the detected object
        if (Input.GetKeyDown(KeyCode.E) && !inventoryManager.isInventoryOpen)
        {
            GameObject heldItem = inventoryManager?.GetRightHandObject();
            currentInteractable?.Interact(heldItem);
        }
    }

    /**
     * @brief Updates the player's animations.
     */
    void SetAnimation()
    {
        camAnim.SetBool("IsWalking", isWalking);
        LHAnim.SetBool("isCloser", isCloser);
        LHAnim.SetBool("isActive", leftHand.childCount > 0);
        RHAnim.SetBool("isActive", rightHand.childCount > 0);
    }

    /**
     * @brief Opens the inventory with a small delay.
     */
    IEnumerator OpenInventory()
    {
        yield return new WaitForSeconds(0.1f);
        inventoryManager.ToggleInventory();
    }

    /**
     * @brief Handles the player's footstep sounds.
     */
    void HandleFootsteps()
    {
        if (isWalking && !inventoryManager.isInventoryOpen && characterController.isGrounded)
        {
            stepTimer += Time.deltaTime;
            if (stepTimer >= stepInterval)
            {
                stepsPlayer.PlayOneShot(stepSound);
                stepTimer = 0f;
            }
        }
    }

    /**
     * @brief Triggers when the player enters a collider.
     * @param other The collider entered by the player.
     */
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "End")
            gameManager.gameComplete = true;
    }
}
