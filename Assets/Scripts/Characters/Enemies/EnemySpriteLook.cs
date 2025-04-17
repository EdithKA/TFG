using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @brief This script makes the enemy's sprite always look towards the player.
 */
public class EnemySpriteLook : MonoBehaviour
{
    Transform target;
    public bool canLookVertically;

    void Start()
    {
        target = FindAnyObjectByType<PlayerController>().transform;
    }


    void Update()
    {
        if(canLookVertically)
        {
            transform.LookAt(target);
        }
        else
        {
            Vector3 modifiedTarget = target.position;
            modifiedTarget.y = transform.position.y;
            transform.LookAt(modifiedTarget);
        }
    }
}
