using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides inspection functionality for examinable objects in the game world.
/// Displays custom thoughts when the 
/// interacts with the object.
/// </summary>
public class InspectInteractable : MonoBehaviour, IInteractable
{
    /// <summary>
    /// The thought message to display when the object is inspected.
    /// </summary>
    public string thought;

    /// <summary>
    /// Reference to the UI text controller for displaying messages.
    /// </summary>
    private UITextController uiTextController;

    /// <summary>
    /// Initializes the UI text controller reference.
    /// </summary>
    private void Start()
    {
        uiTextController = FindObjectOfType<UITextController>();
    }

    /// <summary>
    /// Handles the interaction when the the player examines the object.
    /// </summary>
    /// <param name="objectOnHand">Item currently held by the the player (unused).</param>
    public void Interact(GameObject objectOnHand = null)
    {
        uiTextController.ShowThought(thought);
    }

    /// <summary>
    /// Displays the interaction prompt when the the player hovers over the object.
    /// </summary>
    /// <param name="textController">UI text controller reference.</param>
    public void OnHoverEnter(UITextController textController)
    {
        textController.ShowInteraction("Pulsa [E] para examinar.", Color.cyan);
    }

    /// <summary>
    /// Clears the interaction prompt when the the player stops hovering.
    /// </summary>
    public void OnHoverExit()
    {
        uiTextController.ClearMessages();
    }
}
