using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class LeonController : MonoBehaviour
{
    [Header("[Speed Values]")]
    [SerializeField] private float speed = 5.0f;

    [Header("[Radii Values]")]
    [SerializeField] private float enemieCloseRadius = 5.0f;
    [SerializeField] private float spikeFollowRadius = 5.0f;
    [SerializeField] private float tooCloseRadius = 3.0f;

    [Header("[Stunned Values]")]
    [SerializeField] private float stunnedTime = 3.0f;

    private float          attackCooldownTimer;
    private LeonState      currentState;
    private LeonManager    leonManager;
    private LeonAttack     leonAttack;
    private NavMeshAgent   agent;
    private Transform      player;
    private Animator       animator;
    private bool           isAttackSelected;
    private LeonAttackType selectedAttackType;
    private float          attackRadius;
    private bool           isStunned;

    public bool IsOnAttack { get; set; }

    private List<Renderer> rendererers;
    
    void Start()
    {
        attackCooldownTimer = 0;
        leonManager         = GetComponent<LeonManager>();
        leonAttack          = GetComponent<LeonAttack>();
        agent               = GetComponent<NavMeshAgent>();
        player              = GameObject.FindGameObjectWithTag("Player").transform;
        animator            = GetComponentInChildren<Animator>();
        isAttackSelected    = false;
        currentState        = LeonState.Controlled;
        isStunned           = false; 

        agent.speed         = speed;

        rendererers = new List<Renderer>();

        foreach(Renderer renderer in transform.GetComponentsInChildren<Renderer>())
        {
            rendererers.Add(renderer);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            currentState = LeonState.Stunned;
            Debug.Log("Leon is stunned");
        }

        switch (currentState)
        {
            case LeonState.Stunned:
                if (!isStunned)
                {
                    StartCoroutine(Stunned());
                    foreach (Renderer renderer in rendererers)
                    {
                        renderer.material.color = Color.red;
                    }
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
        isStunned = true; 
        agent.isStopped = true;
        animator.SetTrigger("StunnedTrigger");
        animator.SetBool("isMoving", false);

        yield return new WaitForSeconds(stunnedTime);

        isStunned = false; 
        agent.isStopped = false;

        if (currentState == LeonState.Stunned)
        {
            if (leonManager.HaveTheControllerOnLeon)
            {
                currentState = LeonState.Controlled;
            }
            else if (!leonManager.HaveTheControllerOnLeon)
            {
                currentState = LeonState.NotControlled;
            }
        }

        foreach (Renderer renderer in rendererers)
        {
            renderer.material.color = Color.white;
        }
    }

    private void PursueAndAttackSpike()
    {
        animator.SetBool("isMoving", true); 

        float distance = Vector3.Distance(transform.position, player.position);

        agent.SetDestination(player.position);

        if (attackCooldownTimer > 0)
        {
            if (!leonAttack.IsOnAttack)
                attackCooldownTimer -= Time.deltaTime;
            
            if (distance <= tooCloseRadius && !leonAttack.IsOnAttack)
            {
                agent.isStopped = true;
                animator.SetBool("isMoving", false);
            }
            else if (!leonAttack.IsOnAttack)
            {
                agent.isStopped = false;
                animator.SetBool("isMoving", true);
                agent.SetDestination(player.position);
            }

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

            agent.SetDestination(nearestEnemy.position);

            if (attackCooldownTimer > 0)
            {
                attackCooldownTimer -= Time.deltaTime;
                
                if (distanceToEnemy <= tooCloseRadius && !leonAttack.IsOnAttack)
                {
                    agent.isStopped = true;
                    animator.SetBool("isMoving", false);
                }
                else if (!leonAttack.IsOnAttack)
                {
                    agent.isStopped = false;
                    animator.SetBool("isMoving", true);
                    agent.SetDestination(nearestEnemy.position);
                }

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
        }
        else if (!leonAttack.IsOnAttack)
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

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, tooCloseRadius);
    }

    public void SetNotControlled()
    {
        currentState = LeonState.NotControlled;
    }

    public LeonState CurrentState => currentState;

    [System.Serializable]
    public struct SaveData
    {
        public Vector3        position;
        public Quaternion     rotation;
        public float          attackCooldownTimer;
        public LeonState      currentState;
        public bool           isAttackSelected;
        public LeonAttackType selectedAttackType;
        public float          attackRadius;
        public bool           isStunned;
        public int            animationState;
        public float          animationTime;
    }

    public SaveData GetSaveData()
    {
        SaveData saveData;

        saveData.position            = transform.position;
        saveData.rotation            = transform.rotation;
        saveData.attackCooldownTimer = attackCooldownTimer;
        saveData.currentState        = currentState;
        saveData.isAttackSelected    = isAttackSelected;
        saveData.selectedAttackType  = selectedAttackType;
        saveData.attackRadius        = attackRadius;
        saveData.isStunned           = isStunned;
        saveData.animationState      = animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
        saveData.animationTime       = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        return saveData;
    }

    public void LoadSaveData(SaveData saveData)
    {
        transform.position            = saveData.position;
        transform.rotation            = saveData.rotation;
        attackCooldownTimer           = saveData.attackCooldownTimer;
        currentState                  = saveData.currentState;
        isAttackSelected              = saveData.isAttackSelected;
        selectedAttackType            = saveData.selectedAttackType;
        attackRadius                  = saveData.attackRadius;
        isStunned                     = saveData.isStunned;
        animator.Play(saveData.animationState, 0, saveData.animationTime);
    }
}