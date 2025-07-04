using UnityEngine;
using UnityEngine.AI;

public class EnemyDistanceController : MonoBehaviour
{
    // Referencias
    public Transform playerTransform;
    public AngleToPlayer angleToPlayer;
    public Transform eyes;

    // Configuración navmesh Agent
    public float chaseDistance = 20f; // Distancia para perseguir al jugador
    public float attackDistance = 15f; // Distancia para atacar al jugador
    public float visionAngle = 60f;
    public LayerMask obstacleLayer;
    public int damageAmount = 15;
    public float attackCooldown = 0.8f;
    public Animator anim;
    public Transform[] waypoints;
    public GameObject lightCone;

    private NavMeshAgent navMeshAgent;
    private Stats playerStats;
    private int currentWaypointIndex = 0;
    private float attackTimer = 0f;
    public bool canChase = false;
    public bool canAttack = false;

    void Start()
    {
        playerStats = FindAnyObjectByType<Stats>();
        playerTransform = FindObjectOfType<PlayerController>().transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (waypoints.Length > 0)
        {
            navMeshAgent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }

    void Update()
    {
        NextAction();
        SetAnimation();
        HandleAttack();

        if (lightCone != null)
        {
            Vector3 moveDir = navMeshAgent.velocity.sqrMagnitude > 0.01f
                ? navMeshAgent.velocity.normalized
                : transform.forward;
            lightCone.transform.rotation = Quaternion.LookRotation(moveDir, Vector3.up);
        }
    }

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
            SetDestinationToPlayer();
            canChase = true;
        }
        else
        {
            if (CanSeePlayer())
            {
                canAttack = true;
                navMeshAgent.ResetPath();
            }
            else
            {
                SetDestinationToPlayer();
                canChase = true;
            }
        }
    }

    bool CanSeePlayer()
    {
        Vector3 rayOrigin = eyes.position;
        Vector3 direction = transform.forward; 
        float maxDistance = attackDistance;


        RaycastHit hit;
        bool hitSomething = Physics.Raycast(rayOrigin, direction, out hit, maxDistance, obstacleLayer);

        Debug.DrawLine(rayOrigin, rayOrigin + direction * maxDistance, Color.blue, 0.1f);

        if (hitSomething)
        {
            if (hit.collider.CompareTag("Player"))
            {
                Debug.DrawLine(rayOrigin, hit.point, Color.green, 0.15f);
                return true;
            }
            else
            {
                Debug.DrawLine(rayOrigin, hit.point, Color.yellow, 0.15f);
                return false;
            }
        }
        return false;
    }

    void HandleAttack()
    {
        if (canAttack)
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
        else
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer < 0f) attackTimer = 0f;
        }
    }


    void SetAnimation()
    {

        anim.SetBool("Attack", canAttack);
        anim.SetFloat("spriteRot", angleToPlayer.lastIndex);

    }

    void SetDestinationToPlayer()
    {
        navMeshAgent.SetDestination(playerTransform.position);
    }

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
}
