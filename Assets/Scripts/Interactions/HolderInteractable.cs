using System;
using UnityEngine;

public class HolderInteractable : MonoBehaviour, IInteractable, IPuzzleObjective
{
    public bool isComplete { get; private set; }

    [Header("Configuration")]
    public string correctObjectID;
    public Transform holderPoint;
    public GameTexts gameTexts;

    [Header("References")]
    private InventoryManager inventoryManager;
    private UITextController uiTextController;
    private ItemInteractable itemOnHolder;

    private void Start()
    {
        uiTextController = FindObjectOfType<UITextController>();
        inventoryManager = FindObjectOfType<InventoryManager>();
    }

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

    public void OnHoverExit()
    {
        uiTextController.ClearMessages();
    }

    public void Interact(GameObject objectOnHand = null)
    {
        if (isComplete)
        {
            uiTextController.ShowThought(gameTexts.placedCorrectlyMessage);
            return;
        }

        if (itemOnHolder != null)
        {
            inventoryManager.AddItem(itemOnHolder.itemData);
            Destroy(itemOnHolder.gameObject);
            itemOnHolder = null;
            uiTextController.ShowThought(gameTexts.collectedMessage);
            uiTextController.ClearMessages();
        }
        else if (objectOnHand != null)
        {
            ItemInteractable item = objectOnHand.GetComponent<ItemInteractable>();
            if (item != null)
            {
                inventoryManager.RemoveItem(item.itemData);
                GameObject newItem = Instantiate(item.itemData.itemPrefab, holderPoint.position, holderPoint.rotation, holderPoint);
                itemOnHolder = newItem.GetComponentInChildren<ItemInteractable>(true);

                if (itemOnHolder == null)
                {
                    Destroy(newItem);
                    return;
                }

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
        else
        {
            uiTextController.ShowInteraction(gameTexts.needObjectMessage, Color.red);
        }
    }
}
