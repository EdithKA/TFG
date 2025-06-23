using UnityEngine;

/// <summary>
/// Controls door interaction, requiring a specific item to open/close.
/// Implements the IInteractable interface.
/// </summary>
public class DoorInteractable : MonoBehaviour, IInteractable
{
    [Header("Animation Settings")]
    /// <summary>
    /// Reference to the door's Animator component.
    /// </summary>
    private Animator doorAnimator;

    /// <summary>
    /// Name of the animation parameter that controls door state.
    /// </summary>
    private string openParameter = "Open";

    /// <summary>
    /// Current state of the door (open/closed).
    /// </summary>
    private bool isOpen = false;

    [Header("UI References")]
    /// <summary>
    /// Game text configurations for UI messages.
    /// </summary>
    public GameTexts gameTexts;

    [Header("Required Item")]
    /// <summary>
    /// Item ID required to interact with the door.
    /// </summary>
    public string requiredItem;

    /// <summary>
    /// Initializes the door animator reference.
    /// </summary>
    private void Start()
    {
        doorAnimator = GetComponent<Animator>();
    }

    /// <summary>
    /// Displays interaction prompt when hovering over the door.
    /// </summary>
    /// <param name="textController">UI text controller reference.</param>
    public void OnHoverEnter(UITextController textController)
    {
        textController.ShowInteraction(gameTexts.interactMessage, Color.cyan);
    }

    /// <summary>
    /// Clears interaction prompt when hovering ends.
    /// </summary>
    public void OnHoverExit()
    {
        FindObjectOfType<UITextController>().ClearMessages();
    }

    /// <summary>
    /// Handles door interaction, checking for required item and toggling door state.
    /// </summary>
    /// <param name="objectOnHand">Item currently held by the player.</param>
    public void Interact(GameObject objectOnHand = null)
    {
        InventoryManager inventory = FindObjectOfType<InventoryManager>();

        // Check if player has required item
        if (!inventory.HasItem(requiredItem))
        {
            FindObjectOfType<UITextController>().ShowThought("Parece que necesito algo...");
            return;
        }

        // Toggle door state
        isOpen = !isOpen;
        doorAnimator.SetBool(openParameter, isOpen);

        FindObjectOfType<UITextController>().ShowThought(
            isOpen ? gameTexts.interactMessage : gameTexts.interactMessage
        );
    }
}
