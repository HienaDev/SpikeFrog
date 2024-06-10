using UnityEngine;
using System.Collections.Generic;

public class EnemiesRespawnManager : MonoBehaviour
{
    private List<EnemyManager> enemyManagers;
    private List<EnemyController> enemyControllers;
    private List<GameObject> enemies;

    void Start()
    {
        enemies = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));

        enemyManagers = new List<EnemyManager>();
        enemyControllers = new List<EnemyController>();

        foreach (var enemy in enemies)
        {
            EnemyManager manager = enemy.GetComponent<EnemyManager>();
            EnemyController controller = enemy.GetComponent<EnemyController>();

            if (manager != null)
            {
                enemyManagers.Add(manager);
            }

            if (controller != null)
            {
                enemyControllers.Add(controller);
            }
        }
    }

    public void RespawnAllEnemiesAfterCutscene()
    {
        foreach (var enemy in enemies)
        {
            enemy.SetActive(true);
        }

        foreach (var manager in enemyManagers)
        {
            manager.ResetEnemyAfterCutscene();
        }

        foreach (var controller in enemyControllers)
        {
            controller.RespawnEnemyAfterCutscene();
        }
    }
}