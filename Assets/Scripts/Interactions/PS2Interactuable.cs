using UnityEngine;

public class PS2Interactuable : MonoBehaviour, IInteractuable
{
    [Header("Componentes")]
    public Light led;
    public AudioClip bootEffect;
    private string requiredItemName = "CrashDVD";
    private AudioSource audioSource;
    private UITextController uiController;
    private bool playerOnRange;
    private PlayerController playerController;

    [Header("Mensajes UI")]
    private string interactionMessage = "Presiona [E] para insertar DVD";
    private string successMessage = "¡DVD correcto! Iniciando juego...";
    private string errorMessage_1 = "Necesitas un DVD";
    private string errorMessage_2 = "DVD incorrecto";





    private void Start()
    {
        led = GetComponentInChildren<Light>();
        audioSource = GetComponent<AudioSource>();
        uiController = FindObjectOfType<UITextController>();
        playerController = FindObjectOfType<PlayerController>();

        led.color = Color.red;
    }

    private void Update()
    {
        if (playerOnRange && Input.GetKeyDown(KeyCode.E))
        {
            // Obtener objeto en mano del inventario
            GameObject objetoEnMano = FindObjectOfType<InventoryManager>()?.GetObjectOnHand();
            Interact(objetoEnMano);
        }
    }

    public void Interact(GameObject objectOnHand = null)
    {
        if (objectOnHand != null)
        {
            ItemController item = objectOnHand.GetComponent<ItemController>();
            if (item != null && item.itemData != null)
            {
                if(item.itemData.type == "DVD")
                {
                    if (item.itemData.itemName == requiredItemName)
                    {
                        // Éxito: DVD correcto
                        uiController.ShowMessage(UIMessageType.Read, successMessage);
                        led.color = Color.green;
                        audioSource.PlayOneShot(bootEffect);
                    }
                    else
                    {
                        uiController.ShowMessage(UIMessageType.Read, errorMessage_2);
                    }
                    Destroy(objectOnHand);
                }
                else
                {
                    // Error: DVD incorrecto
                    uiController.ShowMessage(UIMessageType.Read, errorMessage_1);
                }
                return;
            }
        }

        // No hay objeto en mano
        uiController.ShowMessage(UIMessageType.Read, errorMessage_1);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerOnRange = true;
            uiController.ShowMessage(UIMessageType.Interact, interactionMessage);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerOnRange = false;
        }
    }
}
