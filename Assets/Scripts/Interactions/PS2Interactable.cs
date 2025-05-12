using UnityEngine;
using UnityEngine.Video;

public class PS2Interactable : MonoBehaviour, IInteractable
{
    [Header("Componentes")]
    public Light led;
    public AudioClip bootSound;
    public VideoPlayer dvdPlayer;
    public VideoClip correctDVD;
    public VideoClip wrongDVD;
    public GameTexts gameTexts;

    [Header("Configuración")]
    public string requiredDVD = "CrashDVD";

    private AudioSource audioSource;
    private UITextController uiTextController;
    private InventoryManager inventoryManager;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        uiTextController = FindObjectOfType<UITextController>();
        inventoryManager = FindObjectOfType<InventoryManager>();
        led.color = Color.red;
        gameTexts = uiTextController.gameTexts;
    }

    public void Interact(GameObject objectOnHand = null)
    {
        if (objectOnHand != null)
        {
            ItemController item = objectOnHand.GetComponent<ItemController>();
            if (item != null && item.itemData.type == "DVD")
            {
                // Detener reproducción actual antes de cambiar
                if (dvdPlayer.isPlaying)
                {
                    dvdPlayer.Stop();
                    audioSource.Stop();
                }

                // Determinar qué video reproducir
                if (item.itemData.itemName == requiredDVD)
                {
                    uiTextController.ShowThought(gameTexts.dvdCorrectMessage);
                    dvdPlayer.clip = correctDVD;
                    led.color = Color.green;
                }
                else
                {
                    uiTextController.ShowThought(gameTexts.dvdError);
                    dvdPlayer.clip = wrongDVD;
                    led.color = Color.yellow; // Color diferente para DVD incorrecto
                }

                // Reproducir nuevo video
                dvdPlayer.Play();
                audioSource.PlayOneShot(bootSound);

                // Eliminar DVD usado
                inventoryManager.RemoveItem(item.itemData);
                Destroy(objectOnHand);
            }
            else
            {
                uiTextController.ShowInteraction(gameTexts.dvdMissing, Color.red);
            }
        }
        else
        {
            uiTextController.ShowInteraction(gameTexts.dvdMissing, Color.red);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            uiTextController.ShowInteraction(gameTexts.interactMessage, Color.cyan);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            uiTextController.ClearMessages();
        }
    }
}
