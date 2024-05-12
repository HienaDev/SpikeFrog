using UnityEngine;

public class DealDamage : MonoBehaviour
{
    private int damage = 25;

    private void OnTriggerEnter(Collider other)
    {
        EnemyManager enemy = other.GetComponent<EnemyManager>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }
    }

    public void SetDamage(int newDamage)
    {
        damage = newDamage;
    }
}
