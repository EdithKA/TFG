using System.Collections;
using System.Collections.Generic;
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
    /// Distance at which the enemy can attack the player (melee).
    /// </summary>
    public float attackDistanceMelee;

    /// <summary>
    /// Distance at which the enemy can attack the player (distance).
    /// </summary>
    public float attackDistanceRange;

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
    /// Light component for distance enemy type.
    /// </summary>
    public Light spotLight;

    /// <summary>
    /// Object representing the light cone.
    /// </summary>
    public GameObject lightCone;

    [Header("Door Detection")]
    /// <summary>
    /// LayerMask for detecting closed doors.
    /// </summary>
    public LayerMask doorLayer;

    /// <summary>
    /// LayerMask for detecting walls and obstacles.
    /// </summary>
    public LayerMask obstacleLayer;

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
        else if (dist > GetAttackDistance())
        {
            SetDestinationToPlayer();
        }
        else
        {
            canAttack = CanSeePlayer();
        }

        // Desactiva la luz y el objeto si hay una puerta cerrada de por medio
        if (enemyType == EnemyType.Distance)
        {
            bool doorBlocking = IsDoorBlockingPath();
            if (spotLight != null)
                spotLight.enabled = !doorBlocking;
            if (lightCone != null)
                lightCone.SetActive(!doorBlocking);
        }
    }

    /// <summary>
    /// Checks if a closed door is blocking the path to the player.
    /// </summary>
    private bool IsDoorBlockingPath()
    {
        Vector3 direction = playerTransform.position - transform.position;
        float distance = Vector3.Distance(transform.position, playerTransform.position);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, distance, doorLayer))
        {
            DoorInteractable door = hit.collider.GetComponent<DoorInteractable>();
            if (door != null && !door.isOpen)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Checks if the enemy can see the player directly (no obstacles between).
    /// </summary>
    private bool CanSeePlayer()
    {
        Vector3 direction = playerTransform.position - transform.position;
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        float attackDistance = GetAttackDistance();

        if (distance > attackDistance)
            return false;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, distance, obstacleLayer))
        {
            if (hit.collider.CompareTag("Player"))
            {
                return true;
            }
            return false;
        }
        // Si no hay obstáculos, pero el raycast no devuelve nada (raro), asumimos que hay línea de visión
        return Physics.Raycast(transform.position, direction, out hit, distance, doorLayer)
            ? false : true; // Si hay puerta, false (ya se comprueba antes), si no, true
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
    /// Returns attack distance based on enemy type.
    /// </summary>
    private float GetAttackDistance()
    {
        return enemyType == EnemyType.Melee ? attackDistanceMelee : attackDistanceRange;
    }

    /// <summary>
    /// Manages attack logic for both enemy types.
    /// </summary>
    void HandleAttack()
    {
        if (canAttack && attackTimer <= 0f)
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
