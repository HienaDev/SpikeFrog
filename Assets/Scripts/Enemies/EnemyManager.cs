using System.Collections;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private float health = 100f;
    [SerializeField] private float blinkDuration = 0.3f;
    [SerializeField] private float knockbackTime = 0.1f;
    [SerializeField] private float knockbackDistance = 1f;
    
    private EnemyController enemyController;
    private EnemyAttack     enemyAttack;
    private Renderer        enemyRenderer;
    private Animator        animator;
    private float           knockbackCooldown;

    private void Start()
    {
        enemyController = GetComponent<EnemyController>();
        enemyAttack     = GetComponent<EnemyAttack>();
        enemyRenderer   = GetComponent<Renderer>();
        animator        = GetComponent<Animator>();
    }

    private void Update()
    {
        // DEBUG
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(10f);
        }
    }

    public void TakeDamage(float damage)
    {
        StartCoroutine(Blink());
        StartCoroutine(Knockback());
        
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    // DOES NOT WORK WITH THE TOON SHADER
    private IEnumerator Blink()
    {
        enemyRenderer.material.color = Color.red;

        yield return new WaitForSeconds(blinkDuration);

        enemyRenderer.material.color = Color.white;
    }

    private IEnumerator Knockback()
    {
        Vector3 start = transform.position;
        Vector3 knockbackDirection = (transform.position - enemyController.GetPlayerPosition()).normalized;
        knockbackDirection.y = 0; // Ignore vertical displacement for knockback
        Vector3 end = start + knockbackDirection * knockbackDistance;

        float elapsedTime = 0;

        while (elapsedTime < knockbackTime)
        {
            transform.position = Vector3.Lerp(start, end, (elapsedTime / knockbackTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        animator.Play("Hit", -1, 0f);
        knockbackCooldown = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length + 0.5f;

        enemyAttack.AttackCooldown(knockbackCooldown);
    }
}
