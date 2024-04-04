using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private float attackRadius = 2.5f;
    [SerializeField] private float attackCooldown = 2.0f;

    private float lastAttackTime;

    public void AttemptAttack()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            Attack();
            lastAttackTime = Time.time;
        }
    }

    void Attack()
    {
        // Attack logic here
        Debug.Log("Attacking the player.");
    }

    public float GetAttackRadius()
    {
        return attackRadius;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}