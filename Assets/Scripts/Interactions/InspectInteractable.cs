using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @brief Lets you inspect objects. Shows a thought when used.
 */
public class InspectInteractable : MonoBehaviour, IInteractable
{
    public string thought;                     ///< Thought to show on inspect
    GameTexts gameTexts;                       ///< UI texts
    UITextController uiTextController;         ///< UI ref

    /**
     * @brief Get UI refs.
     */
    void Start()
    {
        uiTextController = FindObjectOfType<UITextController>();
        gameTexts = uiTextController.gameTexts;
    }

    /**
     * @brief Show thought when inspected.
     */
    public void Interact(GameObject objectOnHand = null)
    {
        uiTextController.ShowThought(thought);
    }

    /**
     * @brief Show prompt on hover.
     */
    public void OnHoverEnter(UITextController textController)
    {
        textController.ShowInteraction(gameTexts.inspectMessage);
    }

    /**
     * @brief Clear prompt on exit.
     */
    public void OnHoverExit()
    {
        uiTextController.ClearMessages();
    }
}
