using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public bool isAggressive;
    public Transform playerTransform;
    private NavMeshAgent enemyNavMeshAgent;
    public float attackDistance;

    private void Start()
    {
        playerTransform = FindObjectOfType<PlayerMove>().transform;
        enemyNavMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        isAggressive = CheckPlayerDistance();

        SetDestination();
    }

    bool CheckPlayerDistance()
    {
        var dist = Vector3.Distance(transform.position, playerTransform.position);
        
        return dist <= attackDistance;
    }

    void SetDestination()
    {
        if(isAggressive)
        {
            enemyNavMeshAgent.SetDestination(playerTransform.position);
        }
        else
        {
            enemyNavMeshAgent.SetDestination(transform.position);
        }

    }
}
