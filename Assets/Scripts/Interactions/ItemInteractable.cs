using UnityEngine;

/// <summary>
/// Represents an interactable item that can be collected by the the player.
/// Implements the IInteractable interface for interaction handling.
/// </summary>
public class ItemInteractable : MonoBehaviour, IInteractable
{
    [Header("Configuration")]
    /// <summary>
    /// Data container for the item's properties.
    /// </summary>
    public Item itemData;

    /// <summary>
    /// Flag indicating if the item is currently being held by the the player.
    /// </summary>
    public bool isHeld = false;

    [Header("References")]
    /// <summary>
    /// Reference to the inventory manager.
    /// </summary>
    public InventoryManager inventoryManager;

    /// <summary>
    /// Reference to the UI text controller.
    /// </summary>
    public UITextController uiTextController;

    /// <summary>
    /// Reference to the the player controller.
    /// </summary>
    public PlayerController playerController;

    /// <summary>
    /// Reference to the the player stats component.
    /// </summary>
    public Stats stats;

    /// <summary>
    /// Initializes references to necessary components.
    /// </summary>
    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        inventoryManager = playerController.inventoryManager;
        uiTextController = playerController.textController;
        stats = FindObjectOfType<Stats>();
    }

    /// <summary>
    /// Displays interaction prompt when the item is hovered over.
    /// </summary>
    /// <param name="textController">UI text controller reference.</param>
    public void OnHoverEnter(UITextController textController)
    {
        if (!isHeld)
            textController.ShowInteraction(textController.gameTexts.collectMessage);
    }

    /// <summary>
    /// Clears interaction prompt when hover ends.
    /// </summary>
    public void OnHoverExit()
    {
        uiTextController.ClearMessages();
    }

    /// <summary>
    /// Handles item interaction logic when the the player interacts with the item.
    /// </summary>
    /// <param name="objectOnHand">Item currently held by the the player (unused).</param>
    public void Interact(GameObject objectOnHand = null)
    {
        if (isHeld) return;

        // Special case: Mobile is required to collect other items
        if (!inventoryManager.HasItem("Mobile") && itemData.itemID != "Mobile")
        {
            uiTextController.ShowThought(uiTextController.gameTexts.needMobileMessage);
            return;
        }

        // Special handling for Mobile item
        if (itemData.name == "Mobile")
        {
            stats.hasPhone = true;
            AudioSource audioSource = gameObject.GetComponent<AudioSource>();
            if (audioSource != null) audioSource.Stop();
        }

        PickUp();
    }

    /// <summary>
    /// Picks up the item and adds it to the inventory.
    /// </summary>
    private void PickUp()
    {
        inventoryManager.AddItem(itemData);

        if (itemData.itemID == "Mobile")
        {
            // Parent the mobile to the player's hand
            isHeld = true;
            transform.SetParent(playerController.leftHand);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
        else
        {
            // Destroy regular items after collection
            Destroy(gameObject);
        }

        uiTextController.ClearMessages();
    }
}
