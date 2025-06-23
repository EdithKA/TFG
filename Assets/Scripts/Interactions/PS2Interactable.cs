using UnityEngine;
using UnityEngine.Video;
using System;

/// <summary>
/// Controls PS2 puzzle interaction, DVD verification, and reward system.
/// Implements both interactable and puzzle objective interfaces.
/// </summary>
public class PS2Interactable : MonoBehaviour, IInteractable, IPuzzleObjective
{
    /// <summary>
    /// Indicates if the puzzle has been completed.
    /// </summary>
    public bool isComplete { get; private set; }

    /// <summary>
    /// Event triggered when the puzzle is completed.
    /// </summary>
    public event Action onCompleted;

    [Header("Reward Settings")]
    /// <summary>
    /// Item rewarded upon puzzle completion.
    /// </summary>
    public Item crashSaveData;

    [Header("Components")]
    /// <summary>
    /// Status LED light component.
    /// </summary>
    public Light led;

    /// <summary>
    /// Sound played when inserting a DVD.
    /// </summary>
    public AudioClip bootSound;

    /// <summary>
    /// Video soundPlayer component for DVD playback.
    /// </summary>
    public VideoPlayer dvdPlayer;

    /// <summary>
    /// Video clip shown when no DVD is inserted.
    /// </summary>
    public VideoClip noSignal;

    /// <summary>
    /// Video clip shown when correct DVD is inserted.
    /// </summary>
    public VideoClip correctDVD;

    /// <summary>
    /// Video clip shown when wrong DVD is inserted.
    /// </summary>
    public VideoClip wrongDVD;

    /// <summary>
    /// Game text configurations for UI messages.
    /// </summary>
    public GameTexts gameTexts;

    [Header("Configuration")]
    /// <summary>
    /// Required DVD item ID to complete the puzzle.
    /// </summary>
    public string requiredDVD = "CrashDVD";

    /// <summary>
    /// Audio source component for sound playback.
    /// </summary>
    private AudioSource audioSource;

    /// <summary>
    /// UI text controller for displaying messages.
    /// </summary>
    private UITextController uiTextController;

    /// <summary>
    /// Inventory manager reference.
    /// </summary>
    private InventoryManager inventoryManager;

    /// <summary>
    /// Initializes components and sets up initial state.
    /// </summary>
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

    /// <summary>
    /// Displays interaction prompt when hovered.
    /// </summary>
    /// <param name="textController">UI text controller reference.</param>
    public void OnHoverEnter(UITextController textController)
    {
        if (!isComplete)
            textController.ShowInteraction(gameTexts.interactMessage, Color.cyan);
    }

    /// <summary>
    /// Clears interaction prompt when hover ends.
    /// </summary>
    public void OnHoverExit()
    {
        uiTextController.ClearMessages();
    }

    /// <summary>
    /// Handles DVD insertion and verification.
    /// </summary>
    /// <param name="objectOnHand">DVD item being used.</param>
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

    /// <summary>
    /// Handles video end events and reward distribution.
    /// </summary>
    /// <param name="vp">Video soundPlayer that finished playback.</param>
    private void OnVideoEnded(VideoPlayer vp)
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
