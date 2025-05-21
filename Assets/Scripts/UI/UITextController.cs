using System.Collections;
using UnityEngine;
using TMPro;

public enum UIMessageType { Interact, Collect, Collected, Read }

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

    private Coroutine interactionRoutine;
    private Coroutine thoughtRoutine;
    private Coroutine inventoryRoutine;

    /// <summary>
    /// Muestra un mensaje de interacción con el color por defecto.
    /// </summary>
    public void ShowInteraction(string message)
    {
        ShowInteraction(message, interactColor);
    }

    /// <summary>
    /// Muestra un mensaje de interacción con color personalizado (si alguna vez lo necesitas).
    /// </summary>
    public void ShowInteraction(string message, Color color)
    {
        if (interactionRoutine != null) StopCoroutine(interactionRoutine);
        interactionRoutine = StartCoroutine(ShowInteractionRoutine(message, color));
    }

    /// <summary>
    /// Muestra un mensaje de colección con el color definido para colección.
    /// </summary>
    public void ShowCollect(string message)
    {
        ShowInteraction(message, collectColor);
    }

    private IEnumerator ShowInteractionRoutine(string message, Color color)
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
        interactionText.alpha = 0;
        interactionText.text = "";
    }

    /// <summary>
    /// Muestra un pensamiento con efecto máquina de escribir y color por defecto.
    /// </summary>
    public void ShowThought(string message)
    {
        if (thoughtRoutine != null) StopCoroutine(thoughtRoutine);
        thoughtRoutine = StartCoroutine(ShowThoughtRoutine(message));
    }

    private IEnumerator ShowThoughtRoutine(string message)
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

    /// <summary>
    /// Muestra mensaje de inventario con color por defecto.
    /// </summary>
    public void ShowInventoryMessage(string message, bool isAdding)
    {
        if (isAdding) { inventoryText.color = addColor; }
        else { inventoryText.color = removeColor; }
        if (inventoryRoutine != null) StopCoroutine(inventoryRoutine);
        inventoryRoutine = StartCoroutine(ShowInventoryRoutine(message));
    }

    private IEnumerator ShowInventoryRoutine(string message)
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

    /// <summary>
    /// Limpia los mensajes de interacción e inventario.
    /// </summary>
    public void ClearMessages()
    {
        if (interactionRoutine != null) StopCoroutine(interactionRoutine);

        interactionText.text = "";
        interactionText.alpha = 0f;
    }
}
