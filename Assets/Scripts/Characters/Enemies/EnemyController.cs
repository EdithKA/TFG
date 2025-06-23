using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyType { None, Melee, Distance }

public class EnemyController : MonoBehaviour
{
    /// <summary>
    /// Type of enemy behavior (None, Melee, or Distance).
    /// </summary>
    public EnemyType enemyType = EnemyType.None;

    [Header("Behavior Flags")]
    /// <summary>
    /// Whether the enemy can chase the player.
    /// </summary>
    public bool canChase;

    /// <summary>
    /// Whether the enemy can attack the player.
    /// </summary>
    public bool canAttack;

    [Header("Target References")]
    /// <summary>
    /// Reference to the player's transform.
    /// </summary>
    public Transform playerTransform;

    [Header("Movement Settings")]
    /// <summary>
    /// Distance at which the enemy starts chasing the player.
    /// </summary>
    public float chaseDistance;

    /// <summary>
    /// Distance at which the enemy can attack the player.
    /// </summary>
    public float attackDistance;

    /// <summary>
    /// NavMesh agent component for enemy movement.
    /// </summary>
    private NavMeshAgent enemyNavMeshAgent;

    [Header("Animation")]
    /// <summary>
    /// Animator component for enemy animations.
    /// </summary>
    public Animator anim;

    /// <summary>
    /// Script that calculates angle to player for sprite direction.
    /// </summary>
    AngleToPlayer angleToPlayer;

    [Header("Patrol Settings")]
    /// <summary>
    /// Array of waypoints for enemy patrol behavior.
    /// </summary>
    public Transform[] waypoints;

    /// <summary>
    /// Current waypoint index for patrol system.
    /// </summary>
    private int currentWaypointIndex = 0;

    [Header("Attack Settings")]
    /// <summary>
    /// Amount of damage the enemy deals to the player.
    /// </summary>
    public int damageAmount = 25;

    /// <summary>
    /// Time between enemy attacks.
    /// </summary>
    public float attackCooldown = 0.2f;

    /// <summary>
    /// Timer for tracking attack cooldown.
    /// </summary>
    private float attackTimer = 0f;

    /// <summary>
    /// Reference to the player's stats component.
    /// </summary>
    private Stats playerStats;

    [Header("Distance Enemy Settings")]
    /// <summary>
    /// Spotlight component for distance enemy type.
    /// </summary>
    public Light spotLight;

    /// <summary>
    /// Trigger collider for spotlight detection.
    /// </summary>
    public Collider spotTrigger;

    /// <summary>
    /// Whether the player is currently in the spotlight.
    /// </summary>
    public bool playerInSpotlight = false;

    /// <summary>
    /// Timer for distance enemy damage over time.
    /// </summary>
    private float distanceDamageTimer = 0f;

    /// <summary>
    /// Initializes enemy components and references.
    /// </summary>
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

        if (enemyType == EnemyType.Distance && spotTrigger != null)
        {
            spotTrigger.isTrigger = true;
        }
    }

    /// <summary>
    /// Updates enemy behavior, animations, and attack logic each frame.
    /// </summary>
    private void Update()
    {
        NextAction();
        setAnimation();
        HandleAttack();
    }

    /// <summary>
    /// Determines the next action based on distance to player (patrol, chase, or attack).
    /// </summary>
    void NextAction()
    {
        float dist = Vector3.Distance(transform.position, playerTransform.position);
        canAttack = false;

        if (dist > chaseDistance)
        {
            Patrol();
        }
        else if (dist > attackDistance)
        {
            SetDestinationToPlayer();
        }
        else
        {
            canAttack = true;
        }
    }

    /// <summary>
    /// Updates animation parameters based on enemy state and direction.
    /// </summary>
    void setAnimation()
    {
        anim.SetFloat("spriteRot", angleToPlayer.lastIndex);
        anim.SetBool("Attack", canAttack);
    }

    /// <summary>
    /// Sets the enemy's navigation destination to the player's position.
    /// </summary>
    void SetDestinationToPlayer()
    {
        enemyNavMeshAgent.SetDestination(playerTransform.position);
    }

    /// <summary>
    /// Handles enemy patrol behavior between waypoints.
    /// </summary>
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

    /// <summary>
    /// Manages attack logic for different enemy types (Melee and Distance).
    /// </summary>
    void HandleAttack()
    {
        switch (enemyType)
        {
            case EnemyType.Melee:
                if (canAttack && attackTimer <= 0f)
                {
                    playerStats.TakeDamage(damageAmount);
                    attackTimer = attackCooldown;
                }
                else
                {
                    attackTimer -= Time.deltaTime;
                }
                break;

            case EnemyType.Distance:
                if (canAttack && playerInSpotlight)
                {
                    distanceDamageTimer += Time.deltaTime;
                    if (distanceDamageTimer >= 1f)
                    {
                        playerStats.TakeDamage(damageAmount);
                        distanceDamageTimer -= 1f;
                    }
                }
                else
                {
                    distanceDamageTimer = 0f;
                }
                break;
        }
    }

    /// <summary>
    /// Detects when player enters the spotlight trigger for distance enemies.
    /// </summary>
    /// <param name="other">The collider that entered the trigger.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (enemyType == EnemyType.Distance && other.CompareTag("Player"))
        {
            playerInSpotlight = true;
        }
    }

    /// <summary>
    /// Detects when player exits the spotlight trigger for distance enemies.
    /// </summary>
    /// <param name="other">The collider that exited the trigger.</param>
    private void OnTriggerExit(Collider other)
    {
        if (enemyType == EnemyType.Distance && other.CompareTag("Player"))
        {
            playerInSpotlight = false;
        }
    }
}
