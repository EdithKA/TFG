using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * @brief Manages inventory, UI, and special item logic.
 */
public class InventoryManager : MonoBehaviour
{
    [Header("Inventory Settings")]
    public int inventorySize = 20;                  ///< Max inventory size
    public GameObject inventorySlotPrefab;          ///< UI slot prefab
    public Transform inventoryGrid;                 ///< Slot grid parent
    public GameObject inventoryUI;                  ///< Inventory panel

    [Header("Hand References")]
    public Transform leftHand;                      ///< Left hand (phone)
    public Transform rightHand;                     ///< Right hand (equip)

    [Header("Special Items")]
    public Item completedToy;                       ///< Full toy (all pieces)

    [Header("UI Reference")]
    [SerializeField] private UITextController uiTextController; ///< UI controller
    public GameObject inspectMenu;                  ///< Inspect panel
    public Image itemDisplay;                       ///< Inspected item image
    public Button btn;                              ///< Close inspect button
    public AudioSource soundPlayer;                 ///< Audio source
    public AudioClip inventorySoundClip;            ///< Inventory sound

    public List<Item> items = new List<Item>();     ///< Inventory items
    private List<GameObject> slots = new List<GameObject>(); ///< UI slots
    private int toyPieces = 0;                      ///< Piece count

    public GameObject equippedRight;                ///< Equipped right-hand object
    public bool isInventoryOpen = false;            ///< Inventory open/closed

    Stats stats;                                   ///< Player stats
    GameTexts gameTexts;                           ///< UI texts

    /**
     * @brief Get refs and setup.
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
     * @brief Add item, handle pieces, rewards, photos.
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
     * @brief Remove item from inventory.
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
     * @brief Check if item exists by ID.
     */
    public bool HasItem(string itemName) => items.Exists(item => item.itemID == itemName);

    /**
     * @brief Open/close inventory, set cursor.
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
     * @brief Rebuild inventory UI.
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
     * @brief Unequip right-hand object.
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
     * @brief Equip item in right hand.
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
     * @brief Close inventory and lock cursor.
     */
    private void CloseInventory()
    {
        isInventoryOpen = false;
        inventoryUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /**
     * @brief Show inspect menu for item.
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
     * @brief Hide inspect menu.
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
     * @brief Get equipped right-hand object.
     */
    public GameObject GetRightHandObject() => equippedRight;

    /**
     * @brief Get list of item IDs in inventory.
     */
    public List<string> GetInventoryItemIDs()
    {
        List<string> ids = new List<string>();
        foreach (Item item in items)
            ids.Add(item.itemID);
        return ids;
    }

    /**
     * @brief Restore inventory from item IDs.
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
     * @brief Find item by ID (Resources).
     */
    private Item FindItemByID(string id)
    {
        foreach (Item item in Resources.LoadAll<Item>("Items"))
            if (item.itemID == id)
                return item;
        return null;
    }
}
