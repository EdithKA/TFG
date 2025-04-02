using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public bool isAggressive;
    public Transform playerTransform;
    private NavMeshAgent enemyNavMeshAgent;
    public float attackDistance;

    // Animation
    public Animator anim;
    AngleToPlayer angleToPlayer;

    // Patrol
    public Transform[] waypoints; // Array de waypoints
    private int currentWaypointIndex = 0;

    private void Start()
    {
        playerTransform = FindObjectOfType<PlayerMove>().transform;
        enemyNavMeshAgent = GetComponent<NavMeshAgent>();
        angleToPlayer = GetComponent<AngleToPlayer>();

        if (waypoints.Length > 0)
        {
            enemyNavMeshAgent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }

    private void Update()
    {
        isAggressive = CheckPlayerDistance();

        anim.SetFloat("spriteRot", angleToPlayer.lastIndex);

        if (isAggressive)
        {
            SetDestinationToPlayer();
        }
        else
        {
            Patrol();
        }
    }

    bool CheckPlayerDistance()
    {
        var dist = Vector3.Distance(transform.position, playerTransform.position);
        return dist <= attackDistance;
    }

    void SetDestinationToPlayer()
    {
        enemyNavMeshAgent.SetDestination(playerTransform.position);
    }

    void Patrol()
    {
        if (waypoints.Length == 0) return;

        if (!enemyNavMeshAgent.pathPending && enemyNavMeshAgent.remainingDistance < 0.5f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            enemyNavMeshAgent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }
}
