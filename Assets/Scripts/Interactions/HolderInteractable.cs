using System;
using UnityEngine;

/// <summary>
/// Controls object placement puzzles where specific items must be placed on holders.
/// Implements both interactable and puzzle objective interfaces.
/// </summary>
public class HolderInteractable : MonoBehaviour, IInteractable, IPuzzleObjective
{
    /// <summary>
    /// Indicates if the puzzle has been completed.
    /// </summary>
    public bool isComplete { get; private set; }

    /// <summary>
    /// Event triggered when the puzzle is completed.
    /// </summary>
    public event Action onCompleted;

    [Header("Configuration")]
    /// <summary>
    /// Required item ID for puzzle completion.
    /// </summary>
    public string correctObjectID;

    /// <summary>
    /// Transform where placed objects will be positioned.
    /// </summary>
    public Transform holderPoint;

    /// <summary>
    /// Game text configurations for UI messages.
    /// </summary>
    public GameTexts gameTexts;

    [Header("References")]
    /// <summary>
    /// Inventory manager reference.
    /// </summary>
    private InventoryManager inventoryManager;

    /// <summary>
    /// UI text controller reference.
    /// </summary>
    private UITextController uiTextController;

    /// <summary>
    /// Currently placed item on the holder.
    /// </summary>
    private ItemInteractable itemOnHolder;

    /// <summary>
    /// Initializes necessary references.
    /// </summary>
    private void Start()
    {
        uiTextController = FindObjectOfType<UITextController>();
        inventoryManager = FindObjectOfType<InventoryManager>();
    }

    /// <summary>
    /// Displays interaction prompt based on puzzle state.
    /// </summary>
    /// <param name="textController">UI text controller reference.</param>
    public void OnHoverEnter(UITextController textController)
    {
        if (!isComplete)
        {
            textController.ShowInteraction("Pulsa [E] para colocar un objeto", Color.cyan);
        }
        else if (itemOnHolder != null)
        {
            textController.ShowInteraction("Objeto colocado correctamente", Color.green);
        }
    }

    /// <summary>
    /// Clears interaction prompt.
    /// </summary>
    public void OnHoverExit()
    {
        uiTextController.ClearMessages();
    }

    /// <summary>
    /// Handles object placement and puzzle verification.
    /// </summary>
    /// <param name="objectOnHand">Item currently held by the player.</param>
    public void Interact(GameObject objectOnHand = null)
    {
        if (isComplete)
        {
            uiTextController.ShowThought(gameTexts.placedCorrectlyMessage);
            return;
        }

        // Handle existing item removal
        if (itemOnHolder != null)
        {
            inventoryManager.AddItem(itemOnHolder.itemData);
            Destroy(itemOnHolder.gameObject);
            itemOnHolder = null;
            uiTextController.ShowThought(gameTexts.collectedMessage);
            uiTextController.ClearMessages();
        }
        // Handle new item placement
        else if (objectOnHand != null)
        {
            ItemInteractable item = objectOnHand.GetComponent<ItemInteractable>();
            if (item != null)
            {
                // Create new item instance on holder
                GameObject newItem = Instantiate(item.itemData.itemPrefab, holderPoint.position, holderPoint.rotation, holderPoint);
                itemOnHolder = newItem.GetComponentInChildren<ItemInteractable>(true);

                if (itemOnHolder == null)
                {
                    Destroy(newItem);
                    return;
                }

                // Remove original item from inventory
                inventoryManager.RemoveItem(item.itemData);
                Destroy(objectOnHand);

                // Verify if correct item was placed
                if (item.itemData.itemID == correctObjectID)
                {
                    // Disable interaction with placed item
                    itemOnHolder.enabled = false;
                    Collider collider = itemOnHolder.GetComponent<Collider>();
                    if (collider != null) collider.enabled = false;

                    // Complete puzzle
                    uiTextController.ShowThought(gameTexts.placedCorrectlyMessage);
                    isComplete = true;
                    onCompleted?.Invoke();
                }
                else
                {
                    uiTextController.ShowThought(gameTexts.wrongObjectMessage);
                }

                uiTextController.ClearMessages();
            }
        }
        else
        {
            uiTextController.ShowInteraction(gameTexts.needObjectMessage, Color.red);
        }
    }
}
