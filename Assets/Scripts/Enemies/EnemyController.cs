using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float patrolSpeed = 5.0f;
    [SerializeField] private float chaseSpeed = 7.0f;
    [SerializeField] private float detectionRadius = 5.0f;
    [SerializeField] private float alertRadius = 10f;
    [SerializeField] private float pursuitRadius = 15f;
    
    private EnemyState      currentState;
    private int             waypointIndex;
    private NavMeshAgent    agent;
    private bool punching;
    private EnemyAttack     enemyAttack;
    private Animator        animator;
    private EnemyManager    enemyManager;

    private void Start()
    {
        waypointIndex = 0;
        currentState = EnemyState.Patrol;
        enemyAttack = GetComponent<EnemyAttack>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        enemyManager = GetComponent<EnemyManager>();
    }

    private void Update()
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

    private void Patrol()
    {
        agent.speed = patrolSpeed;

        animator.SetBool("isChasing", false);

        if (agent.velocity.magnitude > 0)
        {
            animator.SetBool("isMoving", true);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }

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

    private void PursuePlayer()
    {
        agent.speed = chaseSpeed;
        agent.destination = player.position;
        animator.SetBool("isChasing", true);
    }

    private void DetectPlayer()
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

    private void AlertOthers()
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

    private void Combat()
    {
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
        enemyAttack.AttemptAttack(animator);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, alertRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pursuitRadius);
    }

    public Vector3 GetPlayerPosition()
    {
        return player.position;
    }

    public void StopAgent()
    {
        agent.isStopped = true;
    }

    public void ResumeAgent()
    {
        agent.isStopped = false;
    }

    public bool AgentIsStopped()
    {
        return agent.isStopped;
    }
}