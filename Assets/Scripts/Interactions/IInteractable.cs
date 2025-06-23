using UnityEngine;

/// <summary>
/// Defines the standard interface for objects that can be interacted with by the player.
/// </summary>
public interface IInteractable
{
    /// <summary>
    /// Called when the player hovers over the interactable object.
    /// </summary>
    /// <param name="textController">The UI text controller to display interaction prompts.</param>
    void OnHoverEnter(UITextController textController);

    /// <summary>
    /// Called when the player stops hovering over the interactable object.
    /// </summary>
    void OnHoverExit();

    /// <summary>
    /// Called when the player interacts with the object.
    /// </summary>
    /// <param name="objectOnHand">Optional reference to the item currently held by the player.</param>
    void Interact(GameObject objectOnHand = null);
}
