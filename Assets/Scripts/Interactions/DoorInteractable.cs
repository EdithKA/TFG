using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; // Añadido para NavMeshObstacle

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
    public Animator leftDoorAnim;
    public Animator rightDoorAnim;

    /// <summary>
    /// Name of the animation parameter that controls door state.
    /// </summary>
    private string openParameter = "Open";

    /// <summary>
    /// Current state of the door (open/closed).
    /// </summary>
    public bool isOpen = false;

    [Header("UI References")]
    /// <summary>
    /// Game text configurations for UI messages.
    /// </summary>
    public GameTexts gameTexts;
    public UITextController textController;

    [Header("Required Item")]
    /// <summary>
    /// Item ID required to interact with the door.
    /// </summary>
    public string requiredItem;

    BoxCollider interactCollider;

    /// <summary>
    /// Initializes the door animator reference.
    /// </summary>
    private void Start()
    {
        interactCollider = GetComponent<BoxCollider>();
        textController = FindObjectOfType<UITextController>();

       
    }

    private void Update()
    {
        leftDoorAnim.SetBool(openParameter, isOpen);
        rightDoorAnim.SetBool(openParameter, isOpen);
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
        textController.ClearMessages();
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
            textController.ShowThought("Looks like I need something...");
            return;
        }
        else
        {
            if (!isOpen)
                textController.ShowThought("I can finally open it.");
            else
                textController.ShowThought("Let's keep it closed just in case.");
        }

        // Toggle door state
        isOpen = !isOpen;
    }
}
