using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/**
 * @brief Manages the player's inventory, including UI updates and item equipping.
 */
public class InventoryManager : MonoBehaviour
{
    [Header("Inventory Settings")]
    public int inventorySize = 20; /// Maximum number of items in the inventory.
    public GameObject inventorySlotPrefab; /// Prefab for inventory slot UI elements.
    public Transform inventoryGrid; /// Parent transform for the inventory slots.
    public GameObject inventoryUI; /// Main inventory UI canvas.


    public List<Item> items = new List<Item>(); /// List of items currently in the inventory.
    private List<GameObject> slots = new List<GameObject>(); /// List of instantiated inventory slot UI objects.

    public Transform leftHand; /// Transform for equipping items in the left hand.
    public Transform rightHand; /// Transform for equipping items in the right hand.

    public string itemOnHand; /// Name of the currently equipped item.

    private Camera playerCam; /// Reference to the player's camera.
    public bool IsInventoryOpen { get; private set; } /// Indicates if the inventory UI is open.

    private PlayerController playerMove; /// Reference to the PlayerController script.

    private int toyPieces;

    public Item completedToy;

    /**
     * @brief Initializes references and hides the inventory UI at the start.
     */
    private void Start()
    {
        toyPieces = 0;
        playerMove = FindAnyObjectByType<PlayerController>();
        playerCam = FindObjectOfType<Camera>();
        if (inventoryUI != null)
        {
            inventoryUI.SetActive(false); /// Hide inventory UI at the start.
        }
    }

    private void Update()
    {
        UpdateInventoryUI();
    }

    /**
     * @brief Adds an item to the inventory if there is space and updates the UI.
     * @param item The item to add.
     */
    public void AddItem(Item item)
    {
        if (items.Count < inventorySize)
        {
            items.Add(item);
            if(item.type == "piece")
            {
                toyPieces += 1;
                if(toyPieces == 2)
                {
                    items.RemoveAll(i => i.type == "piece");
                    toyPieces = 0;
                    AddItem(completedToy);
                }

                

            }
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
     * @brief Updates the inventory UI to match the current list of items.
     */
    private void UpdateInventoryUI()
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
            GameObject slot = Instantiate(inventorySlotPrefab, inventoryGrid);
            Image icon = slot.transform.Find("Icon").GetComponent<Image>();
            icon.sprite = item.icon;
            slots.Add(slot);

            Button button = slot.GetComponent<Button>();
            button.onClick.AddListener(() => EquipItem(item));
        }
    }

    /**
     * @brief Toggles the inventory UI on or off and manages the cursor state.
     */
    public void ToggleInventory()
    {
        IsInventoryOpen = !IsInventoryOpen;
        inventoryUI.SetActive(IsInventoryOpen);
        Debug.Log("Inventory opened");
        if (IsInventoryOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    /**
     * @brief Equips the selected item in the player's right hand, or unequips it if already equipped.
     * @param item The item to equip or unequip.
     */
    private void EquipItem(Item item)
    {
        if (itemOnHand == item.name)
        {
            StartCoroutine(DestroyObect());
            itemOnHand = "";
            playerMove.RActive = false;
        }
        else if (item.itemName != "Mobile")
        {
            // Remove any previously equipped item.
            if (itemOnHand != "")
            {
                foreach (Transform child in rightHand)
                {
                    Destroy(child.gameObject);
                }
            }

            GameObject itemSelected = Instantiate(item.itemPrefab, rightHand);
            itemSelected.transform.localPosition = Vector3.zero;
            itemSelected.transform.localRotation = Quaternion.identity;
            itemSelected.GetComponent<ItemController>().isHeld = true;
            itemOnHand = item.name;

            playerMove.RActive = true;
        }
    }

    /**
     * @brief Coroutine that destroys the equipped object after a delay.
     */
    IEnumerator DestroyObect()
    {
        yield return new WaitForSeconds(10f);

        foreach (Transform child in rightHand)
        {
            Destroy(child.gameObject);
        }
    }

    public GameObject GetObjectOnHand()
    {
        if (rightHand != null && rightHand.childCount > 0)
        {
            return rightHand.GetChild(0).gameObject;
        }
        return null;
    }
}
