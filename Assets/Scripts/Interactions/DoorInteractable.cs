using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/**
 * @brief Door that needs an item to open/close.
 */
public class DoorInteractable : MonoBehaviour, IInteractable
{
    [Header("Animation")]
    public Animator leftDoorAnim;   ///< Left door animator
    public Animator rightDoorAnim;  ///< Right door animator
    private string openParameter = "Open"; ///< Animator param
    public bool isOpen = false;     ///< Is the door open

    [Header("UI")]
    public GameTexts gameTexts;         ///< UI texts
    public UITextController textController; ///< UI controller

    [Header("Item")]
    public string requiredItem;     ///< Needed item

    BoxCollider interactCollider;   ///< Collider for interaction

    /**
     * @brief Setup refs.
     */
    void Start()
    {
        interactCollider = GetComponent<BoxCollider>();
        textController = FindObjectOfType<UITextController>();
    }

    void Update()
    {
        leftDoorAnim.SetBool(openParameter, isOpen);
        rightDoorAnim.SetBool(openParameter, isOpen);
    }

    /**
     * @brief Show prompt on hover.
     */
    public void OnHoverEnter(UITextController textController)
    {
        textController.ShowInteraction(gameTexts.interactMessage, Color.cyan);
    }

    /**
     * @brief Clear prompt on exit.
     */
    public void OnHoverExit()
    {
        textController.ClearMessages();
    }

    /**
     * @brief Try to open/close. Needs item.
     */
    public void Interact(GameObject objectOnHand = null)
    {
        InventoryManager inventory = FindObjectOfType<InventoryManager>();

        if (!inventory.HasItem(requiredItem))
        {
            textController.ShowThought(gameTexts.needSomething);
            return;
        }
        else
        {
            if (!isOpen)
                textController.ShowThought(gameTexts.canOpen);
            else
                textController.ShowThought(gameTexts.closeDoor);
        }

        isOpen = !isOpen;
    }
}
