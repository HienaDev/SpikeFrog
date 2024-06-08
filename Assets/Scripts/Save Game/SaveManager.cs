using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    [SerializeField] private string saveFileName;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerCombat playerCombat;
    [SerializeField] private ControlCamera controlCamera;
    [SerializeField] private EnemySave enemySave;
    [SerializeField] private HealthPickupSave healthPickupSave;
    [SerializeField] private Settings settings;
    [SerializeField] private LeonManager leonManager;
    [SerializeField] private LeonController leonController;
    [SerializeField] private DialogSave dialogSave;

    private GameSaveData gameSaveData;
    private string saveFilePath;

    public bool death = false;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        saveFileName = Application.persistentDataPath + "/" + saveFileName;

        SceneManager.sceneLoaded += OnSceneLoaded;

        LookForReferences();
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
        public PlayerHealth.SaveData playerHealth;
        public PlayerMovement.SaveData playerMovement;
        public PlayerCombat.SaveData playerCombat;
        public ControlCamera.SaveData controlCamera;
        public EnemySave.SaveData enemies;
        public HealthPickupSave.SaveData healthPickups;
        public Settings.SaveData settings;
        public LeonManager.SaveData leonManager;
        public LeonController.SaveData leonController;
        public DialogSave.SaveData dialogSave;
    }

    public void QuickSaveGame()
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

        print("Game Saved");
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
