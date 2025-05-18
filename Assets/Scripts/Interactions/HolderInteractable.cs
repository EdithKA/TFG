using UnityEngine;

public class HolderInteractable : MonoBehaviour, IInteractable
{
    [Header("Configuración")]
    public string correctObjectID;
    public bool completed;
    public Transform holderPoint;
    public GameTexts gameTexts;

    [Header("Referencias")]
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
        if (!completed)
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
        // Si ya está completado, no se puede recoger ni colocar nada
        if (completed)
        {
            uiTextController.ShowThought(gameTexts.placedCorrectlyMessage);
            return;
        }
        

        // Si hay un objeto en el holder (y no está completado), puedes recogerlo
        if (itemOnHolder != null)
        {
            // Recoger solo si NO está completado
            inventoryManager.AddItem(itemOnHolder.itemData);
            Destroy(itemOnHolder.gameObject);
            itemOnHolder = null;
            uiTextController.ShowThought(gameTexts.collectedMessage);
            uiTextController.ClearMessages();
        }
        // Si no hay objeto en el holder, puedes colocar uno si tienes en la mano
        else if (objectOnHand != null)
        {
            ItemInteractable item = objectOnHand.GetComponent<ItemInteractable>();
            if (item != null)
            {
                // Instanciar el objeto en el holder
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
                    item.GetComponent<Collider>().enabled = false; 
                    uiTextController.ShowThought(gameTexts.placedCorrectlyMessage);
                    completed = true;
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
