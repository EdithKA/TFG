using UnityEngine;
using UnityEngine.AI;

/**
 * @brief Melee enemy AI: patrol, chase, attack.
 */
public class EnemyMeleeController : MonoBehaviour
{
    // --- References ---
    public Transform playerTransform;      ///< Player's Transform
    Stats playerStats;                     ///< Player stats
    public AngleToPlayer angleToPlayer;    ///< Angle script (for animation)
    public Transform eyes;                 ///< Eyes for raycast
    public Animator anim;                  ///< Animator
    public Transform[] waypoints;          ///< Patrol points

    // --- Config ---
    NavMeshAgent navMeshAgent;             ///< Agent for movement
    public float chaseDistance = 10f;      ///< Start chasing at this distance
    public float attackDistance = 4f;      ///< Start attacking at this distance
    public int damageAmount = 25;          ///< Damage per attack
    public float attackCooldown = 0.5f;    ///< Time between attacks
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
     * @brief Update AI and animations.
     */
    void Update()
    {
        NextAction();
        SetAnimation();
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

        if (hitSomething)
        {
            if (hit.collider.CompareTag("Player"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }
}
