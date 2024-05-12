using UnityEngine;

public class Debugger : MonoBehaviour
{
    private PlayerHealth   playerHealth;
    private PlayerMovement playerMovement;
    [SerializeField] private DealDamage[] dealDamages;

    void Start()
    {
        playerHealth   = GetComponent<PlayerHealth>();
        playerMovement = GetComponent<PlayerMovement>();
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

        // Set damage
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha4))
        {
            foreach (DealDamage dealDamage in dealDamages)
            {
                dealDamage.SetDamage(100);
            }
        }
        
        // Stop enemies from looking for player
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha5))
        {
            foreach (EnemyController enemy in FindObjectsOfType<EnemyController>())
            {
                enemy.SetLookingForPlayer(false);
            }
        }

        // Make enemies look for player
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha6))
        {
            foreach (EnemyController enemy in FindObjectsOfType<EnemyController>())
            {
                enemy.SetLookingForPlayer(true);
            }
        }

        // Kill Player
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha7))
        {
            playerHealth.Damage(100);
        }

        // Toggle Invulnerable to damage Player
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha8))
        {
            playerHealth.ToggleDamageable();
        }

    }
}
