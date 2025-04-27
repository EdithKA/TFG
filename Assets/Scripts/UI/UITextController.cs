using System.Collections;
using UnityEngine;
using TMPro;

public enum UIMessageType { Interact, Collect, Collected, Read }

public class UITextController : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI uiText;
    public CanvasGroup canvasGroup;

    [Header("Style Settings")]
    public Color interactColor = Color.cyan;
    public Color collectColor = Color.yellow;
    public Color collectedColor = Color.green; // Nuevo color para Collected
    public Color readColor = Color.white;
    public float textSize = 28f;

    [Header("Animation Settings")]
    public float typingSpeed = 0.05f;
    public float messageDuration = 2f; // Tiempo que se muestra el mensaje
    public float fadeDuration = 0.4f;

    private Coroutine currentRoutine;

    private void Awake() => canvasGroup.alpha = 0;

    public void ShowMessage(UIMessageType type, string message = null)
    {
        ClearMessage();
        if (currentRoutine != null) StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(ShowMessageRoutine(type, message));
    }

    private IEnumerator ShowMessageRoutine(UIMessageType type, string message = null)
    {
        // Configurar estilo según tipo
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
                uiText.color = collectedColor; // Usar color específico
                break;
            case UIMessageType.Read:
                uiText.color = readColor;
                break;
        }

        uiText.fontSize = textSize;

        // Fade in
        yield return StartCoroutine(FadeCanvas(1f));

        // Escribir texto
        uiText.text = "";
        foreach (char letter in message)
        {
            uiText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        // Esperar antes de ocultar
        yield return new WaitForSeconds(messageDuration);

        // Fade out
        yield return StartCoroutine(FadeCanvas(0f));
    }

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

    public void ClearMessage() => canvasGroup.alpha = 0;
}
