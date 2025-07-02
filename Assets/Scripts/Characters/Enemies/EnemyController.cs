using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyType { None, Melee, Distance }

public class EnemyController : MonoBehaviour
{
    public EnemyType enemyType = EnemyType.None;

    [Header("Behavior Flags")]
    public bool canChase;
    public bool canAttack;

    [Header("Target References")]
    public Transform playerTransform;

    [Header("Movement Settings")]
    public float chaseDistance;
    public float attackDistanceMelee;
    public float attackDistanceRange;
    private NavMeshAgent enemyNavMeshAgent;

    [Header("Animation")]
    public Animator anim;
    AngleToPlayer angleToPlayer;

    [Header("Patrol Settings")]
    public Transform[] waypoints;
    private int currentWaypointIndex = 0;

    [Header("Attack Settings")]
    public int damageAmount = 25;
    public float attackCooldown = 0.2f;
    private float attackTimer = 0f;
    private Stats playerStats;

    [Header("Player LayerMask")]
    public LayerMask playerLayerMask;

    [Header("Obstacle LayerMask")]
    public LayerMask obstacleLayer;

    private void Start()
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

    private void Update()
    {
        NextAction();
        setAnimation();
        HandleAttack();
    }

    void NextAction()
    {
        float dist = Vector3.Distance(transform.position, playerTransform.position);
        canAttack = false;

        if (IsObstacleInFront())
        {
            enemyNavMeshAgent.isStopped = true;
            return;
        }
        else
        {
            enemyNavMeshAgent.isStopped = false;
        }

        if (dist > chaseDistance)
        {
            Patrol();
        }
        else if (dist > GetAttackDistance())
        {
            SetDestinationToPlayer();
        }
        else
        {
            canAttack = CanSeePlayer();
        }
    }

    private bool IsObstacleInFront()
    {
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        float maxDistance = Mathf.Max(chaseDistance, attackDistanceMelee, attackDistanceRange);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, maxDistance, obstacleLayer | playerLayerMask))
        {
            if (hit.collider.transform != playerTransform)
                return true;
        }
        return false;
    }

    private bool CanSeePlayer()
    {
        Vector3 direction = playerTransform.position - transform.position;
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        float attackDistance = GetAttackDistance();

        if (distance > attackDistance)
            return false;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, distance, playerLayerMask))
        {
            if (hit.collider.transform == playerTransform)
            {
                return true;
            }
        }
        return false;
    }

    void setAnimation()
    {
        anim.SetFloat("spriteRot", angleToPlayer.lastIndex);
        anim.SetBool("Attack", canAttack);
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
            int previousIndex = currentWaypointIndex;
            do
            {
                currentWaypointIndex = Random.Range(0, waypoints.Length);
            } while (currentWaypointIndex == previousIndex && waypoints.Length > 1);
            enemyNavMeshAgent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }

    private float GetAttackDistance()
    {
        return enemyType == EnemyType.Melee ? attackDistanceMelee : attackDistanceRange;
    }

    void HandleAttack()
    {
        if (canAttack && attackTimer <= 0f && CanSeePlayer())
        {
            playerStats.TakeDamage(damageAmount);
            attackTimer = attackCooldown;
        }
        else
        {
            attackTimer -= Time.deltaTime;
        }
    }
}
