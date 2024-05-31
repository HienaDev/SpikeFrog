using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class LeonAttack : MonoBehaviour
{
    [Header("[Electric Claw Settings]")]
    [SerializeField] private int     electricClawDamage = 15;
    [SerializeField] private float   radiusElectricClaw = 2.5f;
    [SerializeField] private Vector3 electricClawBoxSize = new Vector3(5.0f, 2.0f, 5.0f);
    

    [Header("[Electro Roar Settings]")]
    [SerializeField] private int        electroRoarDamage = 10;
    [SerializeField] private float      radiusElectroRoar = 5.0f;
    [SerializeField] private GameObject electroRoarBallPrefab;
    [SerializeField] private float      electroRoarExpandSpeed = 1.0f;

    [Header("[Attack Cooldown]")]
    [SerializeField] private float attackCooldown = 2.0f;
    
    private PlayerHealth    playerHealth;
    private NavMeshAgent    agent;
    private Animator        animator;
    private bool            isElectroRoarActive;
    private bool            isElectricClawActive;

    public float AttackCooldown => attackCooldown;
    public bool IsOnAttack { get; private set; }

    void Start()
    {
        playerHealth         = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        agent                = GetComponent<NavMeshAgent>();
        animator             = GetComponentInChildren<Animator>();
        isElectroRoarActive  = false;
        isElectricClawActive = false;
        IsOnAttack           = false;
    }

    void Update()
    {
        // If pressed E execute Electro Roar
        if (Input.GetKeyDown(KeyCode.E))
        {
            ExecuteAttack(LeonAttackType.ElectricClaw, true);
        }

        // If pressed F execute Electric Claw
        if (Input.GetKeyDown(KeyCode.F))
        {
            ExecuteAttack(LeonAttackType.ElectroRoar, true);
        }
    }

    public (LeonAttackType, float) SelectRandomAttack()
    {
        int randomAttack = Random.Range(0, 2);

        if (randomAttack == 0)
        {
            return (LeonAttackType.ElectricClaw, radiusElectricClaw);
        }
        else
        {
            return (LeonAttackType.ElectroRoar, radiusElectroRoar);
        }
    }

    public void ExecuteAttack(LeonAttackType attackType, bool isControlled)
    {
        if (attackType == LeonAttackType.ElectricClaw)
        {
            AttemptElectricClaw(isControlled);
        }
        else if (attackType == LeonAttackType.ElectroRoar)
        {
            AttemptElectroRoar(isControlled);
        }
    }

    private void AttemptElectricClaw(bool isControlled)
    {
        if (!isElectricClawActive)
        {   
            animator.SetTrigger("ClawTrigger");
            StartCoroutine(ElectricClawRoutine(isControlled));
        }
    }

    private IEnumerator ElectricClawRoutine(bool isControlled)
    {
        IsOnAttack = true;
        isElectricClawActive = true;
        agent.isStopped = true;
        agent.updateRotation = true;

        HashSet<Collider> damagedColliders = new HashSet<Collider>();

        yield return new WaitForSeconds(0.8f);

        // Define the box size and position in front of the character
        Vector3 boxSize = electricClawBoxSize;
        Vector3 boxCenter = transform.position + transform.forward * (boxSize.z / 2);

        Collider[] hitColliders = Physics.OverlapBox(boxCenter, boxSize / 2, transform.rotation);
        foreach (var hitCollider in hitColliders)
        {
            if (!damagedColliders.Contains(hitCollider))
            {
                if (isControlled && hitCollider.CompareTag("Player"))
                {
                    if (playerHealth != null)
                    {
                        playerHealth.Damage(electricClawDamage);
                        damagedColliders.Add(hitCollider);
                        Debug.Log("Hit player with Electric Claw");
                    }
                }
                else if (!isControlled && hitCollider.CompareTag("Enemy"))
                {
                    Debug.Log("Hit enemy with Electric Claw");
                    EnemyManager enemyManager = hitCollider.GetComponent<EnemyManager>();

                    if (enemyManager != null)
                    {
                        enemyManager.TakeDamage(electricClawDamage);
                        damagedColliders.Add(hitCollider);
                    }
                }
                else
                    yield return null;
            }
        }

        yield return new WaitForSeconds(0.7f);
        
        isElectricClawActive = false;
        agent.isStopped = false;
        animator.ResetTrigger("ClawTrigger");
        IsOnAttack = false;
    }

    private void AttemptElectroRoar(bool isControlled)
    {
        if (!isElectroRoarActive)
        {
            animator.SetTrigger("RoarTrigger");
            StartCoroutine(ElectroRoarRoutine(isControlled));
        }
    }

    private IEnumerator ElectroRoarRoutine(bool isControlled)
    {
        IsOnAttack = true;
        isElectroRoarActive = true;
        agent.isStopped = true;

        GameObject roarBall = Instantiate(electroRoarBallPrefab, transform.position, Quaternion.identity);
        roarBall.transform.SetParent(transform);

        Vector3 initialScale = new Vector3(0, 0, 0);
        float maxScale = radiusElectroRoar;

        HashSet<Collider> damagedColliders = new HashSet<Collider>();

        while (roarBall.transform.localScale.x < maxScale ||
               roarBall.transform.localScale.y < maxScale ||
               roarBall.transform.localScale.z < maxScale)
        {
            float scaleIncrement = electroRoarExpandSpeed * Time.deltaTime;
            Vector3 newScale = roarBall.transform.localScale + Vector3.one * scaleIncrement;

            newScale.x = Mathf.Min(newScale.x, maxScale);
            newScale.y = Mathf.Min(newScale.y, maxScale);
            newScale.z = Mathf.Min(newScale.z, maxScale);

            roarBall.transform.localScale = newScale;
            roarBall.transform.position = transform.position;

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, newScale.x);

            foreach (var hitCollider in hitColliders)
            {
                if (!damagedColliders.Contains(hitCollider))
                {
                    if (isControlled && hitCollider.CompareTag("Player"))
                    {
                        if (playerHealth != null)
                        {
                            playerHealth.Damage(electroRoarDamage);
                            damagedColliders.Add(hitCollider);
                        }
                    }
                    else if (!isControlled && hitCollider.CompareTag("Enemy"))
                    {
                        Debug.Log("Hit enemy by Electro Roar");
                        EnemyManager enemyManager = hitCollider.GetComponent<EnemyManager>();

                        if (enemyManager != null)
                        {
                            enemyManager.TakeDamage(electroRoarDamage);
                            damagedColliders.Add(hitCollider);
                        }
                    }
                }
            }

            yield return null;
        }

        yield return new WaitForSeconds(1.5f);

        Destroy(roarBall);
        isElectroRoarActive = false;
        agent.isStopped = false;
        animator.ResetTrigger("RoarTrigger");
        IsOnAttack = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, this.radiusElectroRoar);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, this.radiusElectricClaw);

        Gizmos.color = Color.red;
        Vector3 boxSize = electricClawBoxSize;
        Vector3 boxCenter = transform.position + transform.forward * (boxSize.z / 2);
        Gizmos.DrawWireCube(boxCenter, boxSize);
    }
}