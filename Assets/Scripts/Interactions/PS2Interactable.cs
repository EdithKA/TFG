using UnityEngine;
using UnityEngine.Video;
using System;

/**
 * @brief PS2 puzzle: insert correct DVD, get reward.
 */
public class PS2Interactable : MonoBehaviour, IInteractable, IPuzleObjective
{
    public bool isComplete { get; private set; } ///< Puzzle done
    public event Action onCompleted;             ///< On complete event

    [Header("Reward")]
    public Item crashSaveData;                   ///< Reward item

    [Header("Components")]
    public Light led;                            ///< Status LED
    public AudioClip bootSound;                  ///< Boot sound
    public VideoPlayer dvdPlayer;                ///< Video player
    public VideoClip noSignal;                   ///< No DVD video
    public VideoClip correctDVD;                 ///< Correct DVD video
    public VideoClip wrongDVD;                   ///< Wrong DVD video
    public GameTexts gameTexts;                  ///< UI texts

    [Header("Config")]
    public string requiredDVD = "CrashDVD";      ///< Needed DVD

    AudioSource audioSource;                     ///< Audio ref
    UITextController uiTextController;           ///< UI ref
    InventoryManager inventoryManager;           ///< Inventory ref

    /**
     * @brief Get refs, set up.
     */
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        uiTextController = FindObjectOfType<UITextController>();
        inventoryManager = FindObjectOfType<InventoryManager>();
        led.color = Color.red;

        dvdPlayer.loopPointReached += OnVideoEnded;
        dvdPlayer.clip = noSignal;
        dvdPlayer.Play();
    }

    /**
     * @brief Show prompt on hover.
     */
    public void OnHoverEnter(UITextController textController)
    {
        if (!isComplete)
            textController.ShowInteraction(gameTexts.interactMessage, Color.cyan);
    }

    /**
     * @brief Clear prompt on exit.
     */
    public void OnHoverExit()
    {
        uiTextController.ClearMessages();
    }

    /**
     * @brief Insert DVD, check if correct.
     */
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

    /**
     * @brief On video end, give reward if solved.
     */
    void OnVideoEnded(VideoPlayer vp)
    {
        if (isComplete && !inventoryManager.HasItem(crashSaveData.itemID))
        {
            onCompleted?.Invoke();
            inventoryManager.AddItem(crashSaveData);
            uiTextController.ShowThought($"Looks like I got a...¿{crashSaveData.displayName}?");
        }

        dvdPlayer.clip = noSignal;
        dvdPlayer.Play();
        led.color = Color.red;
        isComplete = false;
    }
}
