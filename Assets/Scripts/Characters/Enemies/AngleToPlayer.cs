using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @brief Calculates angle to player for animation direction.
 */
public class AngleToPlayer : MonoBehaviour
{
    private Transform player;           ///< Player's Transform
    private Vector3 targetPos;          ///< Player position (horizontal)
    private Vector3 targetDir;          ///< Direction to player
    private float angle;                ///< Angle to player
    public int lastIndex;               ///< Animation direction index
    private SpriteRenderer spriteRenderer; ///< SpriteRenderer ref

    /**
     * @brief Get refs.
     */
    void Start()
    {
        player = FindObjectOfType<PlayerController>().transform;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    /**
     * @brief Update angle and direction each frame.
     */
    void Update()
    {
        targetPos = new Vector3(player.position.x, transform.position.y, player.position.z);
        targetDir = targetPos - transform.position;
        angle = Vector3.SignedAngle(targetDir, transform.forward, Vector3.up);
        lastIndex = GetIndex(angle);
    }

    /**
     * @brief Get direction index from angle.
     */
    int GetIndex(float angle)
    {
        if (angle > -22.5f && angle < 22.6f) return 0; // Front
        if (angle >= 22.5f && angle < 67.5f) return 7; // Front-right
        if (angle >= 67.5f && angle < 112.5f) return 6; // Right
        if (angle >= 112.5f && angle < 157.5f) return 5; // Back-right

        if (angle <= -157.5f || angle >= 157.5f) return 4; // Back
        if (angle >= -157.4f && angle < -112.5f) return 3; // Back-left
        if (angle >= -112.5f && angle < -67.5f) return 2; // Left
        if (angle >= -67.5f && angle <= -22.5f) return 1; // Front-left

        return lastIndex;
    }
}
