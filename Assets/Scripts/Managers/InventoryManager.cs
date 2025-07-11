using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * @brief Manages the inventory, UI, and special item logic (such as pieces).
 */
public class InventoryManager : MonoBehaviour
{
    [Header("Inventory Settings")]
    public int inventorySize = 20; ///< Maximum inventory size.
    public GameObject inventorySlotPrefab; ///< Prefab for each UI slot.
    public Transform inventoryGrid; ///< Grid where slots are instantiated.
    public GameObject inventoryUI; ///< Inventory UI panel.

    [Header("Hand References")]
    public Transform leftHand; ///< Left hand (for the phone).
    public Transform rightHand; ///< Right hand (for other equipable items).

    [Header("Special Items")]
    public Item completedToy; ///< Complete toy (obtained when collecting all pieces).

    [Header("UI Reference")]
    [SerializeField] private UITextController uiTextController; ///< UI message controller.
    public GameObject inspectMenu; ///< Panel for inspecting items.
    public Image itemDisplay; ///< Image for inspected item.
    public Button btn; ///< Button to close inspect menu.
    public AudioSource soundPlayer;
    public AudioClip inventorySoundClip; ///< Sound played when interacting with the inventory.

    public List<Item> items = new List<Item>(); ///< List of items in the inventory.
    private List<GameObject> slots = new List<GameObject>(); ///< Instantiated UI slots.
    private int toyPieces = 0; ///< Current number of pieces in the inventory.

    public GameObject equippedRight; ///< Object equipped in the right hand.
    public bool isInventoryOpen = false; ///< Inventory state (open/closed).

    Stats stats; ///< Reference to player stats.
    GameTexts gameTexts; ///< Game texts.

    /**
     * @brief Assigns scene references at the start.
     */
    void Start()
    {
        soundPlayer = GetComponent<AudioSource>();
        stats = FindAnyObjectByType<Stats>();
        if (uiTextController == null)
            uiTextController = FindAnyObjectByType<UITextController>();
        if (uiTextController != null)
            gameTexts = uiTextController.gameTexts;

        inventoryUI.SetActive(false);
        inspectMenu.SetActive(false);
        btn.onClick.AddListener(HideInspectMenu);
        RefreshUI();
    }

    /**
     * @brief Adds an item to the inventory and handles special logic (pieces, rewards, photos).
     * @param item The item to add.
     */
    public void AddItem(Item item)
    {
        soundPlayer?.PlayOneShot(inventorySoundClip);
        if (items.Count < inventorySize)
        {
            if (item.type == "piece")
            {
                toyPieces++;
                if (toyPieces == 5)
                {
                    items.RemoveAll(i => i.type == "piece");
                    toyPieces = 0;
                    AddItem(completedToy);
                    return;
                }
            }
            if (item.type == "reward")
            {
                if (stats != null)
                    stats.sanity = Mathf.Min(stats.sanity + 100, 100);
                uiTextController?.ShowThought(gameTexts?.rewardCollected);
            }
            if (item.type == "photo")
            {
                if (stats != null)
                    stats.sanity = Mathf.Min(stats.sanity + 50, 100);
                uiTextController?.ShowThought(gameTexts?.photoCollected);
            }

            items.Add(item);
            RefreshUI();
            uiTextController?.ShowInventoryMessage($"{item.displayName} " + (gameTexts?.objectAdded), true);
        }
    }

