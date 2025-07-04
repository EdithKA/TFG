using System;
using UnityEngine;

// Permite asignar como objetivo de un puzzle un soporte y colocar objetos en el.
public class HolderInteractable : MonoBehaviour, IInteractable, IPuzzleObjective
{
    public bool isComplete { get; private set; } // Indica si el objetivo del holder ya se ha completado.

    
    public string correctObjectID; // ID del objeto correcto que debe de colocarse en el soporte.
    public Transform holderPoint; // Lugar donde se instanciará el objeto colocado.
    public GameTexts gameTexts; 

    InventoryManager inventoryManager;
    UITextController uiTextController;
    ItemInteractable itemOnHolder; // Objeto colocado en el soporte.

    private void Start()
    {
        uiTextController = FindObjectOfType<UITextController>();
        inventoryManager = FindObjectOfType<InventoryManager>();
    }

    // Al pasar el cursor sobre el objeto, muestra un mensaje de interacción si no está completo.
    public void OnHoverEnter(UITextController textController)
    {
        if (!isComplete)
        {
            textController.ShowInteraction(gameTexts.placeObjectMessage);
        }
    }

    // Limpia los mensajes de la UI al dejar de apuntar al objeto.
    public void OnHoverExit()
    {
        uiTextController.ClearMessages();
    }

    // Lógica de interacción con el soporte.
    public void Interact(GameObject objectOnHand = null)
    {
        // Si se ha colocado el objeto correcto, muestra mensaje y termina.
        if (isComplete)
        {
            uiTextController.ShowThought(gameTexts.placedCorrectlyMessage);
            return;
        }

        // Si hay un objeto en el soporte, lo devuelve al inventario y lo elimina del soporte.
        if (itemOnHolder != null)
        {
            inventoryManager.AddItem(itemOnHolder.itemData);
            Destroy(itemOnHolder.gameObject);
            itemOnHolder = null;
            uiTextController.ShowThought(gameTexts.collectedMessage);
            uiTextController.ClearMessages();
        }
        // Si el jugador tiene un objeto en la mano.
        else if (objectOnHand != null)
        {
            ItemInteractable item = objectOnHand.GetComponent<ItemInteractable>();
            if (item != null)
            {
                // Elimina el objeto del inventario y de la mano, y lo instancia en el soporte.
                inventoryManager.RemoveItem(item.itemData);
                GameObject newItem = Instantiate(item.itemData.itemPrefab, holderPoint.position, holderPoint.rotation, holderPoint);
                itemOnHolder = newItem.GetComponentInChildren<ItemInteractable>(true);
                inventoryManager.RemoveItem(item.itemData);
                Destroy(objectOnHand);

                // Si el objeto es el correcto, desactiva la interacción con este y marca el objetivo como completo.
                if (item.itemData.itemID == correctObjectID)
                {
                    itemOnHolder.enabled = false;
                    Collider collider = itemOnHolder.GetComponent<Collider>();
                    if (collider != null) collider.enabled = false;

                    uiTextController.ShowThought(gameTexts.placedCorrectlyMessage);
                    isComplete = true;
                }
                // Si no es el correcto, muestra un mensaje de error.
                else
                {
                    uiTextController.ShowThought(gameTexts.wrongObjectMessage);
                }
                uiTextController.ClearMessages();
            }
        }
        // Si no hay objeto en la mano ni en el soporte, muestra un mensaje de error en rojo.
        else
        {
            uiTextController.ShowInteraction(gameTexts.needObjectMessage, Color.red);
        }
    }
}
