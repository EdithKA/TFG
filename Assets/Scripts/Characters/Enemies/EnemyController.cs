using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyController : MonoBehaviour
{
    [Header("Behavior Flags")]
    public bool canChase;
    public bool canAttack;

    [Header("Target References")]
    public Transform playerTransform;
    public Transform eyes;

    [Header("Movement Settings")]
    public float chaseDistance;
    protected NavMeshAgent enemyNavMeshAgent;

    [Header("Animation")]
    public Animator anim;
    protected AngleToPlayer angleToPlayer;

    [Header("Patrol Settings")]
    public Transform[] waypoints;
    protected int currentWaypointIndex = 0;

    [Header("Attack Settings")]
    public int damageAmount;
    public float attackCooldown;
    protected float attackTimer;
    protected Stats playerStats;

    protected virtual void Start()
    {
        playerStats = FindAnyObjectByType<Stats>();
        playerTransform = FindObjectOfType<PlayerController>().transform;
        enemyNavMeshAgent = GetComponent<NavMeshAgent>();
        angleToPlayer = GetComponent<AngleToPlayer>();

        if (waypoints.Length > 0)
        {
            enemyNavMeshAgent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }

    protected virtual void Update()
    {
        NextAction();
        setAnimation();
        HandleAttack();
    }

    protected abstract void NextAction();

    protected void setAnimation()
    {
        anim.SetFloat("spriteRot", angleToPlayer.lastIndex);
        anim.SetBool("Attack", canAttack);
    }

    protected void SetDestinationToPlayer()
    {
        enemyNavMeshAgent.SetDestination(playerTransform.position);
    }

    protected void Patrol()
    {
        if (waypoints.Length == 0) return;

        if (!enemyNavMeshAgent.pathPending && enemyNavMeshAgent.remainingDistance < 0.5f)
        {
            int previousIndex = currentWaypointIndex;
            do
            {
                currentWaypointIndex = Random.Range(0, waypoints.Length);
            } while (currentWaypointIndex == previousIndex && waypoints.Length > 1);
            enemyNavMeshAgent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }

    protected abstract void HandleAttack();
}
