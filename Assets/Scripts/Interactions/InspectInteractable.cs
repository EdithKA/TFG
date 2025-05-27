using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InspectInteractable : MonoBehaviour, IInteractable
{
    public string thought;
    private UITextController uiTextController;

    private void Start()
    {
        uiTextController = FindObjectOfType<UITextController>();

    }
    public void Interact(GameObject objectOnHand = null)
    {
        uiTextController.ShowThought(thought);
    }

    public void OnHoverEnter(UITextController textController)
    {
        textController.ShowInteraction("Pulsa [E] para examinar.", Color.cyan);
    }

    public void OnHoverExit()
    {
        uiTextController.ClearMessages();
    }


}
