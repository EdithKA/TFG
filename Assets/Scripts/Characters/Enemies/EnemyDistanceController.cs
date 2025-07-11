using UnityEngine;
using UnityEngine.AI;

/**
 * @brief Enemy AI: patrol, chase, attack based on distance/vision.
 */
public class EnemyDistanceController : MonoBehaviour
{
    // --- References ---
    public Transform playerTransform;      ///< Player's Transform
    Stats playerStats;                     ///< Player stats
    public AngleToPlayer angleToPlayer;    ///< Angle script (for animation)
    public Transform eyes;                 ///< Eyes for raycast
    public Animator anim;                  ///< Animator
    public Transform[] waypoints;          ///< Patrol points
    public GameObject lightCone;           ///< Vision cone

    // --- Config ---
    NavMeshAgent navMeshAgent;             ///< Agent for movement
    public float chaseDistance = 20f;      ///< Start chasing at this distance
    public float attackDistance = 15f;     ///< Start attacking at this distance
    public int damageAmount = 15;          ///< Damage per attack
    public float attackCooldown = 0.8f;    ///< Time between attacks
    public float visionAngle = 60f;        ///< FOV (not used here)
    public LayerMask obstacleLayer;        ///< Raycast mask
    int currentWaypointIndex = 0;          ///< Current patrol point
    float attackTimer = 0f;                ///< Attack cooldown timer
    public bool canChase = false;          ///< Can chase player
    public bool canAttack = false;         ///< Can attack player

    /**
     * @brief Get refs.
     */
    void Start()
    {
        playerStats = FindAnyObjectByType<Stats>();
        playerTransform = FindObjectOfType<PlayerController>().transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    /**
     * @brief Update AI, animation, and vision cone.
     */
    void Update()
    {
        NextAction();
        SetAnimation();

        // Rotate vision cone to match movement
        Vector3 moveDir = navMeshAgent.velocity.sqrMagnitude > 0.01f ? navMeshAgent.velocity.normalized : transform.forward;
        lightCone.transform.rotation = Quaternion.LookRotation(moveDir, Vector3.up);
    }

    /**
     * @brief Decide: patrol, chase, or attack.
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
            if (CanSeePlayer())
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
     * @brief Attack player if cooldown ready.
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
     * @brief Update animator params.
     */
    void SetAnimation()
    {
        anim.SetBool("Attack", canAttack);
        anim.SetFloat("spriteRot", angleToPlayer.lastIndex);
    }

    /**
     * @brief Patrol between points.
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
     * @brief Raycast to check if player is visible.
     */
    bool CanSeePlayer()
    {
        Vector3 rayOrigin = eyes.position;
        Vector3 direction = transform.forward;
        float maxDistance = chaseDistance;

        RaycastHit hit;
        bool hitSomething = Physics.Raycast(rayOrigin, direction, out hit, maxDistance, obstacleLayer);

        lightCone.gameObject.SetActive(true);
        if (hitSomething)
        {
            if (hit.collider.CompareTag("Player"))
            {
                return true;
            }
            else
            {
                lightCone.gameObject.SetActive(false);
                return false;
            }
        }
        return false;
    }
}
