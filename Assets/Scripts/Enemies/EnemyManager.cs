using System.Collections;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("[Health Settings]")]
    [SerializeField] private float health = 100f;

    [Header("[Visual Effects]")]
    [SerializeField] private float      blinkDuration = 0.3f;
    [SerializeField] private Material   deathMaterial;
    [SerializeField] private float      fadeOutDuration = 3f;

    [Header("[Knockback]")]
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
        enemyRenderer   = GetComponentInChildren<Renderer>();
        animator        = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(20f); // Debug key to trigger damage
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
        else
        {
            TriggerDamageEffects();
        }
    }

    private void TriggerDamageEffects()
    {
        StartCoroutine(Blink());
        StartCoroutine(Knockback());
    }

    private void Die()
    {
        animator.Play("Death");
        enemyController.StopAgent();
        StartCoroutine(FadeOut());
        Destroy(gameObject, fadeOutDuration);
    }

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

        for (float elapsedTime = 0; elapsedTime < knockbackTime; elapsedTime += Time.deltaTime)
        {
            transform.position = Vector3.Lerp(start, end, elapsedTime / knockbackTime);
            yield return null;
        }

        if (health > 0)
        {
            animator.Play("Hit");
            knockbackCooldown = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length + 0.5f;
            enemyAttack.SetAttackCooldown(knockbackCooldown);
        }
    }

    private IEnumerator FadeOut()
    {
        enemyRenderer.material = deathMaterial;

        for (float elapsed = 0; elapsed < fadeOutDuration; elapsed += Time.deltaTime)
        {
            float newAlpha = Mathf.Lerp(1, 0, elapsed / fadeOutDuration);
            enemyRenderer.material.color = new Color(enemyRenderer.material.color.r, enemyRenderer.material.color.g, enemyRenderer.material.color.b, newAlpha);
            yield return null;
        }
    }
}