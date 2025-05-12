
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/**
* @brief Calculates relative angles to player and manages sprite direction visualization.
*/
public class AngleToPlayer : MonoBehaviour
{
    private Transform player; /// Reference to the player's transform.
    private Vector3 targetPos; /// Calculated target position (ignoring vertical difference).
    private Vector3 targetDir; /// Direction vector from enemy to player.

    private float angle; /// Calculated signed angle between enemy forward and player direction.
    public int lastIndex; /// Last calculated direction index for animation purposes.

    private SpriteRenderer spriteRenderer; /// Reference to the sprite renderer component.

    /**
    * @brief Initializes player reference and sprite renderer component.
*/
    void Start()
    {
        player = FindObjectOfType<PlayerController>().transform;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    /**
    * @brief Updates angle calculations and sprite direction each frame.
*/
    void Update()
    {
        // Calculate horizontal position difference (ignore vertical axis)
        targetPos = new Vector3(player.position.x, transform.position.y, player.position.z);
        targetDir = targetPos - transform.position;

        // Get signed angle between enemy forward and player direction
        angle = Vector3.SignedAngle(targetDir, transform.forward, Vector3.up);

        // Flip sprite based on left/right angle
        Vector3 tempScale = Vector3.one;
        if (angle > 0)
        {
            tempScale.x = -1f; // Flip sprite when player is to the right
        }

        lastIndex = GetIndex(angle); // Update direction index for animations
    }

    /**
    * @brief Converts angle value into 8-direction index for animation states.
    * @param angle Signed angle between -180 and 180 degrees.
    * @return Integer index representing cardinal and intercardinal directions.
*/
    int GetIndex(float angle)
    {
        // Front-facing directions
        if (angle > -22.5f && angle < 22.6f) return 0; // Front-center
        if (angle >= 22.5f && angle < 67.5f) return 7; // Front-right
        if (angle >= 67.5f && angle < 112.5f) return 6; // Right profile
        if (angle >= 112.5f && angle < 157.5f) return 5; // Back-right

        // Back-facing directions
        if (angle <= -157.5f || angle >= 157.5f) return 4; // Directly behind
        if (angle >= -157.4f && angle < -112.5f) return 3; // Back-left
        if (angle >= -112.5f && angle < -67.5f) return 2; // Left profile
        if (angle >= -67.5f && angle <= -22.5f) return 1; // Front-left

        return lastIndex; // Fallback to previous value
    }

    /**
    * @brief Draws debug visuals in Scene view when object is selected.
*/
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, targetPos); // Player direction ray
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, targetPos); // Connection line to player
    }
}