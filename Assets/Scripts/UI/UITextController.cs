using System.Collections;
using UnityEngine;
using TMPro;

public class UITextController : MonoBehaviour
{
    public  TextMeshProUGUI instructionText; 
    public string message = "Pulsa 'E' para interactuar"; 
    public float typingSpeed = 0.05f; 
    public float deletingSpeed = 0.05f; 

    private Coroutine typingCoroutine; 
    private Coroutine deletingCoroutine; 

   

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {

            if (deletingCoroutine != null) StopCoroutine(deletingCoroutine); 
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);

            typingCoroutine = StartCoroutine(TypeText()); 
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) 
        {

            if (typingCoroutine != null) StopCoroutine(typingCoroutine); 
            if (deletingCoroutine != null) StopCoroutine(deletingCoroutine); 

            deletingCoroutine = StartCoroutine(DeleteText());
        }
    }

    private IEnumerator TypeText()
    {
        instructionText.text = ""; 
        foreach (char c in message.ToCharArray())
        {
            instructionText.text += c; 
            yield return new WaitForSeconds(typingSpeed); 
        }    
    }

    private IEnumerator DeleteText()
    {
        while (instructionText.text.Length > 0)
        {
            instructionText.text = instructionText.text.Substring(0, instructionText.text.Length - 1); 
            yield return new WaitForSeconds(deletingSpeed); 
        }
    }
}
