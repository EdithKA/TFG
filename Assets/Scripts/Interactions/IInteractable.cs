using UnityEngine;

/// <summary>
/// Interface for any object that can be interacted with by the player.
/// The optional parameter allows passing the object currently held by the player (if any).
/// </summary>
public interface IInteractable
{
    /// <summary>
    /// Defines the interaction logic for the object.
    /// </summary>
    /// <param name="objectOnHand">The object currently held by the player (can be null).</param>
    void Interact(GameObject objectOnHand = null);
}
