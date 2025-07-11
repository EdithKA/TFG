using System;
using UnityEngine;

/**
 * @brief Holder for puzzle objects. Lets you place and check items.
 */
public class HolderInteractable : MonoBehaviour, IInteractable, IPuzleObjective
{
    public bool isComplete { get; private set; } ///< Is the objective done
    public string correctObjectID;               ///< Needed object ID
    public Transform holderPoint;                ///< Where to place the object

    public GameTexts gameTexts;                  ///< UI messages

    InventoryManager inventoryManager;           ///< Inventory ref
    UITextController uiTextController;           ///< UI ref

    ItemInteractable itemOnHolder;               ///< Current object

    /**
     * @brief Get refs.
     */
    void Start()
    {
        uiTextController = FindObjectOfType<UITextController>();
        inventoryManager = FindObjectOfType<InventoryManager>();
    }

    /**
     * @brief Show prompt if not done.
     */
    public void OnHoverEnter(UITextController textController)
    {
        if (!isComplete)
            textController.ShowInteraction(gameTexts.placeObjectMessage);
    }

    /**
     * @brief Clear UI on exit.
     */
    public void OnHoverExit()
    {
        uiTextController.ClearMessages();
    }

    /**
     * @brief Place or collect object.
     */
    public void Interact(GameObject objectOnHand = null)
    {
        if (isComplete)
        {
            uiTextController.ShowThought(gameTexts.placedCorrectlyMessage);
            return;
        }

        // Take back object if already placed
        if (itemOnHolder != null)
        {
            inventoryManager.AddItem(itemOnHolder.itemData);
            Destroy(itemOnHolder.gameObject);
            itemOnHolder = null;
            uiTextController.ShowThought(gameTexts.collectedMessage);
            uiTextController.ClearMessages();
        }
        // Place object from hand
        else if (objectOnHand != null)
        {
            ItemInteractable item = objectOnHand.GetComponent<ItemInteractable>();
            if (item != null)
            {
                inventoryManager.RemoveItem(item.itemData);
                GameObject newItem = Instantiate(item.itemData.itemPrefab, holderPoint.position, holderPoint.rotation, holderPoint);
                itemOnHolder = newItem.GetComponentInChildren<ItemInteractable>(true);
                inventoryManager.RemoveItem(item.itemData);
                Destroy(objectOnHand);

                if (item.itemData.itemID == correctObjectID)
                {
                    itemOnHolder.enabled = false;
                    Collider collider = itemOnHolder.GetComponent<Collider>();
                    if (collider != null) collider.enabled = false;

                    uiTextController.ShowThought(gameTexts.placedCorrectlyMessage);
                    isComplete = true;
                }
                else
                {
                    uiTextController.ShowThought(gameTexts.wrongObjectMessage);
                }
                uiTextController.ClearMessages();
            }
        }
        // Nothing to interact with
        else
        {
            uiTextController.ShowInteraction(gameTexts.needObjectMessage, Color.red);
        }
    }
}
