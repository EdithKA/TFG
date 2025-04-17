using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    /**
    * @Brief Inventory configuration.
    */

    [Header("Inventory Settings")]
    public int inventorySize = 20;
    public GameObject inventorySlotPrefab; /// Prefab of the inventory boxes.
    public Transform inventoryGrid; /// Grid where the boxes are placed.
    public GameObject inventoryUI; /// Canvas containing the inventory.
    public List<Item> items = new List<Item>(); /// Objects in the inventory.
    private List<GameObject> slots = new List<GameObject>(); /// Slots in the UI.

    /**
     * @brief Coordinates where the objects assigned to the hands are placed.
    */

    public Transform leftHand; /// Anchor point of the left hand.
    public Transform rightHand; /// Anchor point of the right hand.


    public string itemOnHand; ///Active object in the right hand.

    public bool IsInventoryOpen { get; private set; } /// Inventory status.

    PlayerController playerController; /// Player controller.

    /**
     * @brief When the game appears the inventory is hidden.
    */
    private void Start()
    {
        playerController = FindAnyObjectByType<PlayerController>();
        if (inventoryUI != null)
        {
            inventoryUI.SetActive(false); 
        }
    }

    /**
     * @brief It is used to add an object to the inventory.
    */
    public void AddItem(Item item)
    {
        if (items.Count < inventorySize)
        {
            items.Add(item);
            UpdateInventoryUI(); /// We update the UI after adding object.
        }
    }

    /**
     * @brief Eliminates an object from inventory.
     */
    public void RemoveItem(Item item)
    {
        if(items.Contains(item))
        {
            items.Remove(item);
            UpdateInventoryUI(); /// Update the UI when we eliminate an object.
        }
    }

    /**
     * @brief It serves to check if we have an object in the inventory.
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
     *@brief Upper the UI depending on the state of the inventory.
     */
    private void UpdateInventoryUI()
    {
        foreach (GameObject slot in slots)
        {
            Destroy(slot);
        }
        slots.Clear();

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
     * @brief It is used to enable or disable inventory.
     */
    public void ToggleInventory()
    {
        IsInventoryOpen = !IsInventoryOpen;
        inventoryUI.SetActive(IsInventoryOpen);
        Debug.Log("Inventario abierto");
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
     * @brief It is used to equip or unbalance an object in hand.
     */
    private void EquipItem(Item item)
    {
        //Debug.Log(item.name);
        if(itemOnHand == item.name)
        {
            foreach (Transform child in rightHand)
            {
                Destroy(child.gameObject); /// We lift from the right hand if there is any previous object.
                itemOnHand = "";
            }
        }
        
        else if (item.itemName != "Mokia") /// If the object collected is the phone, it is equipped on the left.
        {
            GameObject itemSelected = Instantiate(item.itemPrefab, rightHand);
            itemSelected.transform.localPosition = Vector3.zero; 
            itemSelected.transform.localRotation = Quaternion.identity;
            itemSelected.GetComponent<Object>().isHeld = true;
            itemOnHand = item.itemName;
            playerController.RightHandOn = !playerController.RightHandOn;
        }

    }
}




