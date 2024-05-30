using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class LeonController : MonoBehaviour
{
    [Header("[Speed Values]")]
    [SerializeField] private float speed = 5.0f;

    [Header("[Radii Values]")]
    [SerializeField] private float enemieCloseRadius = 5.0f;
    [SerializeField] private float spikeFollowRadius = 5.0f;

    [Header("[Stunned Values]")]
    [SerializeField] private float stunnedTime = 3.0f;

    private float          attackCooldownTimer;
    private LeonState      currentState;
    private LeonManager    leonManager;
    private LeonAttack     leonAttack;
    private NavMeshAgent   agent;
    private Transform      player;
    private PlayerHealth   playerHealth;
    private Animator       animator;
    private bool           isAttackSelected;
    private LeonAttackType selectedAttackType;
    private float          attackRadius;
    private bool           isStunned; // New flag to prevent multiple coroutine calls
    
    void Start()
    {
        attackCooldownTimer = 0;
        leonManager         = GetComponent<LeonManager>();
        leonAttack          = GetComponent<LeonAttack>();
        agent               = GetComponent<NavMeshAgent>();
        player              = GameObject.FindGameObjectWithTag("Player").transform;
        playerHealth        = player.GetComponent<PlayerHealth>();
        animator            = GetComponentInChildren<Animator>();
        isAttackSelected    = false;
        currentState        = LeonState.Controlled;
        isStunned           = false; // Initialize the flag

        agent.speed         = speed;
    }

    void Update()
    {
        if (!playerHealth.IsAlive)
        {
            agent.isStopped = true;
            return;
        }

        switch (currentState)
        {
            case LeonState.Stunned:
                if (!isStunned) // Only start coroutine if not already stunned
                {
                    StartCoroutine(Stunned());
                }
                break;
            case LeonState.Controlled:
                PursueAndAttackSpike();
                break;
            case LeonState.NotControlled:
                PursueAndAttackEnemies();
                break;
        }
    }

    private IEnumerator Stunned()
    {
        isStunned = true; // Set the flag to true when starting the coroutine
        agent.isStopped = true;
        animator.SetTrigger("StunnedTrigger");
        animator.SetBool("isMoving", false);

        yield return new WaitForSeconds(stunnedTime);

        currentState = LeonState.Controlled;
        isStunned = false; // Reset the flag when coroutine ends
        agent.isStopped = false;
    }

    private void PursueAndAttackSpike()
    {
        animator.SetBool("isMoving", true);

        float distance = Vector3.Distance(transform.position, player.position);

        if (attackCooldownTimer > 0)
        {
            attackCooldownTimer -= Time.deltaTime;
            return;
        }

        if (!isAttackSelected)
        {
            (selectedAttackType, attackRadius) = leonAttack.SelectRandomAttack();
            isAttackSelected = true;
        }

        if (distance <= attackRadius)
        {
            leonAttack.ExecuteAttack(selectedAttackType, currentState == LeonState.Controlled);
            isAttackSelected = false;
            attackCooldownTimer = leonAttack.AttackCooldown;
        }
        else if (distance > spikeFollowRadius)
        {
            agent.SetDestination(player.position);
        }
        else
        {
            agent.isStopped = true;
            animator.SetBool("isMoving", false);
        }
    }

    private void PursueAndAttackEnemies()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, enemieCloseRadius);
        Transform nearestEnemy = null;
        float nearestDistance = float.MaxValue;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                if (distance < nearestDistance)
                {
                    nearestEnemy = hitCollider.transform;
                    nearestDistance = distance;
                }
            }
        }

        if (nearestEnemy != null)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, nearestEnemy.position);

            if (attackCooldownTimer > 0)
            {
                attackCooldownTimer -= Time.deltaTime;
                return;
            }

            if (!isAttackSelected)
            {
                (selectedAttackType, attackRadius) = leonAttack.SelectRandomAttack();
                isAttackSelected = true;
            }

            if (distanceToEnemy <= attackRadius)
            {
                leonAttack.ExecuteAttack(selectedAttackType, currentState == LeonState.Controlled);
                isAttackSelected = false;
                attackCooldownTimer = leonAttack.AttackCooldown;
            }
            else
            {
                agent.SetDestination(nearestEnemy.position);
            }
        }
        else
        {
            FollowSpike();
        }
    }

    private void FollowSpike()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > spikeFollowRadius)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
            animator.SetBool("isMoving", true);
        }
        else
        {
            agent.isStopped = true;
            animator.SetBool("isMoving", false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, enemieCloseRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, spikeFollowRadius);
    }

    public LeonState CurrentState { get => currentState; set => currentState = value; }
}