using UnityEngine;

public class Debugger : MonoBehaviour
{
    private PlayerHealth   playerHealth;
    private PlayerMovement playerMovement;
    private DealDamage     dealDamage;

    void Start()
    {
        playerHealth   = GetComponent<PlayerHealth>();
        playerMovement = GetComponent<PlayerMovement>();
        dealDamage     = GetComponent<DealDamage>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha1))
        {
            playerHealth.FullHealth();
        }

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha2))
        {
            playerHealth.Regen(10);
        }

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha3))
        {
            playerHealth.Damage(10);
        }

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha4))
        {
            playerMovement.SetVelocity(50);
        }

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha5))
        {
            dealDamage.SetDamage(100);
        }

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha6))
        {
            foreach (EnemyController enemy in FindObjectsOfType<EnemyController>())
            {
                enemy.SetLookingForPlayer(false);
            }
        }

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha7))
        {
            foreach (EnemyController enemy in FindObjectsOfType<EnemyController>())
            {
                enemy.SetLookingForPlayer(true);
            }
        }
    }
}
