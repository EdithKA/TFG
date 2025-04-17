using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @brief Controls enemy sprite rotation to face the player, with optional vertical tracking.
 */
public class EnemySpriteLook : MonoBehaviour
{
    public Transform target; /// Reference to the player's transform.
    public bool canLookVertically; /// Can the enemy look up/down at the player?

    /**
     * @brief Initializes target reference to the player's transform.
     */
    void Start()
    {
        target = FindAnyObjectByType<PlayerController>().transform;
    }

    /**
     * @brief Updates sprite rotation to face the player each frame.
     */
    void Update()
    {
        if (canLookVertically)
        {
            // Full 3D rotation tracking
            transform.LookAt(target);
        }
        else
        {
            // Horizontal-only tracking (lock Y-axis position)
            Vector3 modifiedTarget = target.position;
            modifiedTarget.y = transform.position.y;
            transform.LookAt(modifiedTarget);
        }
    }
}
