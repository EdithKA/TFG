using UnityEngine;

public class DoorInteractuable : MonoBehaviour, IInteractuable
{
    public Animator anim;
    private bool isOpen = false;
    private bool isPlayerInTrigger = false;

    void Update()
    {
        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    // Implementación de la interfaz
    public void Interact(GameObject objectOnHand = null)
    {
        if (anim == null || anim.runtimeAnimatorController == null)
        {
            Debug.LogError("Animator o Animator Controller no asignado en " + gameObject.name);
            return;
        }
        isOpen = !isOpen;
        anim.SetBool("Open", isOpen);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
        }
    }
}
