using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private float attackRadius = 2.5f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private Collider punchHitbox;  // Ensure this is linked in the inspector

    private float attackCooldown;
    private float lastAttackTime;

    public void AttemptAttack(Animator animator)
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            animator.Play("Punch", -1, 0f); // Play punch animation
            lastAttackTime = Time.time;
            attackCooldown = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player hit by attack.");
            lastAttackTime = Time.time;
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}