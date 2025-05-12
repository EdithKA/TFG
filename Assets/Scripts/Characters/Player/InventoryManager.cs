using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [Header("Inventory Settings")]
    public int inventorySize = 20;
    public GameObject inventorySlotPrefab;
    public Transform inventoryGrid;
    public GameObject inventoryUI;

    [Header("Hand References")]
    public Transform leftHand;    // Para el móvil
    public Transform rightHand;   // Para otros objetos

    [Header("Special Items")]
    public Item completedToy;

    private List<Item> items = new List<Item>();
    private List<GameObject> slots = new List<GameObject>();
    private int toyPieces = 0;

    private GameObject equippedLeft;   // Objeto en mano izquierda (solo "Mobile")
    private GameObject equippedRight;  // Objeto en mano derecha
    private bool isInventoryOpen = false;
    public bool IsInventoryOpen => isInventoryOpen;

    private void Start()
    {
        if (inventoryUI != null) inventoryUI.SetActive(false);
        RefreshUI();
    }

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

            // Equipar automáticamente el móvil
            if (item.itemName == "Mobile") EquipItem(item);
        }
    }

    public void RemoveItem(Item item)
    {
        if (items.Contains(item)) items.Remove(item);
        RefreshUI();
    }

    public bool HasItem(string itemName) => items.Exists(item => item.itemName == itemName);

    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        inventoryUI.SetActive(isInventoryOpen);
        Cursor.lockState = isInventoryOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isInventoryOpen;
    }

    public void RefreshUI()
    {
        foreach (GameObject slot in slots) Destroy(slot);
        slots.Clear();

        foreach (Item item in items)
        {
            GameObject slot = Instantiate(inventorySlotPrefab, inventoryGrid);
            Image icon = slot.transform.Find("Icon").GetComponent<Image>();
            icon.sprite = item.icon;

            Button button = slot.GetComponent<Button>();
            button.onClick.AddListener(() => EquipItem(item));

            slots.Add(slot);
        }
    }

    private void EquipItem(Item item)
    {
        if (item.itemName == "Mobile") ToggleLeftHandItem(item);
        else ToggleRightHandItem(item);
    }

    private void ToggleLeftHandItem(Item item)
    {
        if (equippedLeft != null)
        {
            Destroy(equippedLeft);
            equippedLeft = null;
        }
        else
        {
            equippedLeft = Instantiate(item.itemPrefab, leftHand);
            equippedLeft.transform.localPosition = Vector3.zero;
            equippedLeft.transform.localRotation = Quaternion.identity;
            equippedLeft.GetComponent<ItemController>().isHeld = true;
        }
    }

    private void ToggleRightHandItem(Item item)
    {
        // Si ya está equipado, lo desequipa
        if (equippedRight != null && equippedRight.GetComponent<ItemController>().itemData == item)
        {
            Destroy(equippedRight);
            equippedRight = null;
        }
        else // Si no está equipado, lo equipa
        {
            if (equippedRight != null) Destroy(equippedRight);
            equippedRight = Instantiate(item.itemPrefab, rightHand);
            equippedRight.transform.localPosition = Vector3.zero;
            equippedRight.transform.localRotation = Quaternion.identity;
            equippedRight.GetComponent<ItemController>().isHeld = true;
        }
    }

    public GameObject GetObjectOnHand()
    {
        // Prioriza la mano derecha, si no hay nada devuelve la izquierda
        return equippedRight != null ? equippedRight : equippedLeft;
    }
}
