using UnityEngine;

/**
 * @brief Collectible item. Implements interaction.
 */
public class ItemInteractable : MonoBehaviour, IInteractable
{
    [Header("Config")]
    public Item itemData;                    ///< Item data
    public bool isHeld = false;              ///< Is the item held

    [Header("Refs")]
    public InventoryManager inventoryManager;///< Inventory ref
    public UITextController uiTextController;///< UI ref
    public PlayerController playerController;///< Player ref
    public Stats stats;                      ///< Player stats

    /**
     * @brief Get refs.
     */
    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        inventoryManager = playerController.inventoryManager;
        uiTextController = playerController.textController;
        stats = FindObjectOfType<Stats>();
    }

    /**
     * @brief Show prompt on hover.
     */
    public void OnHoverEnter(UITextController textController)
    {
        if (!isHeld)
            textController.ShowInteraction(textController.gameTexts.collectMessage);
    }

    /**
     * @brief Clear prompt on exit.
     */
    public void OnHoverExit()
    {
        uiTextController.ClearMessages();
    }

    /**
     * @brief Collect item. Needs mobile for others.
     */
    public void Interact(GameObject objectOnHand = null)
    {
        if (isHeld) return;

        // Need mobile to pick up other items
        if (!inventoryManager.HasItem("Mobile") && itemData.itemID != "Mobile")
        {
            uiTextController.ShowThought(uiTextController.gameTexts.needMobileMessage);
            return;
        }

        // Special case for Mobile
        if (itemData.name == "Mobile")
        {
            stats.hasPhone = true;
            AudioSource audioSource = gameObject.GetComponent<AudioSource>();
            if (audioSource != null) audioSource.Stop();
        }

        PickUp();
    }

    /**
     * @brief Add to inventory and handle visuals.
     */
    void PickUp()
    {
        inventoryManager.AddItem(itemData);

        if (itemData.itemID == "Mobile")
        {
            isHeld = true;
            transform.SetParent(playerController.leftHand);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
        else
        {
            Destroy(gameObject);
        }

        uiTextController.ClearMessages();
    }
}
