using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * @brief Manages the player's inventory, including UI updates, item storage, and equipment logic.
 *        Designed for a survival horror game where the player can pick up, equip, and combine items.
 */
public class InventoryManager : MonoBehaviour
{
    [Header("Inventory Settings")]
    public int inventorySize = 20;                       /// Maximum number of items the player can carry.
    public GameObject inventorySlotPrefab;                /// Prefab for each inventory slot in the UI.
    public Transform inventoryGrid;                       /// Parent transform for inventory slots in the UI.
    public GameObject inventoryUI;                        /// Main inventory UI panel.

    [Header("Hand References")]
    public Transform leftHand;                            /// Transform for equipping items in the left hand.
    public Transform rightHand;                           /// Transform for equipping items in the right hand.

    [Header("Special Items")]
    public Item completedToy;                             /// The combined toy item (when all pieces are collected).

    private List<Item> items = new List<Item>();          /// List of items currently in the inventory.
    private List<GameObject> slots = new List<GameObject>(); /// List of instantiated inventory slot UI objects.
    private int toyPieces = 0;                            /// Counter for collected toy pieces.

    private string itemOnHand;                            /// Name of the currently equipped item.
    private bool isInventoryOpen = false;                 /// Is the inventory currently open?
    public bool IsInventoryOpen => isInventoryOpen;       /// Public getter for inventory state.

    private PlayerController playerController;            /// Reference to the PlayerController script.

    /**
     * @brief Initializes inventory state and hides the UI at the start.
     */
    private void Start()
    {
        playerController = FindAnyObjectByType<PlayerController>();
        if (inventoryUI != null)
            inventoryUI.SetActive(false); // Hide inventory UI at the start.

        RefreshUI();
    }

    /**
     * @brief Adds an item to the inventory, handles special cases, and updates the UI.
     * @param item The item to add.
     */
    public void AddItem(Item item)
    {
        if (items.Count < inventorySize)
        {
            // Handle toy piece combination logic
            if (item.type == "piece")
            {
                toyPieces++;
                if (toyPieces == 2)
                {
                    items.RemoveAll(i => i.type == "piece");
                    toyPieces = 0;
                    AddItem(completedToy);
                    return; // Prevent double UI refresh
                }
            }

            items.Add(item);
            RefreshUI();
        }
    }

    /**
     * @brief Removes an item from the inventory and updates the UI.
     * @param item The item to remove.
     */
    public void RemoveItem(Item item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
            RefreshUI();
        }
    }

    /**
     * @brief Checks if the inventory contains an item with the given name.
     * @param itemName The name of the item to check.
     * @return True if the item is found, false otherwise.
     */
    public bool HasItem(string itemName)
    {
        foreach (Item item in items)
        {
            if (item.itemName == itemName)
                return true;
        }
        return false;
    }

    /**
     * @brief Toggles the inventory UI and manages cursor state.
     */
    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        inventoryUI.SetActive(isInventoryOpen);

        Cursor.lockState = isInventoryOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isInventoryOpen;
    }

    /**
     * @brief Refreshes the inventory UI to match the current items.
     */
    public void RefreshUI()
    {
        // Remove all existing slots from the UI.
        foreach (GameObject slot in slots)
        {
            Destroy(slot);
        }
        slots.Clear();

        // Create a new slot for each item in the inventory.
        foreach (Item item in items)
        {
            Item currentItem = item; // Prevent closure issue in delegates
            GameObject slot = Instantiate(inventorySlotPrefab, inventoryGrid);
            Image icon = slot.transform.Find("Icon").GetComponent<Image>();
            icon.sprite = currentItem.icon;
            slots.Add(slot);

            Button button = slot.GetComponent<Button>();
            button.onClick.AddListener(() => EquipItem(currentItem));
        }
    }

    /**
     * @brief Equips the selected item in the player's right hand, or unequips it if already equipped.
     * @param item The item to equip or unequip.
     */
    private void EquipItem(Item item)
    {
        if (itemOnHand == item.itemName)
        {
            StartCoroutine(DestroyEquippedItem());
            itemOnHand = "";
        }
        else if (item.itemName != "Mobile")
        {
            // Remove any previously equipped item.
            if (!string.IsNullOrEmpty(itemOnHand))
            {
                foreach (Transform child in rightHand)
                {
                    Destroy(child.gameObject);
                }
            }

            // Equip the new item
            GameObject itemSelected = Instantiate(item.itemPrefab, rightHand);
            itemSelected.transform.localPosition = Vector3.zero;
            itemSelected.transform.localRotation = Quaternion.identity;
            itemSelected.GetComponent<ItemController>().isHeld = true;
            itemOnHand = item.itemName;
        }
    }

    /**
     * @brief Coroutine that destroys the equipped object after a delay.
     */
    IEnumerator DestroyEquippedItem()
    {
        yield return new WaitForSeconds(10f);

        foreach (Transform child in rightHand)
        {
            Destroy(child.gameObject);
        }
    }

    /**
     * @brief Returns the GameObject currently equipped in the right hand, if any.
     * @return The equipped object or null.
     */
    public GameObject GetObjectOnHand()
    {
        if (rightHand != null && rightHand.childCount > 0)
        {
            return rightHand.GetChild(0).gameObject;
        }
        return null;
    }
}
