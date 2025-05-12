using System.Collections;
using UnityEngine;
using TMPro;

public enum UIMessageType { Interact, Collect, Collected, Read }

public class UITextController : MonoBehaviour
{
    [Header("Text Components")]
    public TextMeshProUGUI interactionText;  // Action messages
    public TextMeshProUGUI thoughtText;      // Thoughts/Narration

    [Header("Game Texts")]
    public GameTexts gameTexts;              // ScriptableObject with texts

    [Header("Interaction Settings")]
    public Color interactColor = Color.cyan;
    public Color collectColor = Color.yellow;
    public float interactionFadeDuration = 0.4f;
    public float interactionDisplayTime = 1.5f;

    [Header("Thought Settings")]
    public Color thoughtColor = Color.white;
    public float thoughtTypingSpeed = 0.03f;
    public float thoughtDisplayTime = 3f;    // Time thought stays after writing
    public float thoughtFadeDuration = 0.5f; // Fade out duration

    private Coroutine interactionRoutine;
    private Coroutine thoughtRoutine;

    /// <summary> Shows an interaction message with fade </summary>
    public void ShowInteraction(string message, Color color)
    {
        if (interactionRoutine != null) StopCoroutine(interactionRoutine);
        interactionRoutine = StartCoroutine(ShowInteractionRoutine(message, color));
    }

    /// <summary> Shows a thought with typewriter effect, always </summary>
    public void ShowThought(string message)
    {
        if (thoughtRoutine != null) StopCoroutine(thoughtRoutine);
        thoughtRoutine = StartCoroutine(ShowThoughtTypewriterRoutine(message));
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

    private IEnumerator ShowThoughtTypewriterRoutine(string message)
    {
        thoughtText.text = "";
        thoughtText.color = thoughtColor;
        thoughtText.alpha = 1f;

        // Typewriter effect
        foreach (char letter in message)
        {
            thoughtText.text += letter;
            yield return new WaitForSeconds(thoughtTypingSpeed);
        }

        // Wait after writing
        yield return new WaitForSeconds(thoughtDisplayTime);

        // Fade out
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

    public void ClearMessages()
    {
        //if (thoughtRoutine != null) StopCoroutine(thoughtRoutine);
        if (interactionRoutine != null) StopCoroutine(interactionRoutine);

        //thoughtText.text = "";
        interactionText.text = "";
        //thoughtText.alpha = 0f;
        interactionText.alpha = 0f;
    }
}
