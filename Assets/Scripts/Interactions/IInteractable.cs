using UnityEngine;

public interface IInteractable
{
    void OnHoverEnter(UITextController textController);
    void OnHoverExit();
    void Interact(GameObject objectOnHand = null);
}
