using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{

    [Header("Inventory Settings")]
    public int inventorySize = 20;
    public GameObject inventorySlotPrefab; //Prefab de las casillas del inventario
    public Transform inventoryGrid; //Grid donde se colocan las casillas
    public GameObject inventoryUI; //Canvas que contiene el inventario

    public List<Item> items = new List<Item>();
    private List<GameObject> slots = new List<GameObject>(); //Slots en la UI


    public Transform leftHand;
    public Transform rightHand;

    public string itemOnHand;

    Camera playerCam;
    public bool IsInventoryOpen { get; private set; }

    PlayerMove playerMove;

    private void Start()
    {
        playerMove = FindAnyObjectByType<PlayerMove>();
        playerCam = FindObjectOfType<Camera>();
        if (inventoryUI != null)
        {
            inventoryUI.SetActive(false); 
        }
    }

    private void Update()
    {
       
    }


    public void AddItem(Item item)
    {
        if (items.Count < inventorySize)
        {
            items.Add(item);
            UpdateInventoryUI(); //Actualizamos la UI tras añadir objeto
        }
        else
        {
            //Debug.Log("Inventory is full");
        }
    }

    public void RemoveItem(Item item)
    {
        if(items.Contains(item))
        {
            items.Remove(item);
            UpdateInventoryUI(); //Actualizar la UI cuando eliminamos un objeto
        }
    }

    public bool HasItem(string itemName)
    {
        foreach (Item item in items)
        {
            if (item.itemName == itemName)
                return true;
        }
        return false;
    }

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

    private void EquipItem(Item item)
    {
        // Check if this is the currently equipped item
        if (itemOnHand == item.name)
        {
            // User is unequipping the current item
            foreach (Transform child in rightHand)
            {
                Destroy(child.gameObject); // Remove the object from hand
                itemOnHand = "";
            }

            
        }
        else if (item.itemName != "Mokia")
        {
            // User is equipping a new item
            GameObject itemSelected = Instantiate(item.itemPrefab, rightHand);
            itemSelected.transform.localPosition = Vector3.zero;
            itemSelected.transform.localRotation = Quaternion.identity;
            itemSelected.GetComponent<Object>().isHeld = true;
            itemOnHand = item.itemName;

          
        }
    }

}




