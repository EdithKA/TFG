using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Gestiona el inventario, la UI y la lógica de objetos especiales (a piezas).
public class InventoryManager : MonoBehaviour
{
    [Header("Inventory Settings")]
    public int inventorySize = 20; // Tamaño máximo del inventario.
    public GameObject inventorySlotPrefab; // Prefab para cada slot de la UI.
    public Transform inventoryGrid; // Grid donde se instancian los slots.
    public GameObject inventoryUI; // Panel de la UI del inventario.

    [Header("Hand References")]
    public Transform leftHand; // Mano izquierda (para el teléfono).
    public Transform rightHand; // Mano derecha (para el resto de objetos equipables).

    [Header("Special Items")]
    public Item completedToy; // Juguete completo (se obtiene al tener todas las piezas).

    [Header("UI Reference")]
    UITextController uiTextController; // Controlador de mensajes de la UI.
    GameTexts gameTexts; // Textos del juego.

    public List<Item> items = new List<Item>(); // Lista de objetos en el inventario.
    List<GameObject> slots = new List<GameObject>(); // Slots instanciados en la UI.
    int toyPieces = 0; // Contador de piezas actuales en el inventario.

    public GameObject equippedRight; // Objeto equipado en la mano derecha.
    public bool isInventoryOpen = false; // Estado del inventario (abierto/cerrado).


    public AudioSource soundPlayer;
    public AudioClip inventorySoundClip; // Sonido al interactuar con el inventario.
    Stats stats; // Referencia a las estadísticas del jugador.

    // Object Inspection
    public GameObject inspectMenu;
    public Image itemDisplay;

    private void Start()
    {
        // Asignamos las referencias de la escena.
        soundPlayer = GetComponent<AudioSource>();
        stats = FindAnyObjectByType<Stats>();
        gameTexts = FindAnyObjectByType<GameTexts>();

        // Inventario oculto al inicio.
        inventoryUI.SetActive(false);

        // Ocultamos el menu de inspección
        inspectMenu.gameObject.SetActive(false);
        Button btn = itemDisplay.GetComponent<Button>();
        btn.onClick.AddListener(HideInspectMenu);
       

    }

    private void Update()
    {
        UpdateUI();
    }

    // Añade un objeto al inventario y gestiona la lógica especial (piezas, recompensas, fotos).
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

    // Elimina un objeto del inventario
    public void RemoveItem(Item item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
            uiTextController.ShowInventoryMessage($"{item.displayName} " + gameTexts.objectRemoved, false);
            soundPlayer.PlayOneShot(inventorySoundClip);
        }
    }

    // Comprueba si el inventario contiene un objeto con un determinado ID.
    public bool HasItem(string itemName) => items.Exists(item => item.itemID == itemName);

    // Abre o cierra el inventario y gestiona la visibilidad y el bloqueo del cursor.
    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        inventoryUI.SetActive(isInventoryOpen);
        Cursor.lockState = isInventoryOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isInventoryOpen;
    }

    // Actualiza la UI del inventario.
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

    // Desequipa el objeto de la mano.
    public void UnequipItem()
    {
        if (equippedRight != null)
        {
            Destroy(equippedRight);
            equippedRight = null;
        }
    }

    // Equipa un objeto de la mano o muestra mensaje si es pieza, foto o reward.
    private void EquipItem(Item item)
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

    // Muestra en el menú de inspección el icono del objeto.
    public void ShowInspectMenu(Sprite photoSprite)
    {
        itemDisplay.sprite = photoSprite;
        inspectMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
    }

    // Oculta el menú de inspección.
    public void HideInspectMenu()
    {
       
        inspectMenu.SetActive(false);
        if (!isInventoryOpen)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
       
    }

    // Devuelve el objeto equipado en la mano derecha.
    public GameObject GetRightHandObject() => equippedRight;

}
