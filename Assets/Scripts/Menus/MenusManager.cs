using Unity.VisualScripting;
using UnityEngine;

public class MenusManager : MonoBehaviour
{
    [SerializeField] private PlayerCombat    playerCombat;
    [SerializeField] private ControlCamera   controlCamera;
    [SerializeField] private SaveManager     saveManager;
    [SerializeField] private GameObject      gameUI;
    [SerializeField] private GameObject      mainMenuBackground;
    [SerializeField] private GameObject      mainMenu;
    [SerializeField] private GameObject      settingsMenu;
    [SerializeField] private GameObject      loadingScreen;
    [SerializeField] private GameObject      newGameQuestion;
    [SerializeField] private GameObject      exitGameQuestion;

    private bool isInGame;
    private bool isSettingsOpen;

    private void Start()
    {
        isInGame = false;

        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;

        mainMenuBackground.SetActive(true);
        mainMenu.SetActive(true);
        gameUI.SetActive(false);
        loadingScreen.SetActive(false);

        playerCombat.enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isSettingsOpen)
                CloseSettingsMenu();
            else if (isInGame)
                OpenSettingsMenu();
        }
    }

    public void ContinueGame()
    {
        LoadLevel();
    }

    private void LoadLevel()
    {
        mainMenuBackground.SetActive(false);
        mainMenu.SetActive(false);
        loadingScreen.SetActive(true);
        saveManager.QuickLoadGame();

        if (!saveManager.QuickLoadGame())
            saveManager.QuickSaveGame();
        
        loadingScreen.SetActive(false);
        gameUI.SetActive(true);  
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;

        playerCombat.enabled = true;
        isInGame = true;
    }

    public void NewGame()
    {
        if (saveManager.SaveFileExists())
            newGameQuestion.SetActive(true);
        else
            LoadLevel();
    }

    public void ConfirmNewGame()
    {
        newGameQuestion.SetActive(false);
        saveManager.DeleteSaveFile();
        saveManager.QuickSaveGame();
        LoadLevel();
    }

    public void CancelNewGame()
    {
        newGameQuestion.SetActive(false);
    }

    public void ExitGame()
    {
        exitGameQuestion.SetActive(true);
    }

        public void ConfirmExitGame()
    {
        #if UNITY_STANDALONE
            // Exit application if running standalone
            Application.Quit();
#endif
#if UNITY_EDITOR
            // Stop game if running in editor
            UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void CancelExitGame()
    {
        exitGameQuestion.SetActive(false);
    }

    public void OpenSettingsMenu()
    {
        if (!isInGame)
        {
            mainMenu.SetActive(false);
        }

        if (isInGame)
        {
            controlCamera.enabled = false;
            gameUI.SetActive(false);
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
        }

        isSettingsOpen = true;
        settingsMenu.SetActive(true);
    }

    public void CloseSettingsMenu()
    {
        settingsMenu.SetActive(false);

        if (!isInGame)
            mainMenu.SetActive(true);

        if (isInGame)
        {
            controlCamera.enabled = true;
            gameUI.SetActive(true);
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;
            isSettingsOpen = false;
        }
    }
}