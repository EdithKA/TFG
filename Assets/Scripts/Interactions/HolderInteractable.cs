using UnityEngine;

public class HolderInteractable : MonoBehaviour, IInteractable
{
    [Header("Configuración")]
    public string correctObjectName;
    public bool completed;
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
        if (!completed)
        {

            if (itemOnHolder != null)
            {
                if (objectOnHand == null)
                {
                    inventoryManager.AddItem(itemOnHolder.itemData);
                    Destroy(itemOnHolder.gameObject);
                    itemOnHolder = null;
                    uiTextController.ShowThought(gameTexts.collectedMessage);
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
                        // Instancia el objeto y busca ItemController en hijos
                        GameObject newItem = Instantiate(item.itemData.itemPrefab, holderPoint.position, holderPoint.rotation, holderPoint);
                        itemOnHolder = newItem.GetComponentInChildren<ItemController>(true);

                        if (itemOnHolder == null)
                        {
                            Destroy(newItem);
                            return;
                        }

                        inventoryManager.RemoveItem(item.itemData);
                        Destroy(objectOnHand);


                        if (item.itemData.itemID == correctObjectName)
                        {
                            uiTextController.ShowThought(gameTexts.placedCorrectlyMessage);
                            completed = true;

                        }
                        else
                            uiTextController.ShowThought(gameTexts.wrongObjectMessage);

                        uiTextController.ClearMessages();
                    }
                }
                else
                {
                    uiTextController.ShowInteraction(gameTexts.needObjectMessage, Color.red);
                }
            }
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !completed)
        {
            uiTextController.ShowInteraction(itemOnHolder ? gameTexts.collectMessage : gameTexts.interactMessage, Color.cyan);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) uiTextController.ClearMessages();
    }
}
