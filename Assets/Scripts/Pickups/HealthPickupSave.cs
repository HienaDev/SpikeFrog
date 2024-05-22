using System.Collections.Generic;
using UnityEngine;

public class HealthPickupSave : MonoBehaviour
{
    [SerializeField] private GameObject healthPickupPrefab;

    private List<HealthPickup> healthPickups;

    // Start is called before the first frame update
    void Start()
    {
        healthPickups = new List<HealthPickup>();

        for (int i = 0; i < transform.childCount; i++)
        {
            healthPickups.Add(transform.GetChild(i).GetComponent<HealthPickup>());
        }
    }

    [System.Serializable]
    public struct SaveData
    {
        public HealthPickup.SaveData[] healthPickup;
        public Vector3[] healthPickupPositions;
    }

    public SaveData GetSaveData()
    {
        SaveData saveData;

        for (int i = 0; i < transform.childCount; i++)
        {
            HealthPickup healthPickup = transform.GetChild(i).GetComponent<HealthPickup>();
            if (healthPickup != null)
            {
                healthPickups.Add(healthPickup);
            }
        }

        saveData.healthPickup = new HealthPickup.SaveData[healthPickups.Count];
        saveData.healthPickupPositions = new Vector3[healthPickups.Count];

        for (int i = 0; i < healthPickups.Count; i++)
        {
            saveData.healthPickup[i] = healthPickups[i].GetSaveData();
            saveData.healthPickupPositions[i] = healthPickups[i].transform.position;
        }

        return saveData;
    }

    public void LoadSaveData(SaveData saveData)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        healthPickups.Clear();
        for (int i = 0; i < saveData.healthPickup.Length; i++)
        {
            GameObject newPickup = Instantiate(healthPickupPrefab, saveData.healthPickupPositions[i], Quaternion.identity, transform);
            HealthPickup healthPickup = newPickup.GetComponent<HealthPickup>();
            healthPickup.LoadSaveData(saveData.healthPickup[i]);
            healthPickups.Add(healthPickup);
        }
    }
}
