using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("[Attack Values]")]
    [SerializeField] private float attackRadius = 2f;
    [SerializeField] private int   attackDamage = 10;

    private float attackCooldown;
    private float lastAttackTime;
    private bool  canAttack;

    public void AttemptAttack(Animator animator)
    {
        EnemyController controller = GetComponent<EnemyController>();

        if (Time.time - this.lastAttackTime >= this.attackCooldown && !controller.IsAgentStopped())
        {
            canAttack = true;
            animator.Play("Punch", -1, 0f);
            this.lastAttackTime = Time.time;
            this.attackCooldown = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && canAttack)
        {
            canAttack = false;

            this.lastAttackTime = Time.time;
            other.GetComponent<PlayerHealth>().Damage(this.attackDamage);
        }
    }

    public float GetAttackRadius() => this.attackRadius;
    public float GetAttackCooldown() => this.attackCooldown;
    public void SetAttackRadius(float radius) => this.attackRadius = radius;
    public void SetAttackCooldown(float cooldown) => this.attackCooldown = cooldown;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, this.attackRadius);
    }
}