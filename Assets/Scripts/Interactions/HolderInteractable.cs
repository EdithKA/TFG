using UnityEngine;

public class HolderInteractable : MonoBehaviour, IInteractable
{
    [Header("Configuración")]
    public string correctObjectName;
    public Transform holderPoint;
    public GameTexts gameTexts;

    [Header("Referencias")]
    private InventoryManager inventoryManager;
    private UITextController uiTextController;
    private ItemController itemOnHolder;

    private void Start()
    {
        inventoryManager = FindObjectOfType<InventoryManager>();
        uiTextController = FindObjectOfType<UITextController>();
        gameTexts = uiTextController.gameTexts;
        holderPoint = holderPoint = transform.GetChild(0);

    }

    public void Interact(GameObject objectOnHand = null)
    {
        if (itemOnHolder != null)
        {
            if (objectOnHand == null)
            {
                inventoryManager.AddItem(itemOnHolder.itemData);
                Destroy(itemOnHolder.gameObject);
                itemOnHolder = null;
                uiTextController.ShowThought(gameTexts.collectedMessage);

                // Limpia el mensaje de interacción al recoger
                uiTextController.ClearMessages();
            }
            else
            {
                uiTextController.ShowInteraction(gameTexts.wrongObjectMessage, Color.red);
            }
        }
        else
        {
            if (objectOnHand != null)
            {
                ItemController item = objectOnHand.GetComponent<ItemController>();
                if (item != null)
                {
                    itemOnHolder = Instantiate(item.itemData.itemPrefab, holderPoint.position, holderPoint.rotation, holderPoint).GetComponent<ItemController>();
                    inventoryManager.RemoveItem(item.itemData);
                    Destroy(objectOnHand);

                    bool isCorrect = item.itemData.itemName.Equals(correctObjectName, System.StringComparison.OrdinalIgnoreCase);

                    if (isCorrect)
                    {
                        uiTextController.ShowThought(gameTexts.placedCorrectlyMessage);
                        
                    }
                    else
                    {
                        uiTextController.ShowThought(gameTexts.wrongObjectMessage);
                    }

                    // Limpia el mensaje de interacción al colocar
                    uiTextController.ClearMessages();
                }
            }
            else
            {
                uiTextController.ShowInteraction(gameTexts.needObjectMessage, Color.red);
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            uiTextController.ShowInteraction(itemOnHolder ? gameTexts.collectMessage : gameTexts.interactMessage, Color.cyan);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) uiTextController.ClearMessages();
    }
}
