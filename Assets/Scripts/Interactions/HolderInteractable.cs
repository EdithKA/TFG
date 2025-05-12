using UnityEngine;

/**
 * @brief Allows the player to place or pick up objects on a holder (like a shelf or pedestal).
 *        Provides UI feedback and visual cues for correct placement.
 */
public class HolderInteractable : MonoBehaviour, IInteractable
{
    [Header("Holder Settings")]
    public string correctObjectName;           /// The name of the correct item for this holder.
    public ItemController itemOnHolder;        /// The item currently placed on the holder.
    public Transform holderPoint;              /// The exact position where the item will appear.
    public bool isCorrect = false;             /// True if the correct item is placed.

    [Header("References")]
    private InventoryManager inventoryManager; /// Reference to the player's inventory.
    private UITextController uiTextController; /// Reference to the UI text controller.
    private bool playerInRange;                /// Is the player close enough to interact?
    private Renderer objectRenderer;           /// Renderer for visual feedback.

    [Header("UI Messages")]
    private string interactionMessage = "Press [E] to place an object";
    private string pickupMessage = "Press [E] to pick up object";
    private string successMessage = "Object placed correctly!";
    private string errorMessage = "You need an object to place here";
    private string wrongObjectMessage = "There is already an object here";

    /**
     * @brief Initializes references and sets default values.
     */
    void Start()
    {
        inventoryManager = FindObjectOfType<InventoryManager>();
        uiTextController = FindObjectOfType<UITextController>();
        objectRenderer = GetComponent<Renderer>();

        if (holderPoint == null)
            holderPoint = transform;
    }

    /**
     * @brief Handles UI feedback, visual cues, and interaction input each frame.
     */
    void Update()
    {
        // Check if the correct object is on the holder
        isCorrect = itemOnHolder != null &&
                    itemOnHolder.itemData != null &&
                    itemOnHolder.itemData.itemName == correctObjectName;

        // Visual feedback: green if correct, white if not
        if (objectRenderer != null)
        {
            objectRenderer.material.color = isCorrect ? Color.green : Color.white;
        }

        // Handle player interaction input
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            GameObject objectInHand = inventoryManager?.GetObjectOnHand();
            Interact(objectInHand);

            // Update the UI message after interaction
            if (itemOnHolder == null)
            {
                uiTextController.ShowMessage(UIMessageType.Read, interactionMessage);
            }
            else
            {
                uiTextController.ShowMessage(UIMessageType.Read, pickupMessage);
            }
        }
    }

    /**
     * @brief Handles placing and picking up objects from the holder.
     * @param objectOnHand The object currently held by the player (can be null).
     */
    public void Interact(GameObject objectOnHand = null)
    {
        // If there is an object on the holder
        if (itemOnHolder != null)
        {
            // If the player is not holding anything, pick up the object
            if (objectOnHand == null)
            {
                Item itemData = itemOnHolder.itemData;

                // Add the item to the inventory and remove it from the holder
                inventoryManager.AddItem(itemData);
                Destroy(itemOnHolder.gameObject);
                itemOnHolder = null;

                uiTextController.ShowMessage(UIMessageType.Collected, "Object picked up");
            }
            else
            {
                uiTextController.ShowMessage(UIMessageType.Read, wrongObjectMessage);
            }
        }
        // If the holder is empty
        else
        {
            // If the player is holding an object, place it
            if (objectOnHand != null)
            {
                ItemController item = objectOnHand.GetComponent<ItemController>();
                if (item != null && item.itemData != null)
                {
                    Item itemData = item.itemData;

                    // Remove the item from inventory and destroy the one in hand
                    inventoryManager.RemoveItem(itemData);
                    Destroy(objectOnHand);

                    // Instantiate the item prefab on the holder
                    GameObject newItemObject = Instantiate(itemData.itemPrefab, holderPoint.position, holderPoint.rotation);
                    newItemObject.transform.SetParent(holderPoint);

                    ItemController newItem = newItemObject.GetComponent<ItemController>();
                    if (newItem != null)
                    {
                        newItem.isHeld = false;
                        newItem.itemData = itemData;
                        itemOnHolder = newItem;
                    }

                    uiTextController.ShowMessage(UIMessageType.Collected, "Object placed");

                    if (itemData.itemName == correctObjectName)
                        uiTextController.ShowMessage(UIMessageType.Read, successMessage);
                }
            }
            else
            {
                uiTextController.ShowMessage(UIMessageType.Collect, errorMessage);
            }
        }
    }

    /**
     * @brief Shows the correct UI message when the player enters interaction range.
     */
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (itemOnHolder == null)
                uiTextController.ShowMessage(UIMessageType.Interact, interactionMessage);
            else
                uiTextController.ShowMessage(UIMessageType.Interact, pickupMessage);
        }
    }

    /**
     * @brief Clears the UI message when the player leaves interaction range.
     */
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            uiTextController.ClearMessage();
        }
    }
}
