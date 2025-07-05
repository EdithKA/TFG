using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @brief This script calculates the angle between the enemy and the player to determine animation direction.
 */
public class AngleToPlayer : MonoBehaviour
{
    Transform player; ///< Reference to the player
    Vector3 targetPos; ///< Player's position projected on the horizontal plane
    Vector3 targetDir; ///< Direction from the enemy to the player
    float angle; ///< Calculated angle between the enemy's forward and the player
    public int lastIndex; ///< Direction index for animations
    SpriteRenderer spriteRenderer;

    /**
     * @brief At the start, references are assigned automatically.
     */
    void Start()
    {
        player = FindObjectOfType<PlayerController>().transform;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    /**
     * @brief Every frame, the angle and direction for the enemy are calculated.
     */
    void Update()
    {
        // The player's position is obtained and the direction towards them is calculated
        targetPos = new Vector3(player.position.x, transform.position.y, player.position.z);
        targetDir = targetPos - transform.position;

        // The angle between the enemy's forward and the direction is calculated
        angle = Vector3.SignedAngle(targetDir, transform.forward, Vector3.up);

        // The index for the enemy's animation is calculated
        lastIndex = GetIndex(angle);
    }

    /**
     * @brief Calculates the animation index based on the angle.
     * @param angle The angle between the enemy's forward and the player.
     * @return The animation direction index.
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

}
