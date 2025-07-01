using System.Collections;
using UnityEngine;

/// <summary>
/// Controls soundPlayer movement, stamina, inventory, and raycast-based interactions.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    /// <summary>
    /// Base movement speed of the soundPlayer.
    /// </summary>
    public float playerSpeed = 20f;

    /// <summary>
    /// Maximum stamina value.
    /// </summary>
    public float stamina;

    /// <summary>
    /// Maximum distance for interaction raycasts.
    /// </summary>
    public float interactDistance = 3f;

    /// <summary>
    /// Layer mask for interactable objects.
    /// </summary>
    public LayerMask interactMask;
    public LayerMask doorsMask;

    [Header("References")]
    /// <summary>
    /// Animator for camera movement.
    /// </summary>
    public Animator camAnim;

    /// <summary>
    /// Animator for left hand.
    /// </summary>
    public Animator LHAnim;

    /// <summary>
    /// Animator for right hand.
    /// </summary>
    public Animator RHAnim;

    /// <summary>
    /// Transform for left hand item position.
    /// </summary>
    public Transform leftHand;

    /// <summary>
    /// Transform for right hand item position.
    /// </summary>
    public Transform rightHand;

    /// <summary>
    /// Reference to inventory manager.
    /// </summary>
    public InventoryManager inventoryManager;

    /// <summary>
    /// Reference to UI text controller.
    /// </summary>
    public UITextController textController;

    /// <summary>
    /// Reference to game manager.
    /// </summary>
    public GameManager gameManager;

    [Header("Sounds Configuration")]
    /// <summary>
    /// Audio source for footsteps.
    /// </summary>
    public AudioSource stepsPlayer;

    /// <summary>
    /// Footstep sound clip.
    /// </summary>
    public AudioClip stepSound;

    /// <summary>
    /// Time interval between footstep sounds.
    /// </summary>
    public float stepInterval = 1f;

    /// <summary>
    /// Character controller component reference.
    /// </summary>
    private CharacterController characterController;

    /// <summary>
    /// Main camera reference.
    /// </summary>
    private Camera mainCamera;

    /// <summary>
    /// Current stamina value.
    /// </summary>
    private float currentStamina;

    /// <summary>
    /// Current movement speed.
    /// </summary>
    private float speed;

    /// <summary>
    /// Current movement vector.
    /// </summary>
    private Vector3 movementVector;

    /// <summary>
    /// Is the soundPlayer currently walking?
    /// </summary>
    private bool isWalking;

    /// <summary>
    /// Is the soundPlayer in "closer" mode (zoomed view)?
    /// </summary>
    public bool isCloser;

    /// <summary>
    /// Currently focused interactable object.
    /// </summary>
    private IInteractable currentInteractable;

    /// <summary>
    /// Timer for footstep sounds.
    /// </summary>
    private float stepTimer = 0f;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        mainCamera = Camera.main;
        currentStamina = stamina;
        inventoryManager = FindObjectOfType<InventoryManager>();
        textController = FindObjectOfType<UITextController>();
        gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        HandleZoom();
        HandleInventoryToggle();
        HandleMovement();
        HandleInteraction();
        Update1Animations();
        HandleInteractionInput();
        HandleFootsteps();
    }

    /// <summary>
    /// Handles mobile phone zoom based on right mouse button
    /// </summary>
    void HandleZoom()
    {
        if (Input.GetMouseButtonDown(1) && !inventoryManager.IsInventoryOpen)
        {
            isCloser = true;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            isCloser = false;
        }
    }

    void HandleInventoryToggle()
    {
        if (Input.GetKeyDown(KeyCode.I) && inventoryManager.HasItem("Mobile"))
        {
            StartCoroutine(OpenInventory());
        }
    }

    void HandleMovement()
    {
        if (!inventoryManager.IsInventoryOpen)
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
    }

    void HandleInteraction()
    {
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
    }

    void HandleInteractionInput()
    {
        if (Input.GetKeyDown(KeyCode.E) && !inventoryManager.IsInventoryOpen)
        {
            GameObject heldItem = inventoryManager?.GetRightHandObject();
            currentInteractable?.Interact(heldItem);
        }
    }

    void Update1Animations()
    {
        camAnim.SetBool("IsWalking", isWalking);
        LHAnim.SetBool("isCloser", isCloser);
        LHAnim.SetBool("isActive", leftHand.childCount > 0);
        RHAnim.SetBool("isActive", rightHand.childCount > 0);
    }

    IEnumerator OpenInventory()
    {
        yield return new WaitForSeconds(0.1f);
        inventoryManager.ToggleInventory();
    }

    void HandleFootsteps()
    {
        if (isWalking && !inventoryManager.IsInventoryOpen && characterController.isGrounded)
        {
            stepTimer += Time.deltaTime;
            if (stepTimer >= stepInterval)
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
