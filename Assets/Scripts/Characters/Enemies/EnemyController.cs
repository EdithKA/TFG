using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

/**
 * @brief This class controls the mechanics that correspond to enemies.
 */
public class EnemyController : MonoBehaviour
{
    public bool canChase;
    public bool canAttack;
    public Transform playerTransform;
    private NavMeshAgent enemyNavMeshAgent;
    public float chaseDistance;
    public float attackDistance;


    /// Animation.
    public Animator anim;
    AngleToPlayer angleToPlayer;

    /// Patrol.
    public Transform[] waypoints; // Array de waypoints
    private int currentWaypointIndex = 0;

    private void Start()
    {
        playerTransform = FindObjectOfType<PlayerController>().transform;
        enemyNavMeshAgent = GetComponent<NavMeshAgent>();
        angleToPlayer = GetComponent<AngleToPlayer>();

        if (waypoints.Length > 0)
        {
            enemyNavMeshAgent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }

    private void Update()
    {

        NextAction();
        setAnimation();
       
    }

    /**
     * @brief It is used to determine what the enemy's following action will be depending on his distances to the player.
     */
    void NextAction()
    {

        float dist = Vector3.Distance(transform.position, playerTransform.position);
        //Debug.Log(dist);
        canAttack = false;
        if(dist > chaseDistance) //Patrullar
        {
            Patrol();
        }
        else if (dist > attackDistance) //Atacar
        {
            SetDestinationToPlayer();
        }
        else
        {
            canAttack = true;
        }
    }

    /**
     * @brief Assigns the animation that corresponds to the enemy.
     */
    void setAnimation()
    {
        anim.SetFloat("spriteRot", angleToPlayer.lastIndex);
        anim.SetBool("Attack", canAttack);
    }
    

    /**
     * @brief Assign the player's location as destination.
     */
    void SetDestinationToPlayer()
    {
        enemyNavMeshAgent.SetDestination(playerTransform.position);
    }

    /**
     * @brief Implants the patrol mechanics of the enemy.
     */
    void Patrol()
    {
        if (waypoints.Length == 0) return;

        if (!enemyNavMeshAgent.pathPending && enemyNavMeshAgent.remainingDistance < 0.5f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            enemyNavMeshAgent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }
}
