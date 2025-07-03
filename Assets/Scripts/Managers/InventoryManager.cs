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
    public Transform leftHand;
    public Transform rightHand;

    [Header("Special Items")]
    public Item completedToy;

    [Header("UI Reference")]
    [SerializeField] private UITextController uiTextController;

    public List<Item> items = new List<Item>();
    private List<GameObject> slots = new List<GameObject>();
    private int toyPieces = 0;

    public GameObject equippedRight;
    private bool isInventoryOpen = false;
    public bool IsInventoryOpen => isInventoryOpen;

    public AudioSource soundPlayer;
    public AudioClip inventorySoundClip;
    Stats stats;

    [Header("Photo Inspection")]
    public GameObject inspectMenu;
    public Image photoDisplay;

    private void Start()
    {
        soundPlayer = GetComponent<AudioSource>();
        stats = FindAnyObjectByType<Stats>();
        if (inventoryUI != null) inventoryUI.SetActive(false);

        inspectMenu.gameObject.SetActive(false);
        Button btn = photoDisplay.GetComponent<Button>();
        btn.onClick.AddListener(HidePhotoInspect);
       

        RefreshUI();
    }

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
                uiTextController.ShowThought("It's like my childhood is protecting me. I feel whole again.");
            }



            if (item.type == "photo")
            {
                stats.sanity = Mathf.Min(stats.sanity + 50, 100);
                uiTextController.ShowThought("Holding this photo, I feel a bit stronger. Memories really can heal.");
            }

            items.Add(item);
            RefreshUI();
            uiTextController.ShowInventoryMessage($"{item.displayName} added to inventory.", true);
        }
    }

    private bool IsMobileEquipped() => leftHand.childCount > 0;
    public bool HasMobileEquipped() => IsMobileEquipped();

    public void RemoveItem(Item item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
            RefreshUI();
            uiTextController.ShowInventoryMessage($"{item.displayName} removed from inventory.", false);
            soundPlayer.PlayOneShot(inventorySoundClip);
        }
    }

    public bool HasItem(string itemName) => items.Exists(item => item.itemID == itemName);

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

            if (item.itemID != "Mobile")
            {
                Button button = slot.GetComponent<Button>();
                if (item.type == "photo")
                {
                    button.onClick.AddListener(() => ShowPhotoInspect(item.icon));
                }
                else
                {
                    button.onClick.AddListener(() => EquipRightHandItem(item));
                }
            }

            slots.Add(slot);
        }
    }

    public void UnequipRightHandItem()
    {
        if (equippedRight != null)
        {
            Destroy(equippedRight);
            equippedRight = null;
        }
    }

    private void EquipRightHandItem(Item item)
    {
        if (equippedRight != null && equippedRight.GetComponent<ItemInteractable>().itemData == item)
        {
            UnequipRightHandItem();
            ToggleInventory();
        }
        else
        {
            if (item.type == "piece")
            {
                uiTextController.ShowThought("Looks like I need more parts.");
            }
            else if (item.type == "photo")
            {
                ShowPhotoInspect(item.icon);
            }
            else
            {
                UnequipRightHandItem();
                equippedRight = Instantiate(item.itemPrefab, rightHand);
                equippedRight.transform.localPosition = item.equipPositionOffset;
                equippedRight.transform.localRotation = Quaternion.Euler(item.equipRotationOffset);
                equippedRight.GetComponent<ItemInteractable>().isHeld = true;
                ToggleInventory();
            }
        }
    }

    /// <summary>
    /// Muestra la foto en el objeto Image de la UI
    /// </summary>
    public void ShowPhotoInspect(Sprite photoSprite)
    {
        photoDisplay.sprite = photoSprite;
        inspectMenu.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
    }

    /// <summary>
    /// Oculta la foto de la UI
    /// </summary>
    public void HidePhotoInspect()
    {
       
        inspectMenu.SetActive(false);
        Time.timeScale = 1f;
        if (!isInventoryOpen)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
       
    }

    public GameObject GetRightHandObject() => equippedRight;

    public List<string> GetInventoryItemIDs()
    {
        List<string> ids = new List<string>();
        foreach (Item item in items)
            ids.Add(item.itemID);
        return ids;
    }

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

    private Item FindItemByID(string id)
    {
        foreach (Item item in Resources.LoadAll<Item>("Items"))
            if (item.itemID == id)
                return item;
        return null;
    }
}
