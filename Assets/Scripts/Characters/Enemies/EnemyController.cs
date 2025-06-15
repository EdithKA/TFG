
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyType { None, Melee, Distance}
/**
* @brief Controls enemy AI behavior including patrolling, chasing, and attacking the player.
*/
public class EnemyController : MonoBehaviour
{
    public EnemyType enemyType = EnemyType.None;
    [Header("Behavior Flags")]
    public bool canChase; /// Can the enemy chase the player?
    public bool canAttack; /// Is the enemy currently attacking?

    [Header("Target References")]
    public Transform playerTransform; /// Reference to the player's transform.

    [Header("Movement Settings")]
    public float chaseDistance; /// Minimum distance to start chasing.
    public float attackDistance; /// Minimum distance to initiate attack.

    private NavMeshAgent enemyNavMeshAgent; /// Reference to the NavMeshAgent component.

    [Header("Animation")]
    public Animator anim; /// Animator controller for enemy sprites.
    AngleToPlayer angleToPlayer; /// Component to determine facing direction.

    [Header("Patrol Settings")]
    public Transform[] waypoints; /// Array of patrol waypoints.
    private int currentWaypointIndex = 0; /// Current waypoint index in array.

    [Header("Attack Settings")]
    public int damageAmount = 25;           // Daño por ataque
    public float attackCooldown = 0.2f;     // Tiempo entre ataques
    private float attackTimer = 0f;
    private Stats playerStats;


    /**
    * @brief Initializes components and starts patrolling.
*/
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

        if(enemyType == EnemyType.Distance)
        {
            attackDistance = attackDistance * 0.5f;
        }
    }

    /**
    * @brief Updates enemy behavior and animations each frame.
*/
    private void Update()
    {
        NextAction();
        setAnimation();
        HandleAttack();
    }

    /**
    * @brief Determines enemy behavior based on distance to player.
*/
    void NextAction()
    {
        float dist = Vector3.Distance(transform.position, playerTransform.position);
        canAttack = false;

        if (dist > chaseDistance)
        {
            Patrol(); // Patrol when player is far
        }
        else if (dist > attackDistance)
        {
            SetDestinationToPlayer(); // Chase when player is in chase range
        }
        else
        {
            canAttack = true; // Attack when player is close
        }
    }

    /**
    * @brief Updates animation parameters based on facing direction and attack state.
*/
    void setAnimation()
    {
        anim.SetFloat("spriteRot", angleToPlayer.lastIndex); // Set sprite rotation direction
        anim.SetBool("Attack", canAttack); // Trigger attack animation
    }

    /**
    * @brief Sets navigation target to player's current position.
*/
    void SetDestinationToPlayer()
    {
        enemyNavMeshAgent.SetDestination(playerTransform.position);
    }

    /**
    * @brief Handles waypoint navigation and patrolling behavior.
*/
    void Patrol()
    {
        if (waypoints.Length == 0) return;

        // Cycle through waypoints when reaching current destination
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

    void HandleAttack()
    {
        if(canAttack && attackTimer <= 0f)
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