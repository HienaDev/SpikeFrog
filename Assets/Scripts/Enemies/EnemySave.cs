using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySave : MonoBehaviour
{
    private EnemyManager[]      enemyManagers;
    private EnemyController[]   enemyControllers;

    // List of enemies inside the EnemySave object
    private List<GameObject>    enemies;

    // Start is called before the first frame update
    void Start()
    {
        // Take all the game object with tag "Enemy" and add them to the list in the EnemySave object
        enemies = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));

        Debug.Log("Enemies: " + enemies.Count);

        enemyManagers    = new EnemyManager[transform.childCount];
        enemyControllers = new EnemyController[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            enemyManagers[i]    = transform.GetChild(i).GetComponent<EnemyManager>();
            enemyControllers[i] = transform.GetChild(i).GetComponent<EnemyController>();
        } 
    }

    [System.Serializable]
    public struct SaveData
    {
        public EnemyManager.SaveData[] enemyManager;
        public EnemyController.SaveData[] enemyController;
    }

    public SaveData GetSaveData()
    {
        SaveData saveData;

        for (int i = 0; i < transform.childCount; i++)
        {
            if (enemyManagers[i].IsAlive)
            {
                enemyManagers[i] = transform.GetChild(i).GetComponent<EnemyManager>();
                enemyControllers[i] = transform.GetChild(i).GetComponent<EnemyController>();
            }
        }

        saveData.enemyManager    = new EnemyManager.SaveData[enemyManagers.Length];
        saveData.enemyController = new EnemyController.SaveData[enemyControllers.Length];

        for (int i = 0; i < enemyManagers.Length; i++)
        {
            saveData.enemyManager[i] = enemyManagers[i].GetSaveData();
            saveData.enemyController[i] = enemyControllers[i].GetSaveData();
        }

        return saveData;
    }

    public void LoadSaveData(SaveData saveData)
    {
        for (int i = 0; i < enemyManagers.Length; i++)
        {
            enemyManagers[i].LoadSaveData(saveData.enemyManager[i]);
            enemyControllers[i].LoadSaveData(saveData.enemyController[i]);
        }
    }

    public void ReloadEnemies()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].SetActive(true);
            enemyManagers[i].ResetEnemy();
        }
    }
}
