using System.Collections;
using UnityEngine;
using TMPro;

/**
 * @brief UI messages: interaction, thoughts, inventory.
 */
public enum UIMessageType { Interact, Collect, Collected, Read }

/**
 * @brief Controls UI text for interaction, thoughts, inventory.
 */
public class UITextController : MonoBehaviour
{
    [Header("Text Components")]
    public TextMeshProUGUI interactionText; ///< Interaction message
    public TextMeshProUGUI thoughtText;     ///< Thought message
    public TextMeshProUGUI inventoryText;   ///< Inventory message

    [Header("Game Texts")]
    public GameTexts gameTexts;             ///< UI texts

    [Header("Interaction Settings")]
    public Color interactColor = Color.cyan;///< Default interact color
    public Color collectColor = Color.yellow;///< Collect color
    public float interactionFadeDuration = 0.4f;
    public float interactionDisplayTime = 1.5f;

    [Header("Inventory Settings")]
    public Color addColor;                  ///< Add color
    public Color removeColor;               ///< Remove color
    public float inventoryTypingSpeed = 0.03f;
    public float inventoryDisplayTime = 1f;
    public float inventoryFadeDuration = 0.5f;

    [Header("Thought Settings")]
    public Color thoughtColor = Color.white;///< Thought color
    public float thoughtTypingSpeed = 0.03f;
    public float thoughtDisplayTime = 3f;
    public float thoughtFadeDuration = 0.5f;

    Coroutine interactionRoutine;
    Coroutine thoughtRoutine;
    Coroutine inventoryRoutine;

    /**
     * @brief Show interaction message (default color).
     */
    public void ShowInteraction(string message)
    {
        ShowInteraction(message, interactColor);
    }

    /**
     * @brief Show interaction message (custom color).
     */
    public void ShowInteraction(string message, Color color)
    {
        if (interactionRoutine != null) StopCoroutine(interactionRoutine);
        interactionRoutine = StartCoroutine(ShowInteractionRoutine(message, color));
    }

    /**
     * @brief Show collect message.
     */
    public void ShowCollect(string message)
    {
        ShowInteraction(message, collectColor);
    }

    IEnumerator ShowInteractionRoutine(string message, Color color)
    {
        interactionText.color = color;
        interactionText.text = message;

        // Fade in
        float elapsed = 0f;
        while (elapsed < interactionFadeDuration)
        {
            interactionText.alpha = Mathf.Lerp(0, 1, elapsed / interactionFadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        interactionText.alpha = 1;

        yield return new WaitForSeconds(interactionDisplayTime);

        // Fade out
        elapsed = 0f;
        while (elapsed < interactionFadeDuration)
        {
            interactionText.alpha = Mathf.Lerp(1, 0, elapsed / interactionFadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        interactionText.text = "";
        interactionText.alpha = 0;
    }

    /**
     * @brief Show thought message with typewriter effect.
     */
    public void ShowThought(string message)
    {
        if (thoughtRoutine != null) StopCoroutine(thoughtRoutine);
        thoughtRoutine = StartCoroutine(ShowThoughtRoutine(message));
    }

    IEnumerator ShowThoughtRoutine(string message)
    {
        thoughtText.text = "";
        thoughtText.color = thoughtColor;
        thoughtText.alpha = 1f;

        foreach (char letter in message)
        {
            thoughtText.text += letter;
            yield return new WaitForSeconds(thoughtTypingSpeed);
        }

        yield return new WaitForSeconds(thoughtDisplayTime);

        float elapsed = 0f;
        float startAlpha = thoughtText.alpha;
        while (elapsed < thoughtFadeDuration)
        {
            thoughtText.alpha = Mathf.Lerp(startAlpha, 0, elapsed / thoughtFadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        thoughtText.text = "";
        thoughtText.alpha = 0f;
    }

    /**
     * @brief Show inventory message (add/remove).
     */
    public void ShowInventoryMessage(string message, bool isAdding)
    {
        if (isAdding) { inventoryText.color = addColor; }
        else { inventoryText.color = removeColor; }
        if (inventoryRoutine != null) StopCoroutine(inventoryRoutine);
        inventoryRoutine = StartCoroutine(ShowInventoryRoutine(message));
    }

    IEnumerator ShowInventoryRoutine(string message)
    {
        inventoryText.text = "";
        inventoryText.alpha = 1f;

        foreach (char letter in message)
        {
            inventoryText.text += letter;
            yield return new WaitForSeconds(inventoryTypingSpeed);
        }

        yield return new WaitForSeconds(inventoryDisplayTime);

        float elapsed = 0f;
        float startAlpha = inventoryText.alpha;
        while (elapsed < inventoryFadeDuration)
        {
            inventoryText.alpha = Mathf.Lerp(startAlpha, 0, elapsed / inventoryFadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        inventoryText.text = "";
        inventoryText.alpha = 0f;
    }

    /**
     * @brief Clear interaction and inventory messages.
     */
    public void ClearMessages()
    {
        if (interactionRoutine != null) StopCoroutine(interactionRoutine);

        interactionText.text = "";
        interactionText.alpha = 0f;
    }
}
