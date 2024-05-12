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
        // Full health player
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha1))
        {
            playerHealth.FullHealth();
        }

        // Regen player
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha2))
        {
            playerHealth.Regen(10);
        }

        // Damage player
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha3))
        {
            playerHealth.Damage(10);
        }

        // Increase player speed
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha4))
        {
            playerMovement.SetVelocity(50);
        }

        // Decrease player speed
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha5))
        {
            playerMovement.SetVelocity(10);
        }

        // Set damage
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha6))
        {
            dealDamage.SetDamage(100);
        }
        
        // Stop enemies from looking for player
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha7))
        {
            foreach (EnemyController enemy in FindObjectsOfType<EnemyController>())
            {
                enemy.SetLookingForPlayer(false);
            }
        }

        // Make enemies look for player
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha8))
        {
            foreach (EnemyController enemy in FindObjectsOfType<EnemyController>())
            {
                enemy.SetLookingForPlayer(true);
            }
        }

        // Kill Player
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha9))
        {
            playerHealth.Damage(100);
        }
    }
}
