using System;
using UnityEngine;

/**
 * @brief Allows assigning a holder as a puzzle objective and placing objects on it.
 */
public class HolderInteractable : MonoBehaviour, IInteractable, IPuzleObjective
{
    /**
     * @brief Indicates whether the holder's objective has been completed.
     */
    public bool isComplete { get; private set; }

    /**
     * @brief ID of the correct object that must be placed on the holder.
     */
    public string correctObjectID;

    /**
     * @brief Location where the placed object will be instantiated.
     */
    public Transform holderPoint;

    public GameTexts gameTexts;

    InventoryManager inventoryManager;
    UITextController uiTextController;

    /**
     * @brief Object currently placed on the holder.
     */
    ItemInteractable itemOnHolder;

    /**
     * @brief Initializes references to UI and inventory controllers.
     */
    void Start()
    {
        uiTextController = FindObjectOfType<UITextController>();
        inventoryManager = FindObjectOfType<InventoryManager>();
    }

    /**
     * @brief When hovering over the object, shows an interaction message if not completed.
     * @param textController Reference to the UI text controller.
     */
    public void OnHoverEnter(UITextController textController)
    {
        if (!isComplete)
        {
            textController.ShowInteraction(gameTexts.placeObjectMessage);
        }
    }

    /**
     * @brief Clears UI messages when the object is no longer being pointed at.
     */
    public void OnHoverExit()
    {
        uiTextController.ClearMessages();
    }

    /**
     * @brief Logic for interacting with the holder.
     * @param objectOnHand The object currently held by the player.
     */
    public void Interact(GameObject objectOnHand = null)
    {
        // If the correct object has been placed, show a message and exit.
        if (isComplete)
        {
            uiTextController.ShowThought(gameTexts.placedCorrectlyMessage);
            return;
        }

        // If there is an object on the holder, return it to the inventory and remove it from the holder.
        if (itemOnHolder != null)
        {
            inventoryManager.AddItem(itemOnHolder.itemData);
            Destroy(itemOnHolder.gameObject);
            itemOnHolder = null;
            uiTextController.ShowThought(gameTexts.collectedMessage);
            uiTextController.ClearMessages();
        }
        // If the player has an object in hand.
        else if (objectOnHand != null)
        {
            ItemInteractable item = objectOnHand.GetComponent<ItemInteractable>();
            if (item != null)
            {
                // Removes the object from the inventory and from the hand, and instantiates it on the holder.
                inventoryManager.RemoveItem(item.itemData);
                GameObject newItem = Instantiate(item.itemData.itemPrefab, holderPoint.position, holderPoint.rotation, holderPoint);
                itemOnHolder = newItem.GetComponentInChildren<ItemInteractable>(true);
                inventoryManager.RemoveItem(item.itemData);
                Destroy(objectOnHand);

                // If the object is the correct one, disables interaction and marks the objective as complete.
                if (item.itemData.itemID == correctObjectID)
                {
                    itemOnHolder.enabled = false;
                    Collider collider = itemOnHolder.GetComponent<Collider>();
                    if (collider != null) collider.enabled = false;

                    uiTextController.ShowThought(gameTexts.placedCorrectlyMessage);
                    isComplete = true;
                }
                // If it is not the correct one, shows an error message.
                else
                {
                    uiTextController.ShowThought(gameTexts.wrongObjectMessage);
                }
                uiTextController.ClearMessages();
            }
        }
        // If there is no object in hand or on the holder, shows an error message in red.
        else
        {
            uiTextController.ShowInteraction(gameTexts.needObjectMessage, Color.red);
        }
    }
}
