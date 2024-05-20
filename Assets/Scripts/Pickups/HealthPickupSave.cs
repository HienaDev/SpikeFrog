using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HealthPickupSave : MonoBehaviour
{
    private HealthPickup[] healthPickups;

    // Start is called before the first frame update
    void Start()
    {
        healthPickups = new HealthPickup[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            healthPickups[i] = transform.GetChild(i).GetComponent<HealthPickup>();
        }
    }

    [System.Serializable]
    public struct SaveData
    {
        public HealthPickup.SaveData[] healthPickup;
    }

    public SaveData GetSaveData()
    {
        SaveData saveData;

        for (int i = 0; i < transform.childCount; i++)
        {
            healthPickups[i] = transform.GetChild(i).GetComponent<HealthPickup>();
        }

        saveData.healthPickup = new HealthPickup.SaveData[healthPickups.Length];

        for (int i = 0; i < healthPickups.Length; i++)
        {
            saveData.healthPickup[i] = healthPickups[i].GetSaveData();
        }

        return saveData;
    }

    public void LoadSaveData(SaveData saveData)
    {
        
        for (int i = 0; i < healthPickups.Length; i++)
        {
            healthPickups[i].LoadSaveData(saveData.healthPickup[i]);
        }
    }
}
