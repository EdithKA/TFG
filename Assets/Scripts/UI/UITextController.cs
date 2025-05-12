using System.Collections;
using UnityEngine;
using TMPro;

/**
 * @brief Types of UI messages that can be shown to the player.
 */
public enum UIMessageType { Interact, Collect, Collected, Read }

/**
 * @brief Controls the display, style, and animation of in-game UI messages.
 *        Supports different message types (interact, collect, collected, read) with typing and fade effects.
 */
public class UITextController : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI uiText;              /// Reference to the UI text component.
    public CanvasGroup canvasGroup;             /// CanvasGroup for fading in/out.

    [Header("Style Settings")]
    public Color interactColor = Color.cyan;    /// Color for interact messages.
    public Color collectColor = Color.yellow;   /// Color for collect messages.
    public Color collectedColor = Color.green;  /// Color for collected messages.
    public Color readColor = Color.white;       /// Color for read messages.
    public float textSize = 28f;                /// Font size for all messages.

    [Header("Animation Settings")]
    public float typingSpeed = 0.05f;           /// Delay between each character for typing effect.
    public float messageDuration = 1f;          /// How long the message stays visible.
    public float fadeDuration = 0.4f;           /// Fade in/out duration.

    private Coroutine currentRoutine;           /// Reference to the currently running message coroutine.

    /**
     * @brief Ensures the UI starts hidden.
     */
    private void Awake()
    {
        canvasGroup.alpha = 0;
    }

    /**
     * @brief Shows a message of a specific type, with optional custom text.
     * @param type The type of message (affects color and default text).
     * @param message Optional custom message (overrides default for type).
     */
    public void ShowMessage(UIMessageType type, string message = null)
    {
        ClearMessage();
        if (currentRoutine != null) StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(ShowMessageRoutine(type, message));
    }

    /**
     * @brief Handles the animated display of the message, including typing and fading.
     */
    private IEnumerator ShowMessageRoutine(UIMessageType type, string message = null)
    {
        // Set style and default message based on type
        switch (type)
        {
            case UIMessageType.Interact:
                message = "Pulsa [E] para interactuar";
                uiText.color = interactColor;
                break;
            case UIMessageType.Collect:
                message = "Pulsa [E] para recoger";
                uiText.color = collectColor;
                break;
            case UIMessageType.Collected:
                uiText.color = collectedColor;
                break;
            case UIMessageType.Read:
                uiText.color = readColor;
                break;
        }

        uiText.fontSize = textSize;

        // Fade in the canvas
        yield return StartCoroutine(FadeCanvas(1f));

        // Typing effect: show message letter by letter
        uiText.text = "";
        foreach (char letter in message)
        {
            uiText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        // Wait for the message to stay visible
        yield return new WaitForSeconds(messageDuration);

        // Fade out the canvas
        yield return StartCoroutine(FadeCanvas(0f));
    }

    /**
     * @brief Fades the canvas alpha to the target value over time.
     */
    private IEnumerator FadeCanvas(float targetAlpha)
    {
        float startAlpha = canvasGroup.alpha;
        float t = 0f;

        while (t < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t / fadeDuration);
            t += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }

    /**
     * @brief Instantly clears the message and hides the canvas.
     */
    public void ClearMessage()
    {
        uiText.text = string.Empty;
        canvasGroup.alpha = 0;
        StopAllCoroutines();
    }
}
