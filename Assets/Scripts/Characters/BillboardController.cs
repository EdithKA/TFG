using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardController : MonoBehaviour
{
    public Transform player; 
    public bool alignToCameraForward = true; 

    private void Start()
    {
        if (player == null)
        {
            player = Camera.main.transform; 
        }
    }

    private void Update()
    {
        if (player == null) return;

        Vector3 directionToPlayer = player.position - transform.position;

        directionToPlayer.y = 0; 

        if (alignToCameraForward)
        {
            transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward, Vector3.up);
        }
        else
        {
            transform.rotation = Quaternion.LookRotation(directionToPlayer.normalized, Vector3.up);
        }
    }
}
