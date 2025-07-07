using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @brief This script allows the enemy's sprite to always face the player.
 */
public class EnemySpriteLook : MonoBehaviour
{
    public Transform target; ///< The player's position, the enemy always looks at the player

    /**
     * @brief Assigns the reference to the player in the scene.
     */
    void Start()
    {
        target = FindAnyObjectByType<PlayerController>().transform;
    }

    /**
     * @brief Updates the enemy's orientation every frame to face the player.
     */
    void Update()
    {
        Vector3 modifiedTarget = target.position;
        modifiedTarget.y = transform.position.y;
        transform.LookAt(modifiedTarget);
       
    }
}
