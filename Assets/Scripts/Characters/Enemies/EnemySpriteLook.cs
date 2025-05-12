using UnityEngine;

/**
 * @brief Makes the enemy sprite always face the player.
 *        Optionally, the enemy can also look up/down if needed.
 *        Super useful for 2.5D or top-down games where you want the enemy to "track" the player visually.
 */
public class EnemySpriteLook : MonoBehaviour
{
    [Header("Target Reference")]
    public Transform target;             /// Reference to the player's transform (set automatically in Start).

    [Header("Look Settings")]
    public bool canLookVertically = false; /// If true, the enemy will look up/down at the player (full 3D tracking).

    /**
     * @brief Finds the player in the scene and stores their transform.
     */
    void Start()
    {
        // Find the PlayerController in the scene and get its transform
        target = FindAnyObjectByType<PlayerController>().transform;
    }

    /**
     * @brief Rotates the enemy each frame so it always faces the player.
     *        If canLookVertically is false, only rotates horizontally.
     */
    void Update()
    {
        if (target == null) return; // Safety check

        if (canLookVertically)
        {
            // Enemy looks directly at the player (including up/down)
            transform.LookAt(target);
        }
        else
        {
            // Only rotate on the Y axis (so the enemy doesn't tilt up/down)
            Vector3 flatTargetPosition = target.position;
            flatTargetPosition.y = transform.position.y; // Keep Y the same as the enemy
            transform.LookAt(flatTargetPosition);
        }
    }
}
