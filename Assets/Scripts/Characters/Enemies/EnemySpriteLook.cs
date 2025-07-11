using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @brief Enemy always looks at the player.
 */
public class EnemySpriteLook : MonoBehaviour
{
    public Transform target; ///< Player's Transform

    /**
     * @brief Get player ref.
     */
    void Start()
    {
        target = FindAnyObjectByType<PlayerController>().transform;
    }

    /**
     * @brief Rotate to face player (horizontal only).
     */
    void Update()
    {
        Vector3 modifiedTarget = target.position;
        modifiedTarget.y = transform.position.y;
        transform.LookAt(modifiedTarget);
    }
}
