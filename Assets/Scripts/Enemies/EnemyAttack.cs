using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private float attackRadius = 2.5f;
    [SerializeField] private float attackDamage = 10f;

    private float attackCooldown;
    private float lastAttackTime;

    public void AttemptAttack(Animator animator)
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            Attack(animator);
            lastAttackTime = Time.time;
        }
    }

    private void Attack(Animator animator)
    {
        //Debug.Log("Attacking the player.");
        animator.Play("Punch",  -1, 0f);
        attackCooldown = animator.GetCurrentAnimatorClipInfo(0).Length;
    }

    public float GetAttackRadius()
    {
        return attackRadius;
    }

    public float GetAttackCooldown()
    {
        return attackCooldown;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}