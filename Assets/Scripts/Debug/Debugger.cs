using UnityEngine;
using UnityEngine.SceneManagement;

public class Debugger : MonoBehaviour
{
    private PlayerHealth   playerHealth;
    private LeonManager    leonManager;
    private SaveManager    saveManager;
    private TeleportToLab  teleportToLab;

    [SerializeField] private DealDamage[] dealDamages;
    [SerializeField] private GameObject[] doors;

    void Start()
    {
        playerHealth   = GetComponent<PlayerHealth>();
        saveManager    = FindObjectOfType<SaveManager>();
        leonManager    = FindObjectOfType<LeonManager>();
        teleportToLab  = FindObjectOfType<TeleportToLab>();
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

        // Destroy doors
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha9))
        {
            foreach (GameObject door in doors)
            {
                Destroy(door);
            }
        }

        // Teleport to lab
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha0))
        {
            teleportToLab.TeleportPlayer();
        }

        // Reset Level
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        // Destroy Leon Controller
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.T))
        {
            leonManager.DestroyController();
        }


        // Delete save file
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Delete))
        {
            saveManager.DeleteSaveFile();
        }
    }
}
