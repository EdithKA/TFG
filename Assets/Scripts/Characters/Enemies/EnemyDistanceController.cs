using UnityEngine;
using UnityEngine.AI;

public class EnemyDistanceController : MonoBehaviour
{
    // Referencias
    public Transform playerTransform;
    Stats playerStats;
    public AngleToPlayer angleToPlayer;
    public Transform eyes;
    public Animator anim;
    public Transform[] waypoints; // Puntos para el patrullaje
    public GameObject lightCone; // "Rayo" del enemigo


    // Configuración del Agent
    NavMeshAgent navMeshAgent;
    public float chaseDistance = 20f; // Distancia para perseguir al jugador
    public float attackDistance = 15f; // Distancia para atacar al jugador
    public int damageAmount = 15; // Daño que hace el enemigo
    public float attackCooldown = 0.8f; // Tiempo entre ataques
    public float visionAngle = 60f; // Ángulo de visión del enemigo (para el raycast)
    public LayerMask obstacleLayer; // Capas que el raycast puede detectar
    int currentWaypointIndex = 0;
    float attackTimer = 0f;
    public bool canChase = false;
    public bool canAttack = false;

    // Busca las referencias necesarias al iniciar
    void Start()
    {
        playerStats = FindAnyObjectByType<Stats>();
        playerTransform = FindObjectOfType<PlayerController>().transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        NextAction();
        SetAnimation();

        // El "rayo" del enemigo va siempre en la dirección de éste
        Vector3 moveDir = navMeshAgent.velocity.sqrMagnitude > 0.01f ? navMeshAgent.velocity.normalized: transform.forward;
        lightCone.transform.rotation = Quaternion.LookRotation(moveDir, Vector3.up);
    }

    // Decide qué debe de hacer el enemigo: Patrullar, perseguir o atacar
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
            if (CanSeePlayer()) //Solo ataca al jugador si lo ve directamente
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

    // Lógica de ataque del enemigo con un tiempo entre ataques
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


    void SetAnimation()
    {
        anim.SetBool("Attack", canAttack);
        anim.SetFloat("spriteRot", angleToPlayer.lastIndex);
    }

    // Lógica de patrulla entre diferentes puntos
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

    // Utilizando un raycast, comprueba si el enemigo ve al jugador
    bool CanSeePlayer()
    {
        Vector3 rayOrigin = eyes.position; // El raycast sale desde los ojos del enemigo
        Vector3 direction = transform.forward;
        float maxDistance = chaseDistance; //  Cómo de lejos ve el enemigo

        RaycastHit hit;
        bool hitSomething = Physics.Raycast(rayOrigin, direction, out hit, maxDistance, obstacleLayer);

        if (hitSomething)
        {
            if (hit.collider.CompareTag("Player"))
            {
                return true; //Si el raycast colisiona con el jugador, devuelve true
            }
            else
            {
                return false;
            }
        }
        return false;
    }
}
