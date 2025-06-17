using System.Collections;
using UnityEngine;

/// <summary>
/// Controls player movement, stamina, inventory, and raycast-based interactions.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float playerSpeed = 20f;
    public float stamina;
    public float interactDistance = 3f;
    public LayerMask interactMask;

    [Header("References")]
    public Animator camAnim;
    public Animator LHAnim;
    public Animator RHAnim;
    public Transform leftHand;
    public Transform rightHand;
    public InventoryManager inventoryManager;
    public UITextController textController;
    public GameManager gameManager;

    CharacterController characterController;
    Camera mainCamera;
    float currentStamina;
    float speed;
    Vector3 movementVector;
    bool isWalking;
    public bool isInventoryOpen;
    bool isCloser;
    IInteractable currentInteractable;

    [Header("Sounds Configuration")]
    public AudioSource stepsPlayer;
    public AudioClip stepSound;
    public float stepInterval = 1f;

    float stepTimer = 0f;
    

    /// <summary>
    /// Initialize references and set starting values.
    /// </summary>
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        mainCamera = Camera.main;
        currentStamina = stamina;
        inventoryManager = FindObjectOfType<InventoryManager>();
        textController = FindObjectOfType<UITextController>();
        gameManager = FindObjectOfType<GameManager>();


    }

    /// <summary>
    /// Main update loop for movement, inventory, and interaction.
    /// </summary>
    void Update()
    {
        HandleInventoryToggle();
        HandleMovement();
        HandleInteraction();
        UpdateAnimations();
        HandleInteractionInput();
        HandleFootsteps();

    }

    /// <summary>
    /// Opens or closes the inventory if the player has the mobile.
    /// </summary>
    void HandleInventoryToggle()
    {
        if (Input.GetKeyDown(KeyCode.I) && inventoryManager.HasItem("Mobile"))
        {
            isInventoryOpen = !isInventoryOpen;
            StartCoroutine(OpenInventory());
            isCloser = !isCloser;
        }
    }

    /// <summary>
    /// Handles player movement and stamina.
    /// </summary>
    void HandleMovement()
    {
        if (!isInventoryOpen)
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

            if (Input.GetMouseButtonDown(1)) isCloser = true;
            else if (Input.GetMouseButtonUp(1)) isCloser = false;

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
    }

    /// <summary>
    /// Handles raycast interaction detection and hover UI.
    /// </summary>
    void HandleInteraction()
    {
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        IInteractable newInteractable = null;

        if (Physics.Raycast(ray, out hit, interactDistance, interactMask, QueryTriggerInteraction.Collide))
            newInteractable = hit.collider.GetComponent<IInteractable>();

        if (newInteractable != currentInteractable)
        {
            if (currentInteractable != null)
                currentInteractable.OnHoverExit();

            currentInteractable = newInteractable;
            if (currentInteractable != null)
                currentInteractable.OnHoverEnter(textController);
        }
    }

    /// <summary>
    /// Handles interaction input (E key).
    /// </summary>
    void HandleInteractionInput()
    {
        if (Input.GetKeyDown(KeyCode.E) && !isInventoryOpen)
        {
            GameObject heldItem = inventoryManager?.GetRightHandObject();
            currentInteractable?.Interact(heldItem);
        }
    }

    /// <summary>
    /// Updates camera and hand animations.
    /// </summary>
    void UpdateAnimations()
    {
        camAnim.SetBool("IsWalking", isWalking);
        LHAnim.SetBool("isCloser", isCloser);
        LHAnim.SetBool("isActive", leftHand.childCount > 0);
        RHAnim.SetBool("isActive", rightHand.childCount > 0);
    }

    /// <summary>
    /// Coroutine for inventory toggle delay.
    /// </summary>
    IEnumerator OpenInventory()
    {
        yield return new WaitForSeconds(0.1f);
        inventoryManager.ToggleInventory();
        isInventoryOpen = inventoryManager.IsInventoryOpen;
    }

    void HandleFootsteps()
    {
        if(isWalking & !isInventoryOpen && characterController.isGrounded)
        {
            stepTimer += Time.deltaTime;
            if(stepTimer >= stepInterval)
            {
                stepsPlayer.PlayOneShot(stepSound);
                stepTimer = 0f;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "End")
            gameManager.gameComplete = true;

    }

}
