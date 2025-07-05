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
    UITextController uiTextController; ///< UI message controller.
    GameTexts gameTexts; ///< Game texts.

    public List<Item> items = new List<Item>(); ///< List of items in the inventory.
    List<GameObject> slots = new List<GameObject>(); ///< Instantiated UI slots.
    int toyPieces = 0; ///< Current number of pieces in the inventory.

    public GameObject equippedRight; ///< Object equipped in the right hand.
    public bool isInventoryOpen = false; ///< Inventory state (open/closed).

    public AudioSource soundPlayer;
    public AudioClip inventorySoundClip; ///< Sound played when interacting with the inventory.
    Stats stats; ///< Reference to player stats.

    // Object Inspection
    public GameObject inspectMenu;
    public Image itemDisplay;

    /**
     * @brief Assigns scene references at the start.
     */
    void Start()
    {
        soundPlayer = GetComponent<AudioSource>();
        stats = FindAnyObjectByType<Stats>();
        gameTexts = FindAnyObjectByType<GameTexts>();

        // Hide inventory at start.
        inventoryUI.SetActive(false);

        // Hide inspection menu.
        inspectMenu.gameObject.SetActive(false);
        Button btn = itemDisplay.GetComponent<Button>();
        btn.onClick.AddListener(HideInspectMenu);
    }

    void Update()
    {
        UpdateUI();
    }

    /**
     * @brief Adds an item to the inventory and handles special logic (pieces, rewards, photos).
     * @param item The item to add.
     */
    public void AddItem(Item item)
    {
        soundPlayer.PlayOneShot(inventorySoundClip);
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
                stats.sanity = Mathf.Min(stats.sanity + 100, 100);
                uiTextController.ShowThought(gameTexts.rewardCollected);
            }
            if (item.type == "photo")
            {
                stats.sanity = Mathf.Min(stats.sanity + 50, 100);
                uiTextController.ShowThought(gameTexts.photoCollected);
            }

            items.Add(item);
            uiTextController.ShowInventoryMessage($"{item.displayName} " + gameTexts.objectAdded, true);
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
            uiTextController.ShowInventoryMessage($"{item.displayName} " + gameTexts.objectRemoved, false);
            soundPlayer.PlayOneShot(inventorySoundClip);
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
    }

    /**
     * @brief Updates the inventory UI.
     */
    public void UpdateUI()
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
                if (item.type == "photo")
                {
                    button.onClick.AddListener(() => ShowInspectMenu(item.icon));
                }
                else
                {
                    button.onClick.AddListener(() => EquipItem(item));
                }
            }

            slots.Add(slot);
        }
    }

    /**
     * @brief Unequips the object from the hand.
     */
    public void UnequipItem()
    {
        if (equippedRight != null)
        {
            Destroy(equippedRight);
            equippedRight = null;
        }
    }

    /**
     * @brief Equips an item in the hand or shows a message if it's a piece, photo, or reward.
     * @param item The item to equip.
     */
    void EquipItem(Item item)
    {
        if (equippedRight != null && equippedRight.GetComponent<ItemInteractable>().itemData == item)
        {
            UnequipItem();
            ToggleInventory();
        }
        else
        {
            if (item.type == "piece")
            {
                uiTextController.ShowThought(gameTexts.needPieces);
            }
            else if (item.type == "photo")
            {
                ShowInspectMenu(item.icon);
            }
            else
            {
                UnequipItem();
                equippedRight = Instantiate(item.itemPrefab, rightHand);
                equippedRight.transform.localPosition = item.equipPositionOffset;
                equippedRight.transform.localRotation = Quaternion.Euler(item.equipRotationOffset);
                equippedRight.GetComponent<ItemInteractable>().isHeld = true;
                ToggleInventory();
            }
        }
    }

    /**
     * @brief Shows the item's icon in the inspection menu.
     * @param photoSprite The sprite to display.
     */
    public void ShowInspectMenu(Sprite photoSprite)
    {
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

}
