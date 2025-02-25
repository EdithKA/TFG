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

    public bool isInventoryOpen = false;

    public Transform leftHand;
    public Transform rightHand;

    public string itemOnHand;

    private void Start()
    {
        if (inventoryUI != null)
        {
            inventoryUI.SetActive(false); 
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && HasItem("Mokia"))
        {
            ToggleInventory();
        }
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
            Debug.Log("Inventory is full");
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

    private void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        inventoryUI.SetActive(isInventoryOpen); // Mostrar/Ocultar la UI del inventario

        if (isInventoryOpen)
        {
            Debug.Log("Inventario abierto.");
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Debug.Log("Inventario cerrado.");
        }
    }

    private void EquipItem(Item item)
    {
        Debug.Log(item.name);
        if(itemOnHand == item.name)
        {
            foreach (Transform child in rightHand)
            {
                Destroy(child.gameObject); //Quitamos de la mano derecha si hay algun objeto previo
                itemOnHand = "";
            }
        }
        
        else if (item.itemName != "Mokia")
        {
            GameObject itemSelected = Instantiate(item.itemPrefab, rightHand);
            itemSelected.transform.localPosition = Vector3.zero; 
            itemSelected.transform.localRotation = Quaternion.identity;
            itemSelected.GetComponent<Object>().isHeld = true;
            itemOnHand = item.itemName;
        }
    }
}




