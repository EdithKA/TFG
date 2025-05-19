using UnityEngine;

public class ItemInteractable : MonoBehaviour, IInteractable
{
    [Header("Configuración")]
    public Item itemData;
    public bool isHeld = false;

    [Header("Referencias")]
    private InventoryManager inventoryManager;
    private UITextController uiTextController;
    private PlayerController playerController;

    private void Start()
    {
        inventoryManager = FindObjectOfType<InventoryManager>();
        uiTextController = FindObjectOfType<UITextController>();
        playerController = FindObjectOfType<PlayerController>();
    }

    // Implementación de IInteractable
    public void OnHoverEnter(UITextController textController)
    {
        if (!isHeld)
        {
            textController.ShowInteraction(textController.gameTexts.collectMessage, Color.yellow);
        }
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
        uiTextController.ShowThought($"He encontrado {itemData.displayName}");

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
    }
}
