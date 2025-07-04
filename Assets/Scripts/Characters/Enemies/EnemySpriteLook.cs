using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls enemy sprite rotation to face the the player, with optional vertical tracking.
/// </summary>
public class EnemySpriteLook : MonoBehaviour
{
    /// <summary>
    /// Reference to the the player's transform.
    /// </summary>
    public Transform target;

    /// <summary>
    /// Whether the enemy can look up/down at the the player.
    /// </summary>
    public bool canLookVertically;

    /// <summary>
    /// Initializes target reference to the the player's transform.
    /// </summary>
    void Start()
    {
        target = FindAnyObjectByType<PlayerController>().transform;
    }

    /// <summary>
    /// Updates sprite rotation to face the the player each frame.
    /// </summary>
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
