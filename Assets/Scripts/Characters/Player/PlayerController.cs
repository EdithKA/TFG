using System.Collections;
using UnityEngine;

/// <summary>
/// Controls the player's movement, stamina, inventory, animation logic, and object interaction.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float playerSpeed = 20f;
    float speed;
    private CharacterController characterController;
    public Animator camAnim;
    public bool isWalking;

    private Vector3 inputVector;
    private Vector3 movementVector;
    private float Gravity = -10f;

    [Header("Stamina Settings")]
    public float stamina;
    public float currentStamina;

    [Header("Inventory Settings")]
    public InventoryManager inventoryManager;
    public bool isInventoryOpen = false;

    [Header("Hands Settings")]
    // Left Hand
    public Animator LHAnim;
    public bool LActive = false;
    public bool isCloser = false;

    // Right Hand
    public Animator RHAnim;
    public bool RActive = false;

    [Header("Interaction Settings")]
    public float distanciaInteraccion = 2f; // Distancia máxima para interactuar
    public LayerMask capaInteractuables;    // Asigna la capa de objetos interactuables en el inspector

    /**
     * @brief Initialization of components and player stats.
     */
    void Start()
    {
        inventoryManager = FindObjectOfType<InventoryManager>();
        currentStamina = stamina;
        characterController = GetComponent<CharacterController>();
    }

    /**
     * @brief Main update loop. Handles input, movement, animation updates, and interaction.
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

        // --- INTERACCIÓN ---
        if (Input.GetKeyDown(KeyCode.E) && !isInventoryOpen)
        {
            InteractuarConObjeto();
        }
    }

    /**
     * @brief Método para lanzar el Raycast y realizar la interacción.
     */
    void InteractuarConObjeto()
    {
        Ray ray = new Ray(transform.position + Vector3.up, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, distanciaInteraccion, capaInteractuables))
        {
            IInteractuable interactuable = hit.collider.GetComponent<IInteractuable>();
            if (interactuable != null)
            {
                // Obtén el objeto en mano desde el inventario (ajusta esto según tu sistema real)
                GameObject objetoEnMano = inventoryManager != null ? inventoryManager.GetObjectOnHand() : null;
                interactuable.Interact(objetoEnMano);
            }
        }
    }

    /**
     * @brief Updates all animation parameters for camera and hands.
     */
    void setAnimation()
    {
        camAnim.SetBool("IsWalking", isWalking);
        LHAnim.SetBool("isCloser", isCloser);
        LHAnim.SetBool("isActive", LActive);
        RHAnim.SetBool("isActive", RActive);
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
