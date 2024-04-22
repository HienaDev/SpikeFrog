using System.Collections;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("[Health]")]
    [SerializeField] private float health = 100f;

    [Header("[Blinking]")]
    [SerializeField] private float blinkDuration = 0.3f;

    [Header("[Knockback]")]
    [SerializeField] private float knockbackTime = 0.1f;
    [SerializeField] private float knockbackDistance = 1f;

    [Header("[Death]")]
    [SerializeField] private float      fadeOutDuration = 3f;
    [SerializeField] private Material   deathMaterial;
    
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
            TakeDamage(100f);
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        StartCoroutine(Blink());
        StartCoroutine(Knockback());

        if (health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        animator.Play("Death", -1, 0f);
        enemyController.StopAgent();
        StartCoroutine(FadeOut());
        Destroy(gameObject, fadeOutDuration);
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

        if (health > 0)
        {
            animator.Play("Hit", -1, 0f);
            knockbackCooldown = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length + 0.5f;
        }
        
        enemyAttack.AttackCooldown(knockbackCooldown);
    }

    private IEnumerator FadeOut()
    {
        enemyRenderer.material = deathMaterial;

        float elapsed = 0;
        float startAlpha = enemyRenderer.material.color.a;

        while (elapsed < fadeOutDuration)
        {
            float newAlpha = Mathf.Lerp(startAlpha, 0, elapsed / fadeOutDuration);
            enemyRenderer.material.color = new Color(enemyRenderer.material.color.r, 
                                                    enemyRenderer.material.color.g, 
                                                    enemyRenderer.material.color.b, 
                                                    newAlpha);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}
