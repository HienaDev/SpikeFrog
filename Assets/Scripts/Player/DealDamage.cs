using UnityEngine;

public class DealDamage : MonoBehaviour
{
    private int damage = 25;
    private PlayerSounds playerSounds;


    private void Start()
    {
        playerSounds = GetComponentInParent<PlayerSounds>();

    }

    private void OnTriggerEnter(Collider other)
    {
        EnemyManager enemy = other.GetComponent<EnemyManager>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            playerSounds.PlayPunchSound();
        }

        LeonManager leon = other.GetComponent<LeonManager>();

        if (leon != null)
        {
            leon.TakeDamage(damage);
            playerSounds.PlayPunchSound();
        }

        DestroyByAttack destroyByAttack = other.GetComponent<DestroyByAttack>();

        if (destroyByAttack != null)
        {
            destroyByAttack.Explode();
            playerSounds.PlayPunchSound();
        }
    }

    public void SetDamage(int newDamage)
    {
        damage = newDamage;
    }
}
