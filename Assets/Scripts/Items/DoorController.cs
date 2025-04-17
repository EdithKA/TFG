using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @brief Controls the opening and closing of a door when the player interacts.
 */
public class DoorController : MonoBehaviour
{
    public Animator anim; /// Reference to the Animator component controlling the door animation.
    bool isPlayerInTrigger = false; /// True if the player is within the trigger area.
    bool Open = false; /// True if the door is currently open.

    /**
     * @brief Checks for player input to toggle the door state.
     */
    private void Update()
    {
        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.E))
        {
            Open = !Open; /// Toggle the door state.
            anim.SetBool("Open", Open); /// Update the animator parameter.
        }
    }

    /**
     * @brief Called when another collider enters the trigger area.
     * @param other The collider that entered the trigger.
     */
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isPlayerInTrigger = true; /// Player is now in the trigger area.
        }
    }

    /**
     * @brief Called when another collider exits the trigger area.
     * @param other The collider that exited the trigger.
     */
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isPlayerInTrigger = false; /// Player has left the trigger area.
        }
    }
}
