using UnityEngine;
using UnityEngine.AI;

/**
 * @brief Controls the enemy's behavior based on the player's distance and vision.
 */
public class EnemyDistanceController : MonoBehaviour
{
    // References
    public Transform playerTransform; ///< Reference to the player's transform
    Stats playerStats; ///< Reference to the player's stats
    public AngleToPlayer angleToPlayer; ///< Reference to the angle calculation script
    public Transform eyes; ///< Enemy's eyes position for raycasting
    public Animator anim; ///< Animator for enemy animations
    public Transform[] waypoints; ///< Patrol points
    public GameObject lightCone; ///< Enemy's "ray" or vision cone

    // Agent Configuration
    NavMeshAgent navMeshAgent; ///< NavMeshAgent for movement
    public float chaseDistance = 20f; ///< Distance to start chasing the player
    public float attackDistance = 15f; ///< Distance to start attacking the player
    public int damageAmount = 15; ///< Damage dealt by the enemy
    public float attackCooldown = 0.8f; ///< Time between attacks
    public float visionAngle = 60f; ///< Enemy's field of view angle (for raycast)
    public LayerMask obstacleLayer; ///< Layers the raycast can detect
    int currentWaypointIndex = 0; ///< Current patrol waypoint index
    float attackTimer = 0f; ///< Timer for attack cooldown
    public bool canChase = false; ///< Whether the enemy can chase
    public bool canAttack = false; ///< Whether the enemy can attack

    /**
     * @brief Finds the necessary references at the start.
     */
    void Start()
    {
        playerStats = FindAnyObjectByType<Stats>();
        playerTransform = FindObjectOfType<PlayerController>().transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    /**
     * @brief Updates enemy behavior and animation every frame.
     */
    void Update()
    {
        NextAction();
        SetAnimation();

        // The enemy's "ray" always points in its current direction
        Vector3 moveDir = navMeshAgent.velocity.sqrMagnitude > 0.01f ? navMeshAgent.velocity.normalized : transform.forward;
        lightCone.transform.rotation = Quaternion.LookRotation(moveDir, Vector3.up);
    }

    /**
     * @brief Decides what the enemy should do: patrol, chase, or attack.
     */
    void NextAction()
    {
        float dist = Vector3.Distance(transform.position, playerTransform.position);
        canChase = false;
        canAttack = false;

        if (dist > chaseDistance)
        {
            Patrol();
        }
        else if (dist > attackDistance)
        {
            navMeshAgent.SetDestination(playerTransform.position);
            canChase = true;
        }
        else
        {
            if (CanSeePlayer()) // Only attacks if the player is directly seen
            {
                canAttack = true;
                navMeshAgent.ResetPath();
                Attack();
            }
            else
            {
                navMeshAgent.SetDestination(playerTransform.position);
                canChase = true;
            }
        }
    }

    /**
     * @brief Enemy attack logic with cooldown.
     */
    void Attack()
    {
        if (attackTimer <= 0f)
        {
            playerStats.TakeDamage(damageAmount);
            attackTimer = attackCooldown;
        }
        else
        {
            attackTimer -= Time.deltaTime;
        }
    }

    /**
     * @brief Sets the enemy's animation parameters.
     */
    void SetAnimation()
    {
        anim.SetBool("Attack", canAttack);
        anim.SetFloat("spriteRot", angleToPlayer.lastIndex);
    }

    /**
     * @brief Patrols between different waypoints.
     */
    void Patrol()
    {
        if (waypoints.Length == 0) return;
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.5f)
        {
            int previousIndex = currentWaypointIndex;
            do
            {
                currentWaypointIndex = Random.Range(0, waypoints.Length);
            } while (currentWaypointIndex == previousIndex && waypoints.Length > 1);
            navMeshAgent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }

    /**
     * @brief Uses a raycast to check if the enemy can see the player.
     * @return True if the enemy sees the player, false otherwise.
     */
    bool CanSeePlayer()
    {
        Vector3 rayOrigin = eyes.position; // Raycast starts from the enemy's eyes
        Vector3 direction = transform.forward;
        float maxDistance = chaseDistance; // How far the enemy can see

        RaycastHit hit;
        bool hitSomething = Physics.Raycast(rayOrigin, direction, out hit, maxDistance, obstacleLayer);

        if (hitSomething)
        {
            if (hit.collider.CompareTag("Player"))
            {
                return true; // If the raycast hits the player, return true
            }
            else
            {
                return false;
            }
        }
        return false;
    }
}
