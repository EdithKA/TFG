using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages player inventory system including item storage, UI representation, 
/// and equipment handling. Supports item combination, mobile phone auto-equipping,
/// and dual-hand item management (static left hand for mobile, dynamic right hand).
/// </summary>
public class InventoryManager : MonoBehaviour
{
    [Header("Inventory Settings")]
    public int inventorySize = 20;                // Maximum carry capacity
    public GameObject inventorySlotPrefab;        // UI slot template
    public Transform inventoryGrid;               // Parent for inventory slots
    public GameObject inventoryUI;                // Main inventory panel

    [Header("Hand References")]
    public Transform leftHand;                    // Permanent mobile phone hand
    public Transform rightHand;                   // Equippable item hand

    [Header("Special Items")]
    public Item completedToy;                     // Combined toy item

    [Header("UI Reference")]
    [SerializeField] private UITextController uiTextController; // Asignar desde el Inspector

    private List<Item> items = new List<Item>();  // Inventory contents
    private List<GameObject> slots = new List<GameObject>(); // UI slot instances
    private int toyPieces = 0;                    // Collected toy fragments

    public GameObject equippedRight;              // Currently held right-hand item
    private bool isInventoryOpen = false;         // Inventory visibility state
    public bool IsInventoryOpen => isInventoryOpen; // Public accessor

    /// <summary>
    /// Initializes inventory UI and ensures mobile phone is equipped if present
    /// </summary>
    private void Start()
    {
        if (inventoryUI != null) inventoryUI.SetActive(false);
        RefreshUI();
    }

    /// <summary>
    /// Adds item to inventory and handles special cases:
    /// - Toy piece combination logic
    /// - Mobile phone auto-equipping
    /// </summary>
    public void AddItem(Item item)
    {
        if (items.Count < inventorySize)
        {
            if (item.type == "piece")
            {
                toyPieces++;
                if (toyPieces == 2)
                {
                    items.RemoveAll(i => i.type == "piece");
                    toyPieces = 0;
                    AddItem(completedToy);
                    return;
                }
            }

            items.Add(item);
            RefreshUI();
            uiTextController.ShowInventoryMessage($"{item.displayName} añadido al inventario.", true);
        }
    }

    /// <summary>
    /// Checks if mobile phone is currently equipped
    /// </summary>
    private bool IsMobileEquipped() => leftHand.childCount > 0;

    /// <summary>
    /// Public interface for mobile phone check
    /// </summary>
    public bool HasMobileEquipped() => IsMobileEquipped();

    /// <summary>
    /// Removes specified item from inventory
    /// </summary>
    public void RemoveItem(Item item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
            RefreshUI();
            uiTextController.ShowInventoryMessage($"{item.displayName} eliminado del inventario.", false);
        }
    }

    /// <summary>
    /// Checks if inventory contains item by name
    /// </summary>
    public bool HasItem(string itemName) => items.Exists(item => item.itemID == itemName);

    /// <summary>
    /// Toggles inventory visibility and cursor state
    /// </summary>
    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        inventoryUI.SetActive(isInventoryOpen);
        Cursor.lockState = isInventoryOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isInventoryOpen;
    }

    /// <summary>
    /// Rebuilds inventory UI from current items
    /// </summary>
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
                button.onClick.AddListener(() => EquipRightHandItem(item));
            }

            slots.Add(slot);
        }
    }

    /// <summary>
    /// Removes current right-hand item
    /// </summary>
    public void UnequipRightHandItem()
    {
        if (equippedRight != null)
        {
            Destroy(equippedRight);
            equippedRight = null;
        }
    }

    /// <summary>
    /// Handles right-hand item equipment logic:
    /// - Toggles item if same type is equipped
    /// - Replaces previous item if different
    /// </summary>
    private void EquipRightHandItem(Item item)
    {
        if (equippedRight != null && equippedRight.GetComponent<ItemInteractable>().itemData == item)
        {
            UnequipRightHandItem();
        }
        else
        {
            UnequipRightHandItem();
            equippedRight = Instantiate(item.itemPrefab, rightHand);
            equippedRight.transform.localPosition = item.equipPositionOffset;
            equippedRight.transform.localRotation = Quaternion.Euler(item.equipRotationOffset);
            equippedRight.GetComponent<ItemInteractable>().isHeld = true;
        }
    }

    /// <summary>
    /// Returns currently equipped right-hand item
    /// </summary>
    public GameObject GetRightHandObject() => equippedRight;
}
