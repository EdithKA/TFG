using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/**
 * @brief Defines inventory items as ScriptableObjects for easy asset creation
 */
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    [Header("Item Configuration")]
    public string itemName;         /// Name displayed in inventory UI
    public Sprite icon;             /// 2D representation in inventory slots
    public string type;
    public GameObject itemPrefab;   /// 3D prefab for in-world representation
    public string collectedText;
 
}

/**
 * @brief Handles interactable object behavior and inventory integration
 */
public class ItemController : MonoBehaviour
{
    [Header("Player References")]
    public Transform PlayerHand;    /// Transform for item positioning when held

    [Header("Item Data")]
    public Item itemData;           /// ScriptableObject containing item properties

    [Header("State Management")]
    bool isPlayerInTrigger = false; /// Player proximity detection flag
    public bool isHeld = false;     /// Item equipment status

    [Header("UI Components")]
    public UITextController UIText; /// Interaction prompt controller

    [Header("Inventory System")]
    public InventoryManager inventory; /// Core inventory management reference

    private PlayerController playerMove; /// Player controller for animation updates

    /**
     * @brief Initializes required components and references
     */
    private void Start()
    {
        playerMove = FindAnyObjectByType<PlayerController>();
        UIText = FindAnyObjectByType<UITextController>();
        inventory = FindAnyObjectByType<InventoryManager>();
    }

    /**
     * @brief Handles player interaction input and state changes
     */
    private void Update()
    {
        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.E) && !isHeld)
        {
            // Special case for phone equipment
            if (itemData.name == "Mokia")
            {
                playerMove.LActive = true; // Activate left hand animation
            }

            

            PickUp();
        }

        
    }

    /**
     * @brief Handles trigger enter event for player proximity
     */
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = true;

            
            UIText.ShowMessage(UIMessageType.Collect, null);
            
            
        }
    }

    /**
     * @brief Handles trigger exit event for player proximity
     */
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
        }
    }

    /**
     * @brief Main item pickup logic and inventory management
     */
    void PickUp()
    {
        Debug.Log($"Acquired item: {itemData.itemName}");
        inventory.AddItem(itemData);

        // Mostrar mensaje de recogida (sin borrar inmediatamente)
        UIText.ShowMessage(UIMessageType.Collected, itemData.collectedText);

        if (itemData.itemName == "Mokia")
        {
            isHeld = true;
            transform.SetParent(PlayerHand);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
        else
        {
            Destroy(gameObject);
        }
    }

}
