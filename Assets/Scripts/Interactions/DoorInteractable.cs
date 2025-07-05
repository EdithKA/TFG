using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; // Added for NavMeshObstacle

/**
 * @brief Controls door interaction, requiring a specific item to open/close.
 * Implements the IInteractable interface.
 */
public class DoorInteractable : MonoBehaviour, IInteractable
{
    [Header("Animation Settings")]
    /**
     * @brief Reference to the door's Animator component.
     */
    public Animator leftDoorAnim;
    public Animator rightDoorAnim;

    /**
     * @brief Name of the animation parameter that controls door state.
     */
    private string openParameter = "Open";

    /**
     * @brief Current state of the door (open/closed).
     */
    public bool isOpen = false;

    [Header("UI References")]
    /**
     * @brief Game text configurations for UI messages.
     */
    public GameTexts gameTexts;
    public UITextController textController;

    [Header("Required Item")]
    /**
     * @brief Item ID required to interact with the door.
     */
    public string requiredItem;

    BoxCollider interactCollider;

    /**
     * @brief Initializes the door animator reference.
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
     * @brief Displays interaction prompt when hovering over the door.
     * @param textController UI text controller reference.
     */
    public void OnHoverEnter(UITextController textController)
    {
        textController.ShowInteraction(gameTexts.interactMessage, Color.cyan);
    }

    /**
     * @brief Clears interaction prompt when hovering ends.
     */
    public void OnHoverExit()
    {
        textController.ClearMessages();
    }

    /**
     * @brief Handles door interaction, checking for required item and toggling door state.
     * @param objectOnHand Item currently held by the player.
     */
    public void Interact(GameObject objectOnHand = null)
    {
        InventoryManager inventory = FindObjectOfType<InventoryManager>();

        // Check if player has required item
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

        // Toggle door state
        isOpen = !isOpen;
    }
}
