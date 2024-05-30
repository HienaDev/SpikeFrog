using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class LeonAttack : MonoBehaviour
{
    [Header("[Electric Claw Settings]")]
    [SerializeField] private float  radiusElectricClaw = 2.5f;
    [SerializeField] private int    electricClawDamage = 15;
    

    [Header("[Electro Roar Settings]")]
    [SerializeField] private int        electroRoarDamage = 10;
    [SerializeField] private float      radiusElectroRoar = 5.0f;
    [SerializeField] private GameObject electroRoarBallPrefab;
    [SerializeField] private float      electroRoarExpandSpeed = 1.0f;

    [Header("[Attack Cooldown]")]
    [SerializeField] private float attackCooldown = 2.0f;
        
    private NavMeshAgent    agent;
    private Animator        animator;
    private bool            isElectroRoarActive;
    private bool            isElectricClawActive;

    public float AttackCooldown => attackCooldown;

    // Start is called before the first frame update
    void Start()
    {   
        agent               = GetComponent<NavMeshAgent>();
        animator            = GetComponent<Animator>();
        isElectroRoarActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        // If pressed E execute Electro Roar
        if (Input.GetKeyDown(KeyCode.E))
        {
            ExecuteAttack(LeonAttackType.ElectricClaw, true);
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
            Debug.Log("Electric Claw");
            StartCoroutine(ElectricClawRoutine(isControlled));
        }
    }

    private IEnumerator ElectricClawRoutine(bool isControlled)
    {
        isElectricClawActive = true;
        //animator.Play("Punch", -1, 0f);
        agent.isStopped = true;

        HashSet<Collider> damagedColliders = new HashSet<Collider>();

        yield return new WaitForSeconds(0.1f);

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radiusElectricClaw);
        foreach (var hitCollider in hitColliders)
        {
            if (!damagedColliders.Contains(hitCollider))
            {
                if (isControlled && hitCollider.CompareTag("Player"))
                {
                    Debug.Log("Hit player with Electric Claw");
                    PlayerHealth playerHealth = hitCollider.GetComponent<PlayerHealth>();
                    if (playerHealth != null)
                    {
                        playerHealth.Damage(electricClawDamage);
                        damagedColliders.Add(hitCollider);
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
            }
        }

        agent.isStopped = false;
        isElectricClawActive = false;
    }

    private void AttemptElectroRoar(bool isControlled)
    {
        if (!isElectroRoarActive)
        {
            Debug.Log("Electro Roar");
            StartCoroutine(ElectroRoarRoutine(isControlled));
        }
    }

    private IEnumerator ElectroRoarRoutine(bool isControlled)
    {
        isElectroRoarActive = true;
        //animator.Play("Roar", -1, 0f);
        agent.isStopped = true;
        GameObject roarBall = Instantiate(electroRoarBallPrefab, transform.position, Quaternion.identity);
        roarBall.transform.SetParent(transform);

        Vector3 initialScale = roarBall.transform.localScale;
        float maxScale = radiusElectroRoar;

        HashSet<Collider> damagedColliders = new HashSet<Collider>();

        while (roarBall.transform.localScale.x < maxScale ||
               roarBall.transform.localScale.y < maxScale ||
               roarBall.transform.localScale.z < maxScale)
        {
            float scaleIncrement = electroRoarExpandSpeed * Time.deltaTime;
            Vector3 newScale = roarBall.transform.localScale + Vector3.one * scaleIncrement;

            // Ensure the new scale does not exceed the maximum scale
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
                        PlayerHealth playerHealth = hitCollider.GetComponent<PlayerHealth>();

                        if (playerHealth != null)
                        {
                            playerHealth.Damage(electroRoarDamage);
                            damagedColliders.Add(hitCollider);
                        }
                    }
                    else if (isControlled && hitCollider.CompareTag("Enemy"))
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

        Destroy(roarBall);
        isElectroRoarActive = false;
        agent.isStopped = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, this.radiusElectroRoar);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, this.radiusElectricClaw);
    }
}