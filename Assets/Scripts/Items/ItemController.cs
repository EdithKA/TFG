using UnityEngine;

/**
 * @brief Handles item pickup, inventory integration, and player interaction logic.
 *        Supports both equippable items (like the phone) and consumables.
 */
public class ItemController : MonoBehaviour
{
    [Header("Player References")]
    public Transform playerHand;                    /// Transform where the item will be attached when held.

    [Header("Item Data")]
    public Item itemData;                           /// ScriptableObject containing this item's properties.

    [Header("State Management")]
    private bool isPlayerInTrigger = false;         /// True if the player is near enough to interact.
    public bool isHeld = false;                     /// True if the item is currently equipped.

    [Header("UI Components")]
    public UITextController uiTextController;       /// Reference to the UI text controller.

    [Header("Inventory System")]
    public InventoryManager inventoryManager;       /// Reference to the inventory manager.

    private PlayerController playerController;      /// Reference to the player controller.

    /**
     * @brief Initializes references and assigns the correct hand for equippable items.
     */
    private void Start()
    {
        playerController = FindAnyObjectByType<PlayerController>();

        if (uiTextController == null)
            uiTextController = FindAnyObjectByType<UITextController>();

        if (inventoryManager == null)
            inventoryManager = FindAnyObjectByType<InventoryManager>();

        // Decide which hand to use for this item (e.g., phone goes in left hand)
        if (itemData != null && itemData.itemName == "Mobile")
        {
            if (inventoryManager.leftHand != null)
                playerHand = inventoryManager.leftHand;
        }
        else
        {
            if (inventoryManager.rightHand != null)
                playerHand = inventoryManager.rightHand;
        }

        if (playerHand == null)
            Debug.LogWarning("Player hand reference not assigned!");
    }

    /**
     * @brief Handles player input for picking up the item.
     */
    private void Update()
    {
        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.E) && !isHeld)
        {
            PickUp();
        }
    }

    /**
     * @brief Called when the player enters the item's trigger area.
     */
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = true;
            uiTextController.ShowMessage(UIMessageType.Collect, "Press E to pick up");
        }
    }

    /**
     * @brief Called when the player exits the item's trigger area.
     */
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
            uiTextController.ClearMessage();
        }
    }

    /**
     * @brief Handles the logic for picking up and equipping or consuming the item.
     */
    private void PickUp()
    {
        Debug.Log($"Acquired item: {itemData.itemName}");
        inventoryManager.AddItem(itemData);

        // Show pickup message (do not clear immediately)
        uiTextController.ShowMessage(UIMessageType.Collected, itemData.collectedText);

        // If the item is the phone, equip it in the hand; otherwise, destroy after pickup
        if (itemData.itemName == "Mobile")
        {
            isHeld = true;
            if (playerHand != null)
            {
                transform.SetParent(playerHand);
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
            }
            else
            {
                Debug.LogWarning("Attempted to equip item without valid hand reference!");
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
