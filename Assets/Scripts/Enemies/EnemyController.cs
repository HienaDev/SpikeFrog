using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class EnemyController : MonoBehaviour
{
    [Header("[Speed Values]")]
    [SerializeField] private float patrolSpeed = 5.0f;
    [SerializeField] private float chaseSpeed = 7.0f;

    [Header("[Radius Values]")]
    [SerializeField] private float detectionRadius = 5.0f;
    [SerializeField] private float alertRadius = 10f;
    [SerializeField] private float pursuitRadius = 15f;

    [Header("[Waypoints]")]
    [SerializeField] private GameObject     waypointPrefab;
    [SerializeField] private List<Vector3>  waypointsPositions;

    private GameObject      enemiesWaypointsParent;
    private PlayerHealth    playerHealth;
    private Transform       player;
    private int             waypointIndex;
    private Vector3         initialPosition;
    private EnemyState      currentState;
    private List<Transform> waypoints;
    private EnemyAttack     enemyAttack;
    private NavMeshAgent    agent;
    private Animator        animator;
    private bool            lookingForPlayer;

    private void Start()
    {
        player                  = GameObject.FindGameObjectWithTag("Player").transform;
        playerHealth            = player.GetComponent<PlayerHealth>();
        waypointIndex           = 0;
        initialPosition         = transform.position;
        currentState            = EnemyState.Patrol;
        waypoints               = new List<Transform>();
        enemyAttack             = GetComponent<EnemyAttack>();
        agent                   = GetComponent<NavMeshAgent>();
        animator                = GetComponent<Animator>();
        lookingForPlayer        = true;
        enemiesWaypointsParent  = GameObject.Find("EnemiesWaypoints");
        
        CreateWaypoints();
    }

    private void Update()
    {
        DetectPlayer();

        if (!playerHealth.IsAlive)
            currentState = EnemyState.Patrol;

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
                agent.speed = 0f;
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

        if (waypoints.Count == 0)
            waypointsPositions = new List<Vector3> { initialPosition };

        if (currentState == EnemyState.Patrol)
        {
            if (!agent.pathPending && agent.remainingDistance < 0.1f)
            {
                if (waypoints != null && waypoints.Count > 0)
                {
                    waypointIndex = (waypointIndex + 1) % waypoints.Count;
                    agent.destination = waypoints[waypointIndex].position;
                }
                else
                    agent.destination = waypointsPositions[0];

            }
            else if (waypoints.Count == 0)
                agent.destination = waypointsPositions[0];
        }
    }

    private void PursuePlayer()
    {
        if (lookingForPlayer)
        {
            agent.speed = chaseSpeed;
            agent.destination = player.position;
            animator.SetBool("isChasing", true);
        }
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

    private void CreateWaypoints()
    {
        foreach (Vector3 pos in waypointsPositions)
        {
            //Instantiate waypoints inside the parent object
            GameObject waypoint = Instantiate(waypointPrefab, pos, Quaternion.identity, enemiesWaypointsParent.transform);
            waypoints.Add(waypoint.transform);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, alertRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pursuitRadius);        

        // Draw waypoints
        Gizmos.color = Color.magenta;
        foreach (Vector3 pos in waypointsPositions)
        {
            Gizmos.DrawSphere(pos, 0.4f);
        }
    }

    public Vector3 GetPlayerPosition() => player.position;

    public void StopAgent() => agent.isStopped = true;

    public void ResumeAgent() => agent.isStopped = false;

    public bool IsAgentStopped() => agent.isStopped;
    
    public void SetLookingForPlayer(bool value) => lookingForPlayer = value;

    [System.Serializable]
    public struct SaveData
    {
        public Vector3    position;
        public Quaternion rotation;
        public int        nextWaypointIndex;
        public bool       agentIsStopped;
        public Vector3    agentDestination;
        public Vector3    agentVelocity;
        public bool       active;
    }

    public SaveData GetSaveData()
    {
        SaveData saveData;

        saveData.position          = transform.position;
        saveData.rotation          = transform.rotation;
        saveData.nextWaypointIndex = waypointIndex;
        if (gameObject.activeSelf)
            saveData.agentIsStopped = agent.isStopped;
        else
            saveData.agentIsStopped = false;
        saveData.agentDestination  = agent.destination;
        saveData.agentVelocity     = agent.velocity;
        saveData.active            = gameObject.activeSelf;

        return saveData;
    }

    public void LoadSaveData(SaveData saveData)
    {
        transform.position = saveData.position;
        transform.rotation = saveData.rotation;
        waypointIndex      = saveData.nextWaypointIndex;
        agent.isStopped    = saveData.agentIsStopped;
        agent.destination  = saveData.agentDestination;
        agent.velocity     = saveData.agentVelocity;
        gameObject.SetActive(saveData.active);
    }
}