    /**
     * @brief Removes an item from the inventory.
     * @param item The item to remove.
     */
    public void RemoveItem(Item item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
            RefreshUI();
            uiTextController?.ShowInventoryMessage($"{item.displayName} " + (gameTexts?.objectRemoved), false);
            soundPlayer?.PlayOneShot(inventorySoundClip);
        }
    }

    /**
     * @brief Checks if the inventory contains an item with a given ID.
     * @param itemName The ID of the item.
     * @return True if the item is in the inventory, false otherwise.
     */
    public bool HasItem(string itemName) => items.Exists(item => item.itemID == itemName);

    /**
     * @brief Opens or closes the inventory and manages cursor visibility and locking.
     */
    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        inventoryUI.SetActive(isInventoryOpen);
        Cursor.lockState = isInventoryOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isInventoryOpen;
        if (isInventoryOpen)
            RefreshUI();
    }

    /**
     * @brief Rebuilds inventory UI from current items.
     */
    public void RefreshUI()
    {
        foreach (GameObject slot in slots) Destroy(slot);
        slots.Clear();

        foreach (Item item in items)
        {
            GameObject slot = Instantiate(inventorySlotPrefab, inventoryGrid);
            Image icon = slot.transform.Find("Icon").GetComponent<Image>();
            icon.sprite = item.icon;

            if (item.itemID != "Mobile")
            {
                Button button = slot.GetComponent<Button>();
                button.onClick.RemoveAllListeners();
                Item capturedItem = item;
                if (item.type == "photo")
                {
                    button.onClick.AddListener(() => ShowInspectMenu(capturedItem.icon));
                }
                else
                {
                    button.onClick.AddListener(() =>
                    {
                        EquipRightHandItem(capturedItem);
                        CloseInventory();
                    });
                }
            }
            slots.Add(slot);
        }
    }

    /**
     * @brief Unequips the object from the hand.
     */
    public void UnequipRightHandItem()
    {
        if (equippedRight != null)
        {
            Destroy(equippedRight);
            equippedRight = null;
            CloseInventory();
        }
    }

    /**
     * @brief Equips an item in the hand or shows a message if it's a piece, photo, or reward.
     * @param item The item to equip.
     */
    private void EquipRightHandItem(Item item)
    {
        if (equippedRight != null && equippedRight.GetComponent<ItemInteractable>().itemData == item)
        {
            UnequipRightHandItem();
        }
        else
        {
            UnequipRightHandItem();
            if (item.itemPrefab != null)
            {
                equippedRight = Instantiate(item.itemPrefab, rightHand);
                equippedRight.transform.localPosition = item.equipPositionOffset;
                equippedRight.transform.localRotation = Quaternion.Euler(item.equipRotationOffset);
                var interactable = equippedRight.GetComponent<ItemInteractable>();
                if (interactable != null)
                    interactable.isHeld = true;
            }
        }
    }

    /**
     * @brief Closes the inventory and manages cursor state.
     */
    private void CloseInventory()
    {
        isInventoryOpen = false;
        inventoryUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /**
     * @brief Shows the item's icon in the inspection menu.
     * @param photoSprite The sprite to display.
     */
    public void ShowInspectMenu(Sprite photoSprite)
    {
        Debug.Log("ShowInspectMenu llamado");
        itemDisplay.sprite = photoSprite;
        inspectMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /**
     * @brief Hides the inspection menu.
     */
    public void HideInspectMenu()
    {
        if (inspectMenu != null)
            inspectMenu.SetActive(false);
        if (!isInventoryOpen)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    /**
     * @brief Returns the object equipped in the right hand.
     * @return The equipped GameObject.
     */
    public GameObject GetRightHandObject() => equippedRight;

    /**
     * @brief Returns a list of item IDs currently in the inventory.
     */
    public List<string> GetInventoryItemIDs()
    {
        List<string> ids = new List<string>();
        foreach (Item item in items)
            ids.Add(item.itemID);
        return ids;
    }

    /**
     * @brief Restores inventory from a list of item IDs.
     */
    public void RestoreInventory(List<string> itemIDs)
    {
        items.Clear();
        foreach (string id in itemIDs)
        {
            Item item = FindItemByID(id);
            if (item != null)
                items.Add(item);
        }
        RefreshUI();
    }

    /**
     * @brief Finds an item by its ID from resources.
     */
    private Item FindItemByID(string id)
    {
        foreach (Item item in Resources.LoadAll<Item>("Items"))
            if (item.itemID == id)
                return item;
        return null;
    }
}
