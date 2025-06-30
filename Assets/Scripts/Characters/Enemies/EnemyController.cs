using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyType { None, Melee, Distance }

public class EnemyController : MonoBehaviour
{
    public EnemyType enemyType = EnemyType.None;
    public bool canChase;
    public bool canAttack;
    public Transform playerTransform;
    public float chaseDistance;
    public float attackDistance;
    private NavMeshAgent enemyNavMeshAgent;
    public Animator anim;
    AngleToPlayer angleToPlayer;
    public int damageAmount = 25;
    public float attackCooldown = 0.2f;
    private float attackTimer = 0f;
    private Stats playerStats;
    public LayerMask doorLayer;

    private void Start()
    {
        playerStats = FindAnyObjectByType<Stats>();
        playerTransform = FindObjectOfType<PlayerController>().transform;
        enemyNavMeshAgent = GetComponent<NavMeshAgent>();
        angleToPlayer = GetComponent<AngleToPlayer>();
    }

    private void Update()
    {
        NextAction();
        setAnimation();
        HandleAttack();
    }

    /// <summary>
    /// Checks if a closed door is blocking the path to the player
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
            if (!IsDoorBlockingPath())
            {
                canAttack = true;
            }
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

    void SetDestinationToPlayer()
    {
        enemyNavMeshAgent.SetDestination(playerTransform.position);
    }

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
