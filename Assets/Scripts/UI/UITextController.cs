using System.Collections;
using UnityEngine;
using TMPro;

public enum UIMessageType { Interact, Collect, Collected, Read }

public class UITextController : MonoBehaviour
{
    [Header("Text Components")]
    public TextMeshProUGUI interactionText;  // Mensajes de acción
    public TextMeshProUGUI thoughtText;      // Pensamientos/Narración

    [Header("Game Texts")]
    public GameTexts gameTexts;              // ScriptableObject con textos

    [Header("Interaction Settings")]
    public Color interactColor = Color.cyan;
    public Color collectColor = Color.yellow;
    public float interactionFadeDuration = 0.4f;
    public float interactionDisplayTime = 1.5f;

    [Header("Thought Settings")]
    public Color thoughtColor = Color.white;
    public float thoughtTypingSpeed = 0.03f;
    public float thoughtDisplayTime = 3f;

    private Coroutine interactionRoutine;
    private Coroutine thoughtRoutine;

    /// <summary> Muestra un mensaje de interacción con fade </summary>
    public void ShowInteraction(string message, Color color)
    {
        if (interactionRoutine != null) StopCoroutine(interactionRoutine);
        interactionRoutine = StartCoroutine(ShowInteractionRoutine(message, color));
    }

    /// <summary> Muestra un pensamiento con efecto de escritura </summary>
    public void ShowThought(string message)
    {
        if (thoughtRoutine != null) StopCoroutine(thoughtRoutine);
        thoughtRoutine = StartCoroutine(ShowThoughtRoutine(message));
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

    private IEnumerator ShowThoughtRoutine(string message)
    {
        thoughtText.color = thoughtColor;
        thoughtText.text = "";

        foreach (char letter in message)
        {
            thoughtText.text += letter;
            yield return new WaitForSeconds(thoughtTypingSpeed);
        }

        yield return new WaitForSeconds(thoughtDisplayTime);
        thoughtText.text = "";
    }

    public void ClearMessages()
    {
        interactionText.text = "";
        thoughtText.text = "";
        interactionText.alpha = 0;
        StopAllCoroutines();
    }
}
