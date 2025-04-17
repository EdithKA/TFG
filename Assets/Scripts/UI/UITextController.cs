using System.Collections;
using UnityEngine;
using TMPro;

/**
 * @brief Controls the display and animation of UI instructions for player interaction.
 */
public class UITextController : MonoBehaviour
{
    public TextMeshProUGUI instructionText; /// Reference to the UI text component for instructions.
    public string message = "Pulsa 'E' para interactuar"; /// Message displayed to the player.
    public float typingSpeed = 0.05f; /// Time between each character when typing.
    public float deletingSpeed = 0.05f; /// Time between each character when deleting.

    private Coroutine typingCoroutine; /// Reference to the currently running typing coroutine.
    private Coroutine deletingCoroutine; /// Reference to the currently running deleting coroutine.

    /**
     * @brief Called when another collider enters this object's trigger area.
     * @param other The collider that entered the trigger.
     */
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Stop any ongoing coroutines to avoid overlap.
            if (deletingCoroutine != null) StopCoroutine(deletingCoroutine);
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);

            typingCoroutine = StartCoroutine(TypeText()); /// Start typing the instruction message.
        }
    }

    /**
     * @brief Called when another collider exits this object's trigger area.
     * @param other The collider that exited the trigger.
     */
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Stop any ongoing coroutines to avoid overlap.
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            if (deletingCoroutine != null) StopCoroutine(deletingCoroutine);

            deletingCoroutine = StartCoroutine(DeleteText()); /// Start deleting the instruction message.
        }
    }

    /**
     * @brief Coroutine that types out the message character by character.
     */
    private IEnumerator TypeText()
    {
        instructionText.text = ""; /// Clear the text before typing.
        foreach (char c in message.ToCharArray())
        {
            instructionText.text += c; /// Add the next character.
            yield return new WaitForSeconds(typingSpeed); /// Wait before adding the next character.
        }
    }

    /**
     * @brief Coroutine that deletes the message character by character.
     */
    private IEnumerator DeleteText()
    {
        while (instructionText.text.Length > 0)
        {
            instructionText.text = instructionText.text.Substring(0, instructionText.text.Length - 1); /// Remove the last character.
            yield return new WaitForSeconds(deletingSpeed); /// Wait before removing the next character.
        }
    }
}
