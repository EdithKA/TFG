using UnityEngine;
using UnityEngine.Video;

/**
 * @brief Handles the PS2 console interaction: accepts a specific DVD, plays video, and updates UI.
 *        Designed for a puzzle or Easter egg in a survival horror game.
 */
public class PS2Interactable : MonoBehaviour, IInteractable
{
    [Header("Component References")]
    public Light led;                           /// Reference to the PS2 LED light.
    public AudioClip bootEffect;                /// Audio clip for the boot sound.
    public string requiredItemName = "CrashDVD";/// The name of the correct DVD item.
    private AudioSource audioSource;            /// Audio source for playing effects.
    private UITextController uiController;      /// Reference to UI text controller.
    private bool playerInRange;                 /// Is the player in interaction range?
    private PlayerController playerController;  /// Reference to the player controller.
    private InventoryManager inventoryManager;  /// Reference to the inventory manager.

    [Header("UI Messages")]
    private string interactionMessage = "Press [E] to insert DVD";
    private string successMessage = "Correct DVD! Booting game...";
    private string errorMessageNoDVD = "You need a DVD";
    private string errorMessageWrongDVD = "Wrong DVD";

    [Header("Video Components")]
    public VideoPlayer dvdPlayer;               /// Video player component for PS2.
    public VideoClip bootClip;                  /// Video clip for successful boot.
    public VideoClip errorClip;                 /// Video clip for boot error.

    /**
     * @brief Initializes references and sets the initial LED color.
     */
    private void Start()
    {
        led = GetComponentInChildren<Light>();
        audioSource = GetComponent<AudioSource>();
        uiController = FindObjectOfType<UITextController>();
        playerController = FindObjectOfType<PlayerController>();
        inventoryManager = FindObjectOfType<InventoryManager>();
        led.color = Color.red;
    }

    /**
     * @brief Checks for player input when in range.
     */
    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            // Get the object currently held in the player's hand
            GameObject heldObject = inventoryManager?.GetObjectOnHand();
            Interact(heldObject);
        }
    }

    /**
     * @brief Handles the interaction logic: checks item, plays video, updates UI, and removes item.
     * @param objectOnHand The object currently held by the player.
     */
    public void Interact(GameObject objectOnHand = null)
    {
        if (objectOnHand != null)
        {
            ItemController item = objectOnHand.GetComponent<ItemController>();
            if (item != null && item.itemData != null)
            {
                if (item.itemData.type == "DVD")
                {
                    if (item.itemData.itemName == requiredItemName)
                    {
                        // Success: correct DVD
                        uiController.ShowMessage(UIMessageType.Read, successMessage);
                        dvdPlayer.clip = bootClip;
                    }
                    else
                    {
                        // Wrong DVD
                        uiController.ShowMessage(UIMessageType.Read, errorMessageWrongDVD);
                        dvdPlayer.clip = errorClip;
                    }
                    led.color = Color.green;
                    audioSource.PlayOneShot(bootEffect);
                    dvdPlayer.Play();
                    Destroy(objectOnHand);
                    inventoryManager.RemoveItem(item.itemData);
                }
                else
                {
                    // Held item is not a DVD
                    uiController.ShowMessage(UIMessageType.Read, errorMessageNoDVD);
                }
                return;
            }
        }

        // No object in hand
        uiController.ShowMessage(UIMessageType.Read, errorMessageNoDVD);
    }

    /**
     * @brief Shows interaction message when player enters range.
     */
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            uiController.ShowMessage(UIMessageType.Interact, interactionMessage);
        }
    }

    /**
     * @brief Clears the interaction flag when player leaves range.
     */
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
