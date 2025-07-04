using System.Collections;
using UnityEngine;

// Este script recoge loc controles del jugador
public class PlayerController : MonoBehaviour
{
    // Movement settings
    public float playerSpeed = 20f; 
    public float stamina;
    public float interactDistance = 3f;
    public LayerMask interactMask;
    public LayerMask doorsMask;

    // Referencias
    public Animator camAnim;
    public Animator LHAnim;
    public Animator RHAnim;
    public Transform leftHand;
    public Transform rightHand;
    public InventoryManager inventoryManager;
    public UITextController textController;
    public GameManager gameManager;

    [Header("Sounds Configuration")]
    public AudioSource stepsPlayer;
    public AudioClip stepSound;
    public float stepInterval = 1f;

    CharacterController characterController;
    Camera mainCamera;
    float currentStamina;
    float speed;
    Vector3 movementVector;
    bool isWalking;
    bool isCloser;
    IInteractable currentInteractable;
    float stepTimer = 0f;

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
        GetInputs();
        SetAnimation();
        HandleFootsteps();
    }

    void GetInputs()
    {
        // Acercar mano izquierda (Para ver mejor el telefono móvil)
        if (Input.GetMouseButtonDown(1) && !inventoryManager.isInventoryOpen)
            isCloser = true;
        else if (Input.GetMouseButtonUp(1))
            isCloser = false;

        // Abrir/Cerrar el inventario
        if (Input.GetKeyDown(KeyCode.I) && inventoryManager.HasItem("Mobile"))
            StartCoroutine(OpenInventory());

        // Movimiento
        if (!inventoryManager.isInventoryOpen)
        {
            // Si tiene estamina, el jugador puede correr
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

        // Detecta el objeto con el que se puede interactuar
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

        // Interactua con el objeto detectado
        if (Input.GetKeyDown(KeyCode.E) && !inventoryManager.isInventoryOpen)
        {
            GameObject heldItem = inventoryManager?.GetRightHandObject();
            currentInteractable?.Interact(heldItem);
        }
    }
    
    // Actualiza las animaciones del jugador
    void SetAnimation()
    {
        camAnim.SetBool("IsWalking", isWalking);
        LHAnim.SetBool("isCloser", isCloser);
        LHAnim.SetBool("isActive", leftHand.childCount > 0);
        RHAnim.SetBool("isActive", rightHand.childCount > 0);
    }

    // Abre el inventario con un pequeño retardo
    IEnumerator OpenInventory()
    {
        yield return new WaitForSeconds(0.1f);
        inventoryManager.ToggleInventory();
    }

    // Gestiona el sonido de las pisadas del jugador
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "End")
            gameManager.gameComplete = true;
    }
}
