using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float detectionRadius = 5.0f;
    [SerializeField] private float alertRadius = 10f;
    [SerializeField] private float pursuitRadius = 15f;

    private EnemyState currentState = EnemyState.Patrol;
    private int waypointIndex;
    private NavMeshAgent agent; 
    private EnemyAttack enemyAttack;

    void Start()
    {
        waypointIndex = 0;
        enemyAttack = GetComponent<EnemyAttack>();
        agent = GetComponent<NavMeshAgent>(); 
    }

    void Update()
    {
        DetectPlayer();

        switch (currentState)
        {
            case EnemyState.Patrol:
                Patrol();
                break;
            case EnemyState.Pursuit:
                AlertOthers();
                PursuePlayer();
                break;
            case EnemyState.Combat:
                Combat();
                break;
        }
    }

    void Patrol()
    {
        agent.isStopped = false;

        if (waypoints.Length == 0) return;

        if (currentState == EnemyState.Patrol)
        {
            if (!agent.pathPending && agent.remainingDistance < 0.1f)
            {
                waypointIndex = (waypointIndex + 1) % waypoints.Length;
                agent.destination = waypoints[waypointIndex].position;
            }
            else if (agent.destination != waypoints[waypointIndex].position)
            {
                agent.destination = waypoints[waypointIndex].position;
            }
        }
    }

    void PursuePlayer()
    {
        agent.isStopped = false;
        agent.destination = player.position;
    }

    void DetectPlayer()
    {
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);
        
        if (distanceToPlayer <= enemyAttack.GetAttackRadius())
        {
            currentState = EnemyState.Combat;
        }
        else if (distanceToPlayer <= detectionRadius)
        {
            currentState = EnemyState.Pursuit;
        }
        else if (distanceToPlayer >= pursuitRadius)
        {
            currentState = EnemyState.Patrol;
        }
    }

    void AlertOthers()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, alertRadius);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy") && hitCollider.gameObject != this.gameObject)
            {
                EnemyController enemyController = hitCollider.GetComponent<EnemyController>();

                if (enemyController != null && enemyController.currentState != EnemyState.Pursuit)
                {
                    enemyController.currentState = EnemyState.Pursuit;
                }
            }
        }
    }

    void Combat()
    {
        agent.isStopped = true;
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
        enemyAttack.AttemptAttack();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, alertRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pursuitRadius);
    }
}