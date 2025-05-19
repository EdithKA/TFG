using UnityEngine;
using UnityEngine.Video;
using System;

public class PS2Interactable : MonoBehaviour, IInteractable, IPuzzleObjective
{
    public bool isComplete { get; private set; }
    public event Action onCompleted; // Evento correcto según la interfaz

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
        if (!isComplete)
            textController.ShowInteraction(gameTexts.interactMessage, Color.cyan);
    }

    public void OnHoverExit()
    {
        uiTextController.ClearMessages();
    }

    public void Interact(GameObject objectOnHand = null)
    {
        if (!isComplete)
        {
            if (objectOnHand != null)
            {
                ItemInteractable item = objectOnHand.GetComponent<ItemInteractable>();
                if (item != null && item.itemData.type == "DVD")
                {
                    if (item.itemData.itemID == requiredDVD)
                    {
                        isComplete = true;
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
        if (isComplete && !inventoryManager.HasItem(crashSaveData.itemID))
        {
            onCompleted?.Invoke(); // Evento correcto
            inventoryManager.AddItem(crashSaveData);
            uiTextController.ShowThought($"Parece que he conseguido un...¿{crashSaveData.displayName}?");
        }

        dvdPlayer.clip = noSignal;
        dvdPlayer.Play();
        led.color = Color.red;
        isComplete = false; // Si quieres que solo se pueda completar una vez, elimina esta línea
    }
}
