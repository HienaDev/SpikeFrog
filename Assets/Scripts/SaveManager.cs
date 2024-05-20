using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    [SerializeField] private string              saveFileName;
    [SerializeField] private PlayerHealth        playerHealth;
    [SerializeField] private PlayerMovement      playerMovement;
    [SerializeField] private PlayerCombat        playerCombat;
    [SerializeField] private ControlCamera       controlCamera;
    [SerializeField] private EnemySave           enemySave;
    //[SerializeField] private HealthPickupSave    healthPickUpManager;

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
        public PlayerHealth.SaveData        playerHealth;
        public PlayerMovement.SaveData      playerMovement;
        public PlayerCombat.SaveData        playerCombat;
        public ControlCamera.SaveData       controlCamera;
        public EnemySave.SaveData           enemies;
        //public HealthPickupSave.SaveData    healthPickups;
    }

    private void QuickSaveGame()
    {
        GameSaveData saveData;

        saveData.playerHealth   = playerHealth.GetSaveData();
        saveData.playerMovement = playerMovement.GetSaveData();
        saveData.playerCombat   = playerCombat.GetSaveData();
        saveData.controlCamera  = controlCamera.GetSaveData();
        saveData.enemies        = enemySave.GetSaveData();
        //saveData.healthPickups  = healthPickUpManager.GetSaveData();

        string jsonSaveData = JsonUtility.ToJson(saveData, true);
        
        File.WriteAllText(saveFileName, jsonSaveData);

        print ("Game Saved");
    }

    private void QuickLoadGame()
    {
        if (File.Exists(saveFileName))
        {
            string jsonSaveData = File.ReadAllText(saveFileName);

            GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(jsonSaveData);

            playerHealth.LoadSaveData(saveData.playerHealth);
            playerMovement.LoadSaveData(saveData.playerMovement);
            playerCombat.LoadSaveData(saveData.playerCombat);
            controlCamera.LoadSaveData(saveData.controlCamera);
            enemySave.LoadSaveData(saveData.enemies);
            //healthPickUpManager.LoadSaveData(saveData.healthPickups);

            print ("Game Loaded");
        }
        else
        {
            print ("No save file found");
        }
    }
}
