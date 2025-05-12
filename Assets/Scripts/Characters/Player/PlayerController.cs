using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public UITextController textController;

    [Header("Hands Settings")]
    public Animator LHAnim;
    public Transform leftHand;
    public bool LActive = false;
    public bool isCloser = false;

    public Animator RHAnim;
    public Transform rightHand;
    public bool RActive = false;

    [Header("Interaction Settings")]
    public float interactionRadius = 3f;
    private List<IInteractable> nearbyInteractables = new List<IInteractable>();

    void Start()
    {
        inventoryManager = FindObjectOfType<InventoryManager>();
        currentStamina = stamina;
        characterController = GetComponent<CharacterController>();
        textController = FindObjectOfType<UITextController>();

        // Configurar collider de interacción
        SphereCollider interactionCollider = gameObject.AddComponent<SphereCollider>();
        interactionCollider.isTrigger = true;
        interactionCollider.radius = interactionRadius;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && inventoryManager.HasItem("Mobile"))
        {
            isInventoryOpen = !isInventoryOpen;
            StartCoroutine(OpenInventory());
            isCloser = !isCloser;
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
        setAnimation();

        if (Input.GetKeyDown(KeyCode.E) && !isInventoryOpen)
        { 
            InteractWithObject();
            
        }
       

        LActive = leftHand.childCount > 0;
        RActive = rightHand.childCount > 0;
    }

    void OnTriggerEnter(Collider other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null && !nearbyInteractables.Contains(interactable))
        {
            nearbyInteractables.Add(interactable);
            Debug.Log("Interactuable detectado: " + other.name);
        }
    }

    void OnTriggerExit(Collider other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null)
        {
            nearbyInteractables.Remove(interactable);
            Debug.Log("Interactuable eliminado: " + other.name);
        }
    }

    void InteractWithObject()
    {
        if (nearbyInteractables.Count == 0) return;

        // Limpiar interactuables nulos
        nearbyInteractables.RemoveAll(x => x == null);

        // Buscar el más cercano
        IInteractable nearest = null;
        float minDistance = Mathf.Infinity;
        Vector3 playerPosition = transform.position;

        foreach (IInteractable interactable in nearbyInteractables)
        {
            float distance = Vector3.Distance(
                playerPosition,
                (interactable as MonoBehaviour).transform.position
            );

            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = interactable;
            }
        }

        if (nearest != null)
        {
            GameObject objectInHand = inventoryManager?.equippedRight;
            nearest.Interact(objectInHand);
        }
    }

    void setAnimation()
    {
        camAnim.SetBool("IsWalking", isWalking);
        LHAnim.SetBool("isCloser", isCloser);
        LHAnim.SetBool("isActive", LActive);
        RHAnim.SetBool("isActive", RActive);
    }

    void CheckForHeadBob()
    {
        isWalking = characterController.velocity.magnitude > 0.1f;
    }

    void GetInput()
    {
        if (Input.GetKey(KeyCode.LeftShift) && currentStamina > 0)
        {
            speed = playerSpeed * 3;
            currentStamina -= 0.1f;
        }
        else
        {
            speed = playerSpeed;
            if (currentStamina < stamina) currentStamina += 0.1f;
        }

        if (Input.GetMouseButtonDown(1)) isCloser = true;
        else if (Input.GetMouseButtonUp(1)) isCloser = false;

        inputVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
        inputVector = transform.TransformDirection(inputVector);
        movementVector = (inputVector * speed) + (Vector3.up * Gravity);
    }

    IEnumerator OpenInventory()
    {
        yield return new WaitForSeconds(0.1f);
        inventoryManager.ToggleInventory();
        isInventoryOpen = inventoryManager.IsInventoryOpen;
    }

    void MovePlayer()
    {
        characterController.Move(movementVector * Time.deltaTime);
    }
}
