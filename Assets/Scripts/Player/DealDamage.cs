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

        LeonManager leon = other.GetComponent<LeonManager>();

        if (leon != null)
        {
            leon.TakeDamage(damage);
        }

        DestroyByAttack destroyByAttack = other.GetComponent<DestroyByAttack>();

        if (destroyByAttack != null)
        {
            destroyByAttack.Explode();
        }
    }

    public void SetDamage(int newDamage)
    {
        damage = newDamage;
    }
}
