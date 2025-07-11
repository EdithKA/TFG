using UnityEngine;

/**
 * @brief Interface for objects the player can interact with.
 */
public interface IInteractable
{
    /**
     * @brief Called when the player hovers over the object.
     * @param textController UI text controller for prompts.
     */
    void OnHoverEnter(UITextController textController);

    /**
     * @brief Called when the player stops hovering.
     */
    void OnHoverExit();

    /**
     * @brief Called when the player interacts with the object.
     * @param objectOnHand Item the player holds (optional).
     */
    void Interact(GameObject objectOnHand = null);
}
