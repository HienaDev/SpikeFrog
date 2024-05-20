using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    [SerializeField] private string       saveFileName;
    [SerializeField] private PlayerHealth playerHealth;

    private GameSaveData gameSaveData;
    private string saveFilePath;

    // Start is called before the first frame update
    private void Start()
    {
        saveFileName = Application.persistentDataPath + "/" + saveFileName;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetButtonDown("QuickSave"))
        {
            QuickSaveGame();
        }

        else if (Input.GetButtonDown("QuickLoad"))
        {
            QuickLoadGame();
        }
    }

    private struct GameSaveData
    {
        public PlayerHealth.SaveData playerHealthSaveData;
    }

    private void QuickSaveGame()
    {
        GameSaveData saveData;

        saveData.playerHealthSaveData = playerHealth.GetSaveData();

        string jsonSaveData = JsonUtility.ToJson(saveData, true);
        
        File.WriteAllText(saveFileName, jsonSaveData);
    }

    private void QuickLoadGame()
    {
        print ("Game Loaded");
    }
}
