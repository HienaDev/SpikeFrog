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

        if (Time.time - lastAttackTime >= attackCooldown && !controller.AgentIsStopped())
        {
            animator.Play("Punch", -1, 0f);
            lastAttackTime = Time.time;
            attackCooldown = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            lastAttackTime = Time.time;
            //other.GetComponent<PlayerHealth>().TakeDamage(attackDamage);
        }
    }

    public float GetAttackRadius()
    {
        return attackRadius;
    }

    public float GetAttackCooldown()
    {
        return attackCooldown;
    }

    public void AttackCooldown(float cooldown)
    {
        attackCooldown = cooldown;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}