using UnityEngine;

public class ItemInteractable : MonoBehaviour, IInteractable
{
    [Header("Configuración")]
    public Item itemData;
    public bool isHeld = false;

    [Header("Referencias")]
    public InventoryManager inventoryManager;
    public UITextController uiTextController;
    public PlayerController playerController;

    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        inventoryManager = playerController.inventoryManager;
        uiTextController = playerController.textController;
    }

    public void OnHoverEnter(UITextController textController)
    {
        if (!isHeld)
            textController.ShowInteraction(textController.gameTexts.collectMessage);
    }

    public void OnHoverExit()
    {
        uiTextController.ClearMessages();
    }

    public void Interact(GameObject objectOnHand = null)
    {
        if (isHeld) return;

        if (!inventoryManager.HasItem("Mobile") && itemData.itemID != "Mobile")
        {
            uiTextController.ShowThought(uiTextController.gameTexts.needMobileMessage);
            return;
        }

        PickUp();
    }

    private void PickUp()
    {
        inventoryManager.AddItem(itemData);

        if (itemData.itemID == "Mobile")
        {
            isHeld = true;
            transform.SetParent(playerController.leftHand);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
        else
        {
            Destroy(gameObject);
        }

        uiTextController.ClearMessages();
    }
}
