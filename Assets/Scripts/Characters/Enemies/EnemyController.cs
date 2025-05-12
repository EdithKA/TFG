using UnityEngine;
using UnityEngine.AI;

/**
 * @brief Controls the enemy's AI: patrol, chase, and attack logic.
 *        Designed for a survival horror style game.
 */
public class EnemyController : MonoBehaviour
{
    [Header("Behavior Flags")]
    public bool canChase = true;           /// Determines if the enemy is allowed to chase the player.
    public bool isAttacking = false;       /// True if the enemy is currently attacking.

    [Header("Player Reference")]
    public Transform playerTransform;      /// Reference to the player's transform.

    [Header("Distance Settings")]
    public float chaseDistance = 10f;      /// Minimum distance to start chasing the player.
    public float attackDistance = 2f;      /// Minimum distance to start attacking.

    [Header("Patrol Settings")]
    public Transform[] patrolWaypoints;    /// Array of waypoints for patrolling.
    private int currentWaypointIndex = 0;  /// Tracks which waypoint the enemy is moving towards.

    [Header("Components")]
    private NavMeshAgent navAgent;         /// Reference to the NavMeshAgent for pathfinding.
    private AngleToPlayer angleToPlayer;   /// Reference to the script that calculates facing direction.
    public Animator animator;              /// Animator for controlling enemy animations.

    /**
     * @brief Initializes references and starts patrolling if waypoints are set.
     */
    void Start()
    {
        // Find player in the scene (assumes only one PlayerController exists)
        playerTransform = FindObjectOfType<PlayerController>().transform;
        navAgent = GetComponent<NavMeshAgent>();
        angleToPlayer = GetComponent<AngleToPlayer>();

        // Start patrolling if waypoints are set
        if (patrolWaypoints.Length > 0)
        {
            navAgent.SetDestination(patrolWaypoints[currentWaypointIndex].position);
        }
    }

    /**
     * @brief Updates enemy behavior and animation every frame.
     */
    void Update()
    {
        DecideAction();
        UpdateAnimation();
    }

    /**
     * @brief Decides what the enemy should do based on distance to the player.
     */
    void DecideAction()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        isAttacking = false; // Reset attack flag

        // Priority: Attack > Chase > Patrol
        if (distanceToPlayer <= attackDistance)
        {
            // Player is close enough to attack
            isAttacking = true;
            // (Here you could call an Attack() method if you want to add damage logic)
        }
        else if (distanceToPlayer <= chaseDistance && canChase)
        {
            // Player is within chase range
            navAgent.SetDestination(playerTransform.position);
        }
        else
        {
            // Player is too far, patrol between waypoints
            Patrol();
        }
    }

    /**
     * @brief Updates the animator parameters for direction and attack.
     */
    void UpdateAnimation()
    {
        // Set the direction index for 8-way animation
        animator.SetFloat("spriteRot", angleToPlayer.lastIndex);
        // Set attack animation flag
        animator.SetBool("Attack", isAttacking);
    }

    /**
     * @brief Handles patrolling between waypoints.
     */
    void Patrol()
    {
        if (patrolWaypoints.Length == 0) return;

        // If close to the current waypoint, go to the next one
        if (!navAgent.pathPending && navAgent.remainingDistance < 0.5f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % patrolWaypoints.Length;
            navAgent.SetDestination(patrolWaypoints[currentWaypointIndex].position);
        }
    }
}
