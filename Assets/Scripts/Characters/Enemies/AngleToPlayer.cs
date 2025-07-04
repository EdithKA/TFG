using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @brief Calculates relative angles to the player and manages sprite direction visualization.
 */
public class AngleToPlayer : MonoBehaviour
{
    /// <summary>
    /// Reference to the the player's transform.
    /// </summary>
    private Transform player;

    /// <summary>
    /// Calculated target position (ignoring vertical difference).
    /// </summary>
    private Vector3 targetPos;

    /// <summary>
    /// Direction vector from enemy to the player.
    /// </summary>
    private Vector3 targetDir;

    /// <summary>
    /// Calculated signed angle between enemy forward and the Player direction.
    /// </summary>
    private float angle;

    /// <summary>
    /// Last calculated direction index for animation purposes.
    /// </summary>
    public int lastIndex;

    /// <summary>
    /// Reference to the sprite renderer component.
    /// </summary>
    private SpriteRenderer spriteRenderer;

    /// <summary>
    /// Initializes the player reference and sprite renderer component.
    /// </summary>
    void Start()
    {
        player = FindObjectOfType<PlayerController>().transform;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    /// <summary>
    /// Updates angle calculations and sprite direction each frame.
    /// </summary>
    void Update()
    {
        targetPos = new Vector3(player.position.x, transform.position.y, player.position.z);
        targetDir = targetPos - transform.position;

        angle = Vector3.SignedAngle(targetDir, transform.forward, Vector3.up);

        Vector3 tempScale = Vector3.one;
        if (angle > 0)
        {
            tempScale.x = -1f; 
        }

        lastIndex = GetIndex(angle); 
    }

    /// <summary>
    /// Converts angle value into 8-direction index for animation states.
    /// </summary>
    /// <param name="angle">Signed angle between -180 and 180 degrees.</param>
    /// <returns>Integer index representing cardinal and intercardinal directions.</returns>
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

    /// <summary>
    /// Draws debug visuals in Scene view when object is selected.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, targetPos - transform.position); // Player direction ray
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, targetPos); 
    }
}
