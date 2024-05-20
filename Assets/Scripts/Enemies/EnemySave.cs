using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySave : MonoBehaviour
{
    private EnemyManager[] enemyManagers;
    private EnemyController[] enemyControllers;

    // Start is called before the first frame update
    void Start()
    {
        enemyManagers = new EnemyManager[transform.childCount];
        enemyControllers = new EnemyController[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            enemyManagers[i] = transform.GetChild(i).GetComponent<EnemyManager>();
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
            enemyManagers[i] = transform.GetChild(i).GetComponent<EnemyManager>();
            enemyControllers[i] = transform.GetChild(i).GetComponent<EnemyController>();
        }

        saveData.enemyManager = new EnemyManager.SaveData[enemyManagers.Length];
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
}
