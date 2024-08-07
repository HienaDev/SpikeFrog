using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    [SerializeField] private string             saveFileName;
    [SerializeField] private PlayerHealth       playerHealth;
    [SerializeField] private PlayerMovement     playerMovement;
    [SerializeField] private PlayerCombat       playerCombat;
    [SerializeField] private ControlCamera      controlCamera;
    [SerializeField] private EnemySave          enemySave;
    [SerializeField] private HealthPickupSave   healthPickupSave;
    [SerializeField] private Settings           settings;
    [SerializeField] private LeonManager        leonManager;
    [SerializeField] private LeonController     leonController;
    [SerializeField] private DialogSave         dialogSave;
    [SerializeField] private SaveMessageDisplay saveMessageDisplay;

    private GameSaveData    gameSaveData;
    private string          saveFilePath;

    [HideInInspector] public bool death = false;

    public static SaveManager Instance { get; private set; }

    private void Start()
    {
        saveFileName = Application.persistentDataPath + "/" + saveFileName;

        SceneManager.sceneLoaded += OnSceneLoaded;

        LookForReferences();

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    private void LookForReferences()
    {
        if (playerHealth == null)
            playerHealth = FindObjectOfType<PlayerHealth>();
        if (playerMovement == null)
            playerMovement = FindObjectOfType<PlayerMovement>();
        if (playerCombat == null)
            playerCombat = FindObjectOfType<PlayerCombat>();
        if (controlCamera == null)
            controlCamera = FindObjectOfType<ControlCamera>();
        if (enemySave == null)
            enemySave = FindObjectOfType<EnemySave>();
        if (healthPickupSave == null)
            healthPickupSave = FindObjectOfType<HealthPickupSave>();
        if (settings == null)
            settings = FindObjectOfType<Settings>();
        if (leonManager == null)
            leonManager = FindObjectOfType<LeonManager>();
        if (leonController == null)
            leonController = FindObjectOfType<LeonController>();
        if (dialogSave == null)
            dialogSave = FindObjectOfType<DialogSave>();
        if (saveMessageDisplay == null)
            saveMessageDisplay = FindObjectOfType<SaveMessageDisplay>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            QuickSaveGame();
        }

        if (Input.GetKeyDown(KeyCode.F9))
        {
            QuickLoadGame();
        }
    }

    // called second
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (death)
        {
            QuickLoadGame();
            death = false;
        }
        Debug.Log("OnSceneLoaded: " + scene.name);
        Debug.Log(mode);
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
        public DialogSave.SaveData          dialogSave;
    }

    public void QuickSaveGame()
    {
        LookForReferences();

        GameSaveData saveData;

        saveData.playerHealth       = playerHealth.GetSaveData();
        saveData.playerMovement     = playerMovement.GetSaveData();
        saveData.playerCombat       = playerCombat.GetSaveData();
        saveData.controlCamera      = controlCamera.GetSaveData();
        saveData.enemies            = enemySave.GetSaveData();
        saveData.healthPickups      = healthPickupSave.GetSaveData();
        saveData.settings           = settings.GetSaveData();
        saveData.leonManager        = leonManager.GetSaveData();
        saveData.leonController     = leonController.GetSaveData();
        saveData.dialogSave         = dialogSave.GetSaveData();

        string jsonSaveData = JsonUtility.ToJson(saveData, true);

        File.WriteAllText(saveFileName, jsonSaveData);

        print("Game Saved");

        // Trigger the save message display
        if (saveMessageDisplay != null)
        {
            saveMessageDisplay.DisplaySaveMessage();
        }
    }

    public void CreateNewGame()
    {
        LookForReferences();

        GameSaveData saveData;

        saveData.playerHealth = playerHealth.GetSaveData();
        saveData.playerMovement = playerMovement.GetSaveData();
        saveData.playerCombat = playerCombat.GetSaveData();
        saveData.controlCamera = controlCamera.GetSaveData();
        saveData.enemies = enemySave.GetSaveData();
        saveData.healthPickups = healthPickupSave.GetSaveData();
        saveData.settings = settings.GetSaveData();
        saveData.leonManager = leonManager.GetSaveData();
        saveData.leonController = leonController.GetSaveData();
        saveData.dialogSave = dialogSave.GetSaveData();

        string jsonSaveData = JsonUtility.ToJson(saveData, true);

        File.WriteAllText(saveFileName, jsonSaveData);

        print("Game Created");
    }

    public bool QuickLoadGame()
    {

        LookForReferences();

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

            if (leonManager != null)
                leonManager.LoadSaveData(saveData.leonManager);

            if (leonController != null)
                leonController.LoadSaveData(saveData.leonController);

            dialogSave.LoadSaveData(saveData.dialogSave);

            print("Game Loaded");

            return true;
        }
        else
        {
            print("No save file found");

            return false;
        }
    }

    public void DeleteSaveFile()
    {
        if (File.Exists(saveFileName))
        {
            File.Delete(saveFileName);

            print("Save file deleted");
        }
        else
        {
            print("No save file found");
        }
    }

    public bool SaveFileExists()
    {
        return File.Exists(saveFileName);
    }
}