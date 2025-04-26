using System.Collections;
using System.Collections.Generic;
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
    public GameObject itemPrefab;   /// 3D prefab for in-world representation
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
    public UITextController instructions; /// Interaction prompt controller

    [Header("Inventory System")]
    public InventoryManager inventory; /// Core inventory management reference

    private PlayerController playerMove; /// Player controller for animation updates

    /**
     * @brief Initializes required components and references
     */
    private void Start()
    {
        playerMove = FindAnyObjectByType<PlayerController>();
        instructions = GetComponent<UITextController>();
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

            // Clear interaction prompts
            Destroy(instructions.instructionText);
            Destroy(instructions);

            PickUp();
        }

        if (isHeld)
        {
            Destroy(instructions); // Remove prompts after pickup
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

        // Special handling for equippable items
        if (itemData.itemName == "Mokia")
        {
            isHeld = true;
            // Parent item to player's hand
            transform.position = PlayerHand.position;
            transform.rotation = PlayerHand.rotation;
            transform.SetParent(PlayerHand);
        }
        else
        {
            Destroy(gameObject); // Remove consumable items immediately
        }
    }
}
