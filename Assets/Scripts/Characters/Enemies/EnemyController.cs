using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyType { None, Melee, Distance }

public class EnemyController : MonoBehaviour
{
    public EnemyType enemyType = EnemyType.None;

    public Transform[] waypoints;
    private int currentWaypointIndex = 0;

    public float patrolSpeed = 3.5f;
    public float chaseSpeed = 5f;
    public float chaseDistance = 8f;
    public float meleeAttackDistance = 2f;
    public float rangeAttackDistance = 6f;
    public float viewAngle = 60f;

    public int meleeDamage = 25;
    public int rangeDamage = 15;
    public float attackCooldown = 1f;

    public LayerMask visionMask;
    public GameObject visionCone;
    public Transform eyes; // Asigna aquí el GameObject de los ojos en el inspector

    private float attackTimer = 0f;
    private NavMeshAgent agent;
    private Transform playerTransform;
    private Stats playerStats;
    private bool chasingPlayer = false;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        playerTransform = FindObjectOfType<PlayerController>().transform;
        playerStats = FindAnyObjectByType<Stats>();

        if (waypoints.Length > 0)
        {
            GoToRandomWaypoint();
        }
    }

    private void Update()
    {
        float playerDist = Vector3.Distance(transform.position, playerTransform.position);

        if (playerDist <= chaseDistance)
        {
            chasingPlayer = true;
        }
        else if (chasingPlayer && playerDist > chaseDistance * 1.2f)
        {
            chasingPlayer = false;
            GoToRandomWaypoint();
        }

        if (chasingPlayer)
        {
            ChasePlayer();
            TryAttackPlayer();
        }
        else
        {
            Patrol();
        }

        if (visionCone != null)
        {
            visionCone.transform.position = transform.position;
            visionCone.transform.rotation = transform.rotation;
        }
    }

    void Patrol()
    {
        agent.speed = patrolSpeed;

        if (waypoints.Length == 0) return;

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            GoToRandomWaypoint();
        }
    }

    void GoToRandomWaypoint()
    {
        if (waypoints.Length == 0) return;
        int prevIndex = currentWaypointIndex;
        do
        {
            currentWaypointIndex = Random.Range(0, waypoints.Length);
        } while (waypoints.Length > 1 && currentWaypointIndex == prevIndex);

        agent.SetDestination(waypoints[currentWaypointIndex].position);
    }

    void ChasePlayer()
    {
        agent.speed = chaseSpeed;
        agent.SetDestination(playerTransform.position);
    }

    void TryAttackPlayer()
    {
        if (playerTransform == null || playerStats == null) return;

        Vector3 toPlayer = playerTransform.position - transform.position;
        float distance = toPlayer.magnitude;
        float angle = Vector3.Angle(transform.forward, toPlayer);

        float attackDist = enemyType == EnemyType.Melee ? meleeAttackDistance : rangeAttackDistance;
        int damage = enemyType == EnemyType.Melee ? meleeDamage : rangeDamage;

        if (distance <= attackDist && angle <= viewAngle * 0.5f && HasLineOfSight(attackDist))
        {
            if (attackTimer <= 0f)
            {
                playerStats.TakeDamage(damage);
                attackTimer = attackCooldown;
            }
        }

        if (attackTimer > 0f)
            attackTimer -= Time.deltaTime;
    }

    bool HasLineOfSight(float distance)
    {
        if (eyes == null)
            return false;

        RaycastHit hit;
        if (Physics.Raycast(eyes.position, eyes.forward, out hit, distance, visionMask))
        {
            Debug.Log("Raycast hit: " + hit.collider.gameObject.name + " (Layer: " + LayerMask.LayerToName(hit.collider.gameObject.layer) + ")");
            return hit.collider.CompareTag("Player");
        }
        else
        {
            Debug.Log("Raycast did not hit anything.");
        }
        return false;
    }
}
