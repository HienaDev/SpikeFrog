using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("[Attack Values]")]
    [SerializeField] private float attackRadius = 2.5f;
    [SerializeField] private float attackDamage = 10f;

    private float attackCooldown;
    private float lastAttackTime;

    public void AttemptAttack(Animator animator)
    {
        EnemyController controller = GetComponent<EnemyController>();

        if (Time.time - this.lastAttackTime >= this.attackCooldown && !controller.IsAgentStopped())
        {
            animator.Play("Punch", -1, 0f);
            this.lastAttackTime = Time.time;
            this.attackCooldown = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            this.lastAttackTime = Time.time;
            // other.GetComponent<PlayerHealth>().TakeDamage(this.attackDamage);
        }
    }

    public float GetAttackRadius() => this.attackRadius;

    public float GetAttackCooldown() => this.attackCooldown;

    public void SetAttackCooldown(float cooldown) => this.attackCooldown = cooldown;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, this.attackRadius);
    }
}