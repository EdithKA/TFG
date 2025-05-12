using UnityEngine;
using UnityEngine.Video;

public class PS2Interactable : MonoBehaviour, IInteractable
{
    [Header("Reward")]
    public Item crashSaveData;

    [Header("Componentes")]
    public Light led;
    public AudioClip bootSound;
    public VideoPlayer dvdPlayer;
    public VideoClip noSignal;
    public VideoClip correctDVD;
    public VideoClip wrongDVD;
    public GameTexts gameTexts;

    [Header("Configuración")]
    public string requiredDVD = "CrashDVD";

    private AudioSource audioSource;
    private UITextController uiTextController;
    private InventoryManager inventoryManager;
    private bool completed;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        uiTextController = FindObjectOfType<UITextController>();
        inventoryManager = FindObjectOfType<InventoryManager>();
        led.color = Color.red;
        gameTexts = uiTextController.gameTexts;

        dvdPlayer.loopPointReached += VideoEnded;
        dvdPlayer.clip = noSignal;
        dvdPlayer.Play();
    }

    void VideoEnded(VideoPlayer vp)
    {
        // Si el video correcto ha terminado, doy el premio
        if (completed)
        {
            inventoryManager.AddItem(crashSaveData);
            uiTextController.ShowThought($"Parece que he conseguido un...¿{crashSaveData.displayName}?");

        }
        // Siempre vuelvo a no signal
        dvdPlayer.clip = noSignal;
        dvdPlayer.Play();
        led.color = Color.red;
    }

    public void Interact(GameObject objectOnHand = null)
    {
        if (!completed)
        {
            if (objectOnHand != null)
            {
                ItemController item = objectOnHand.GetComponent<ItemController>();
                if (item != null && item.itemData.type == "DVD")
                {
        

                    if (item.itemData.itemID == requiredDVD)
                    {
                        completed = true;
                        uiTextController.ShowThought(gameTexts.dvdCorrectMessage);
                        dvdPlayer.clip = correctDVD;
                        led.color = Color.green;
                    }
                    else
                    {
                        uiTextController.ShowThought(gameTexts.dvdError);
                        dvdPlayer.clip = wrongDVD;
                        led.color = Color.yellow;
                    }

                    dvdPlayer.Play();
                    audioSource.PlayOneShot(bootSound);

                    inventoryManager.RemoveItem(item.itemData);
                    Destroy(objectOnHand);
                }
                else
                {
                    uiTextController.ShowThought(gameTexts.dvdMissing);
                }
            }
            else
            {
                uiTextController.ShowThought(gameTexts.dvdMissing);
            }
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
