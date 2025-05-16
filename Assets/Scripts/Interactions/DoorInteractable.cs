using UnityEngine;

public class DoorInteractable : MonoBehaviour, IInteractable
{
    [Header("Animation Settings")]
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private string openParameter = "Open";

    [Header("UI References")]
    public GameTexts gameTexts;

    private bool isOpen = false;

    public void OnHoverEnter(UITextController textController)
    {
        textController.ShowInteraction(gameTexts.interactMessage, Color.cyan);
    }

    public void OnHoverExit()
    {
        FindObjectOfType<UITextController>().ClearMessages();
    }

    public void Interact(GameObject objectOnHand = null)
    {
        if (doorAnimator == null) return;

        isOpen = !isOpen;
        doorAnimator.SetBool(openParameter, isOpen);

        FindObjectOfType<UITextController>().ShowThought(
            isOpen ? gameTexts.interactMessage : gameTexts.interactMessage
        );
    }
}
