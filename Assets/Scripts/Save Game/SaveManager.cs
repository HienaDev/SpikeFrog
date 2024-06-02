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
    [SerializeField] private HealthPickupSave    healthPickupSave;
    [SerializeField] private Settings            settings;
    [SerializeField] private LeonManager         leonManager;
    [SerializeField] private LeonController      leonController;

    private GameSaveData gameSaveData;
    private string saveFilePath;

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

        else if (Input.GetKeyDown(KeyCode.F))
        {
            DeleteSaveFile();
        }
    }

    private struct GameSaveData
    {
        public PlayerHealth.SaveData        playerHealth;
        public PlayerMovement.SaveData      playerMovement;
        public PlayerCombat.SaveData        playerCombat;
        public ControlCamera.SaveData       controlCamera;
        public EnemySave.SaveData           enemies;
        public HealthPickupSave.SaveData    healthPickups;
        public Settings.SaveData            settings;
        public LeonManager.SaveData         leonManager;
        public LeonController.SaveData      leonController;
    }

    public void QuickSaveGame()
    {
        GameSaveData saveData;

        saveData.playerHealth   = playerHealth.GetSaveData();
        saveData.playerMovement = playerMovement.GetSaveData();
        saveData.playerCombat   = playerCombat.GetSaveData();
        saveData.controlCamera  = controlCamera.GetSaveData();
        saveData.enemies        = enemySave.GetSaveData();
        saveData.healthPickups  = healthPickupSave.GetSaveData();
        saveData.settings       = settings.GetSaveData();

        
        saveData.leonManager    = leonManager.GetSaveData();
        saveData.leonController = leonController.GetSaveData();

        string jsonSaveData = JsonUtility.ToJson(saveData, true);
        
        File.WriteAllText(saveFileName, jsonSaveData);

        print ("Game Saved");
    }

    public bool QuickLoadGame()
    {
        if (File.Exists(saveFileName))
        {
            enemySave.ReloadEnemies();

            string jsonSaveData = File.ReadAllText(saveFileName);

            GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(jsonSaveData);

            playerHealth.LoadSaveData(saveData.playerHealth);
            playerMovement.LoadSaveData(saveData.playerMovement);
            playerCombat.LoadSaveData(saveData.playerCombat);
            controlCamera.LoadSaveData(saveData.controlCamera);
            enemySave.LoadSaveData(saveData.enemies);
            healthPickupSave.LoadSaveData(saveData.healthPickups);
            settings.LoadSaveData(saveData.settings);

            if(leonManager != null)
                leonManager.LoadSaveData(saveData.leonManager);

            if (leonController != null)
                leonController.LoadSaveData(saveData.leonController);
            
            

            print ("Game Loaded");

            return true;
        }
        else
        {
            print ("No save file found");

            return false;
        }
    }

    public void DeleteSaveFile()
    {
        if (File.Exists(saveFileName))
        {
            File.Delete(saveFileName);

            print ("Save file deleted");
        }
        else
        {
            print ("No save file found");
        }
    }

    public bool SaveFileExists()
    {
        return File.Exists(saveFileName);
    }
}
