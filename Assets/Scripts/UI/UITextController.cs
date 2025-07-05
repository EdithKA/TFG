using System.Collections;
using UnityEngine;
using TMPro;

/**
 * @brief Enum for UI message types.
 */
public enum UIMessageType { Interact, Collect, Collected, Read }

/**
 * @brief Controls the UI text messages for interaction, thoughts, and inventory.
 */
public class UITextController : MonoBehaviour
{
    [Header("Text Components")]
    public TextMeshProUGUI interactionText;
    public TextMeshProUGUI thoughtText;
    public TextMeshProUGUI inventoryText;

    [Header("Game Texts")]
    public GameTexts gameTexts;

    [Header("Interaction Settings")]
    public Color interactColor = Color.cyan;
    public Color collectColor = Color.yellow;
    public float interactionFadeDuration = 0.4f;
    public float interactionDisplayTime = 1.5f;

    [Header("Inventory Settings")]
    public Color addColor;
    public Color removeColor;
    public float inventoryTypingSpeed = 0.03f;
    public float inventoryDisplayTime = 1f;
    public float inventoryFadeDuration = 0.5f;

    [Header("Thought Settings")]
    public Color thoughtColor = Color.white;
    public float thoughtTypingSpeed = 0.03f;
    public float thoughtDisplayTime = 3f;
    public float thoughtFadeDuration = 0.5f;

    Coroutine interactionRoutine;
    Coroutine thoughtRoutine;
    Coroutine inventoryRoutine;

    /**
     * @brief Shows an interaction message with the default color.
     * @param message The message to display.
     */
    public void ShowInteraction(string message)
    {
        ShowInteraction(message, interactColor);
    }

    /**
     * @brief Shows an interaction message with a custom color (if ever needed).
     * @param message The message to display.
     * @param color The color for the message.
     */
    public void ShowInteraction(string message, Color color)
    {
        if (interactionRoutine != null) StopCoroutine(interactionRoutine);
        interactionRoutine = StartCoroutine(ShowInteractionRoutine(message, color));
    }

    /**
     * @brief Shows a collect message with the defined collect color.
     * @param message The message to display.
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
     * @brief Shows a thought message with typewriter effect and default color.
     * @param message The message to display.
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
     * @brief Shows an inventory message with the default color.
     * @param message The message to display.
     * @param isAdding True if adding, false if removing.
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
     * @brief Clears interaction and inventory messages.
     */
    public void ClearMessages()
    {
        if (interactionRoutine != null) StopCoroutine(interactionRoutine);

        interactionText.text = "";
        interactionText.alpha = 0f;
    }
}
