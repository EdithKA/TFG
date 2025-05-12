using UnityEngine;

/**
 * @brief Controls door interaction behavior (opening/closing) using animations.
 *        Implements the IInteractable interface for player interactions.
 */
public class DoorInteractable : MonoBehaviour, IInteractable
{
    [Header("Animation Settings")]
    [SerializeField] private Animator doorAnimator; /// Reference to the door's Animator component.

    [Header("UI References")]
    public UITextController uiTextController;       /// Reference to the UI text controller.
    public GameTexts gameTexts;                     /// Reference to the centralized game texts.

    private bool isOpen = false;                    /// Current state of the door.
    private bool isPlayerInRange = false;           /// Is the player in interaction range?


    private void Start()
    {
       uiTextController = FindAnyObjectByType<UITextController>();
       gameTexts = uiTextController.gameTexts;
    }
    /**
     * @brief Checks for player interaction input each frame.
     */
    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    /**
     * @brief Toggles the door's open/close state through animation.
     * @param objectOnHand Not used in this implementation (required by interface).
     */
    public void Interact(GameObject objectOnHand = null)
    {
        if (doorAnimator == null || doorAnimator.runtimeAnimatorController == null)
        {
            Debug.LogError("Animator or Animator Controller not assigned on " + gameObject.name);
            return;
        }

        isOpen = !isOpen;
        doorAnimator.SetBool("Open", isOpen);

        // Puedes mostrar un pensamiento o feedback si quieres:
        // if (uiTextController != null && gameTexts != null)
        //     uiTextController.ShowThought(isOpen ? "He abierto la puerta." : "He cerrado la puerta.");
    }

    /**
     * @brief Sets interaction flag when player enters trigger zone and shows interaction message.
     */
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (uiTextController != null && gameTexts != null)
                uiTextController.ShowInteraction(gameTexts.interactMessage, uiTextController.interactColor);
        }
    }

    /**
     * @brief Clears interaction flag and hides interaction message when player exits trigger zone.
     */
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (uiTextController != null)
                uiTextController.ClearMessages();
        }
    }
}
