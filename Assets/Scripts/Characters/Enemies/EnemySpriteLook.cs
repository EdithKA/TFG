using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls enemy sprite rotation to face the soundPlayer, with optional vertical tracking.
/// </summary>
public class EnemySpriteLook : MonoBehaviour
{
    /// <summary>
    /// Reference to the soundPlayer's transform.
    /// </summary>
    public Transform target;

    /// <summary>
    /// Whether the enemy can look up/down at the soundPlayer.
    /// </summary>
    public bool canLookVertically;

    /// <summary>
    /// Initializes target reference to the soundPlayer's transform.
    /// </summary>
    void Start()
    {
        target = FindAnyObjectByType<PlayerController>().transform;
    }

    /// <summary>
    /// Updates sprite rotation to face the soundPlayer each frame.
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
