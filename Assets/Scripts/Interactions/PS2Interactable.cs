using UnityEngine;
using UnityEngine.Video;

public class PS2Interactable : MonoBehaviour, IInteractable
{
    [Header("Reward Settings")]
    public Item crashSaveData;

    [Header("Components")]
    public Light led;
    public AudioClip bootSound;
    public VideoPlayer dvdPlayer;
    public VideoClip noSignal;
    public VideoClip correctDVD;
    public VideoClip wrongDVD;
    public GameTexts gameTexts;

    [Header("Configuration")]
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

        dvdPlayer.loopPointReached += OnVideoEnded;
        dvdPlayer.clip = noSignal;
        dvdPlayer.Play();
    }

    // IInteractable implementation
    public void OnHoverEnter(UITextController textController)
    {
        if (!completed)
            textController.ShowInteraction(gameTexts.interactMessage, Color.cyan);
    }

    public void OnHoverExit()
    {
        uiTextController.ClearMessages();
    }

    public void Interact(GameObject objectOnHand = null)
    {
        if (!completed)
        {
            if (objectOnHand != null)
            {
                ItemInteractable item = objectOnHand.GetComponent<ItemInteractable>();
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



    private void OnVideoEnded(VideoPlayer vp)
    {
        if (completed && !inventoryManager.HasItem(crashSaveData.itemID))
        {
            inventoryManager.AddItem(crashSaveData);
            uiTextController.ShowThought($"Parece que he conseguido un...¿{crashSaveData.displayName}?");
        }

        dvdPlayer.clip = noSignal;
        dvdPlayer.Play();
        led.color = Color.red;
        completed = false;
    }
}
