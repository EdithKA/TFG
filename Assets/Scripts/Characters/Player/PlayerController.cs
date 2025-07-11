using System.Collections;
using UnityEngine;

/**
 * @brief Player controls: move, interact, animate, sound.
 */
public class PlayerController : MonoBehaviour
{
    // --- Movement ---
    public float playerSpeed = 20f;           ///< Move speed
    public float stamina;                      ///< Max stamina
    public float interactDistance = 3f;        ///< Interact range
    public LayerMask interactMask;             ///< Interact layer
    public LayerMask doorsMask;                ///< Doors layer

    // --- Refs ---
    public Animator camAnim;                   ///< Camera animator
    public Animator LHAnim;                    ///< Left hand animator
    public Animator RHAnim;                    ///< Right hand animator
    public Transform leftHand;                 ///< Left hand
    public Transform rightHand;                ///< Right hand
    public InventoryManager inventoryManager;  ///< Inventory
    public UITextController textController;    ///< UI controller
    public GameManager gameManager;            ///< Game manager
    CharacterController characterController;   ///< Character controller
    GameTexts gameTexts;                       ///< UI texts

    // --- Sounds ---
    public AudioSource stepsPlayer;            ///< Footstep audio
    public AudioClip stepSound;                ///< Footstep clip
    public float stepInterval = 1f;            ///< Step interval

    Camera mainCamera;                         ///< Main camera
    float currentStamina;                      ///< Current stamina
    float speed;                               ///< Current speed
    Vector3 movementVector;                    ///< Move vector

    bool isWalking;                            ///< Is walking
    bool isCloser;                             ///< Left hand closer (mobile)
    IInteractable currentInteractable;         ///< Current interactable
    float stepTimer = 0f;                      ///< Step timer

    /**
     * @brief Get refs and setup.
     */
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        mainCamera = Camera.main;
        currentStamina = stamina;
        inventoryManager = FindObjectOfType<InventoryManager>();
        textController = FindObjectOfType<UITextController>();
        gameManager = FindObjectOfType<GameManager>();
        gameTexts = textController.gameTexts;
        textController.ShowThought(gameTexts.startThought);
    }

    /**
     * @brief Handle input, animation, footsteps.
     */
    void Update()
    {
        GetInputs();
        SetAnimation();
        HandleFootsteps();
    }

    /**
     * @brief All player input.
     */
    void GetInputs()
    {
        // Left hand closer (mobile)
        if (Input.GetMouseButtonDown(1) && !inventoryManager.isInventoryOpen)
            isCloser = true;
        else if (Input.GetMouseButtonUp(1))
            isCloser = false;

        // Open/close inventory
        if (Input.GetKeyDown(KeyCode.I) && inventoryManager.HasItem("Mobile"))
            StartCoroutine(OpenInventory());

        // Move if inventory closed
        if (!inventoryManager.isInventoryOpen)
        {
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

        // Detect interactables
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        IInteractable newInteractable = null;

        if (Physics.Raycast(ray, out hit, interactDistance, interactMask, QueryTriggerInteraction.Collide))
            newInteractable = hit.collider.GetComponent<IInteractable>();
        else if (Physics.Raycast(ray, out hit, interactDistance, doorsMask, QueryTriggerInteraction.Collide))
            newInteractable = hit.collider.GetComponent<IInteractable>();

        // Handle hover enter/exit
        if (newInteractable != currentInteractable)
        {
            if (currentInteractable != null)
                currentInteractable.OnHoverExit();
            currentInteractable = newInteractable;
            if (currentInteractable != null)
                currentInteractable.OnHoverEnter(textController);
        }

        // Interact with object
        if (Input.GetKeyDown(KeyCode.E) && !inventoryManager.isInventoryOpen)
        {
            GameObject heldItem = inventoryManager?.GetRightHandObject();
            currentInteractable?.Interact(heldItem);
        }
    }

    /**
     * @brief Update animation params.
     */
    void SetAnimation()
    {
        camAnim.SetBool("IsWalking", isWalking);
        LHAnim.SetBool("isCloser", isCloser);
        LHAnim.SetBool("isActive", leftHand.childCount > 0);
        RHAnim.SetBool("isActive", rightHand.childCount > 0);
    }

    /**
     * @brief Open inventory (delayed).
     */
    IEnumerator OpenInventory()
    {
        yield return new WaitForSeconds(0.1f);
        inventoryManager.ToggleInventory();
    }

    /**
     * @brief Play footsteps if moving.
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
     * @brief On trigger: change scene if "End".
     */
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "End")
            gameManager.ChangeScene("End");
    }
}
