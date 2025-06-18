using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    public float attackDistance;

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

    [Header("Distance Enemy Settings")]
    public Light spotLight;
    public Collider spotTrigger;
    public bool playerInSpotlight = false;
    private float distanceDamageTimer = 0f;

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

    private void OnTriggerEnter(Collider other)
    {
        if (enemyType == EnemyType.Distance && other.CompareTag("Player"))
        {
            playerInSpotlight = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (enemyType == EnemyType.Distance && other.CompareTag("Player"))
        {
            playerInSpotlight = false;
        }
    }
}
