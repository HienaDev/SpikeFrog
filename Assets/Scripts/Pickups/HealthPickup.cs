using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [SerializeField] private int regen;

    private bool isActive;

    private void Start()
    {
        isActive = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

        if (playerHealth && !playerHealth.IsFullHealth())
        {
            playerHealth.Regen(regen);

            isActive = false;
            gameObject.SetActive(false);
        }
    }

    [System.Serializable]
    public struct SaveData
    {
        public bool active;
    }

    public SaveData GetSaveData()
    {
        SaveData saveData;

        saveData.active = isActive;

        return saveData;
    }

    public void LoadSaveData(SaveData saveData)
    {
        gameObject.SetActive(saveData.active);
    }
}
