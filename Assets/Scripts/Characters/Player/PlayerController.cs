using System.Collections;
using System.Collections.Generic;
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
    public Animator LHAnim;
    public bool LActive = false;
    public bool isCloser = false;

    public Animator RHAnim;
    public bool RActive = false;

    [Header("Interaction Settings")]
    public float radioInteraccion = 3f; // Radio de detección de objetos
    public LayerMask capaInteractuables;
    private List<IInteractuable> interactuablesCercanos = new List<IInteractuable>();

    void Start()
    {
        inventoryManager = FindObjectOfType<InventoryManager>();
        currentStamina = stamina;
        characterController = GetComponent<CharacterController>();

        // Configurar collider de interacción
        SphereCollider interactionCollider = gameObject.AddComponent<SphereCollider>();
        interactionCollider.isTrigger = true;
        interactionCollider.radius = radioInteraccion;
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
            InteractuarConObjeto();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & capaInteractuables) != 0)
        {
            IInteractuable interactuable = other.GetComponent<IInteractuable>();
            if (interactuable != null && !interactuablesCercanos.Contains(interactuable))
            {
                interactuablesCercanos.Add(interactuable);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & capaInteractuables) != 0)
        {
            IInteractuable interactuable = other.GetComponent<IInteractuable>();
            if (interactuable != null)
            {
                interactuablesCercanos.Remove(interactuable);
            }
        }
    }

    void InteractuarConObjeto()
    {
        if (interactuablesCercanos.Count == 0) return;

        // Buscar el interactuable más cercano
        IInteractuable masCercano = null;
        float distanciaMinima = Mathf.Infinity;
        Vector3 posicionJugador = transform.position;

        foreach (IInteractuable interactuable in interactuablesCercanos)
        {
            if (interactuable == null) continue;

            float distancia = Vector3.Distance(
                posicionJugador,
                (interactuable as MonoBehaviour).transform.position
            );

            if (distancia < distanciaMinima)
            {
                distanciaMinima = distancia;
                masCercano = interactuable;
            }
        }

        if (masCercano != null)
        {
            GameObject objetoEnMano = inventoryManager?.GetObjectOnHand();
            masCercano.Interact(objetoEnMano);
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
