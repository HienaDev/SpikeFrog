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
    private int waypointIndex = 0;

    private NavMeshAgent agent; 

    private static List<EnemyController> allEnemies = new List<EnemyController>();

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>(); 
        allEnemies.Add(this);
    }

    void OnDestroy()
    {
        allEnemies.Remove(this);
    }

    void Update()
    {
        switch (currentState)
        {
            case EnemyState.Patrol:
                Patrol();
                break;
            case EnemyState.Alert:
                AlertOthers();
                PursuePlayer();
                break;
            case EnemyState.Pursuit:
                PursuePlayer();
                break;
            case EnemyState.Combat:
                Combat();
                break;
        }

        DetectPlayer();
    }

    void Patrol()
    {
        if (waypoints.Length == 0) return;

        if (!agent.pathPending && agent.remainingDistance < 0.1f)
        {
            agent.destination = waypoints[waypointIndex].position;
            waypointIndex = (waypointIndex + 1) % waypoints.Length;
        }
    }

    void PursuePlayer()
    {
        agent.destination = player.position;
    }

    void DetectPlayer()
    {
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);
        
        if (distanceToPlayer <= detectionRadius)
        {
            currentState = EnemyState.Pursuit;
            Debug.Log("Player detected");
        }
        else if (distanceToPlayer > pursuitRadius)
        {
            currentState = EnemyState.Patrol;
            Debug.Log("Lost player");
        }
    }

    void AlertOthers()
    {
        foreach (var enemy in allEnemies)
        {
            if (enemy != this && Vector3.Distance(transform.position, enemy.transform.position) <= alertRadius)
            {
                enemy.currentState = EnemyState.Alert;
            }
        }
    }

    void Combat()
    {
        // Placeholder for combat logic
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