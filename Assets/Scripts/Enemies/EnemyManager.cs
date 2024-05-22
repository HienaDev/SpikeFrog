using System.Collections;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("[Health Settings]")]
    [SerializeField] private int health = 100;

    [Header("[Visual Effects]")]
    [SerializeField] private float    blinkDuration = 0.3f;
    [SerializeField] private Material deathMaterial;
    [SerializeField] private float    fadeOutDuration = 3f;

    [Header("[Knockback]")]
    [SerializeField] private float knockbackTime = 0.1f;
    [SerializeField] private float knockbackDistance = 1f;

    [Header("[Collider for Camera]")]
    [SerializeField] private Collider camCheckCollider;

    [Header("[Health Drop]")]
    [SerializeField] private GameObject healthDrop;
    [SerializeField] private int        chanceForHealthDrop = 50;
    [SerializeField] private GameObject healthPickupsParent;

    private EnemyController enemyController;
    private EnemyAttack     enemyAttack;
    private Renderer        enemyRenderer;
    private Animator        animator;
    private float           knockbackCooldown;

    public bool IsAlive => (health > 0);

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
            TakeDamage(20); // Debug key to trigger damage
        }
    }

    public void TakeDamage(int damage)
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



        foreach(Collider col in GetComponentsInChildren<Collider>())
        {
            col.enabled = false;
        }

        foreach (Collider col in GetComponents<Collider>())
        {
            col.enabled = false;
        }

        if(ControlCamera.instance.Targetting)
            ControlCamera.instance.SwapCameras();

        Invoke(nameof(DropHealth), fadeOutDuration - 0.1f);
        Invoke(nameof(Deactivate), fadeOutDuration);
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }

    private void DropHealth()
    {
        if ((Random.Range(0, 100) < chanceForHealthDrop))
        {
            Instantiate(healthDrop, transform.position, Quaternion.identity, healthPickupsParent.transform);
        }
        
    }

    private IEnumerator Blink()
    {
        enemyRenderer.material.SetColor("Color", Color.red);
        yield return new WaitForSeconds(blinkDuration);
        enemyRenderer.material.SetColor("Color", Color.white);
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

    [System.Serializable]
    public struct SaveData
    {
        public int   health;
        public int   animationState;
        public float animationTime;
    }

    public SaveData GetSaveData()
    {
        SaveData saveData;

        saveData.health          = health;
        saveData.animationState  = animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
        saveData.animationTime   = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        return saveData;
    }

    public void LoadSaveData(SaveData saveData)
    {
        health = saveData.health;
        animator.Play(saveData.animationState, 0, saveData.animationTime);
    }
}