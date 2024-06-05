using System.Collections.Generic;
using UnityEngine;

public class MenusManager : MonoBehaviour
{
    [SerializeField] private PlayerCombat   playerCombat;
    [SerializeField] private ControlCamera  controlCamera;
    [SerializeField] private SaveManager    saveManager;
    [SerializeField] private GameObject     gameUI;
    [SerializeField] private GameObject     mainMenuBackground;
    [SerializeField] private GameObject     mainMenu;
    [SerializeField] private GameObject     pauseMenu;
    [SerializeField] private GameObject     settingsMenu;
    [SerializeField] private GameObject     credits;
    [SerializeField] private GameObject     loadingScreen;
    [SerializeField] private GameObject     exitGameQuestion;
    [SerializeField] private GameObject     quitQuestion;
    [SerializeField] private GameObject     saveWarningText;
    [SerializeField] private GameObject     newGameQuestion;

    private bool isInGame;
    private bool isPaused;
    private bool isSettingsOpen;
    private bool isCreditsOpen;
    private bool finalCredits;

    public bool FinalCredits => finalCredits;
    public bool IsPaused => isPaused;

    private HashSet<EnemyController> enemiesInCombat = new HashSet<EnemyController>();

    public void RegisterEnemyCombat(EnemyController enemy)
    {
        enemiesInCombat.Add(enemy);
    }

    public void UnregisterEnemyCombat(EnemyController enemy)
    {
        enemiesInCombat.Remove(enemy);
    }

    public bool IsAnyEnemyInCombat()
    {
        return enemiesInCombat.Count > 0;
    }

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
            if (!isPaused && isInGame)
            {
                OpenPauseMenu();
            }

            else if ((isSettingsOpen && isInGame) || (isSettingsOpen && !isInGame))
            {
                CloseSettingsMenu();
            }

            else if (isPaused && isInGame)
            {
                ClosePauseMenu();
            }

            else if (!isInGame && isCreditsOpen)
            {
                CloseCredits();
            }
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

    public void Quit()
    {
        quitQuestion.SetActive(true);
    }

    public void ConfirmQuit()
    {
        pauseMenu.SetActive(false);
        quitQuestion.SetActive(false);
        GoToMainMenu();
    }

    public void CancelQuit()
    {
        quitQuestion.SetActive(false);
    }

    public void OpenSettingsMenu()
    {
        if (!isInGame)
        {
            mainMenu.SetActive(false);
        }

        if (isPaused)
        {
            pauseMenu.SetActive(false);
        }

        isSettingsOpen = true;
        settingsMenu.SetActive(true);
    }

    public void CloseSettingsMenu()
    {
        settingsMenu.SetActive(false);
        isSettingsOpen = false;

        if (!isInGame)
            mainMenu.SetActive(true);

        if (isPaused)
        {
            pauseMenu.SetActive(true);
        }
    }

    public void OpenPauseMenu()
    {
        AudioListener.pause = true;
        pauseMenu.SetActive(true);
        controlCamera.enabled = false;
        gameUI.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;
        isPaused = true;
    }

    public void ClosePauseMenu()
    {
        AudioListener.pause = false;
        pauseMenu.SetActive(false);
        controlCamera.enabled = true;
        gameUI.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
        isPaused = false;
    }

    public void OpenCredits()
    {
        isCreditsOpen = true;
        // Activate time scale but only on the credits himself TODO
        Time.timeScale = 1;

        if (!isInGame)
        {
            mainMenu.SetActive(false);
            credits.SetActive(true);
        }

        if (isInGame)
        {
            pauseMenu.SetActive(false);
            credits.SetActive(true);
        }
    }

    public void CloseCredits()
    {
        isCreditsOpen = false;
        Time.timeScale = 0;

        if (!isInGame)
        {
            credits.SetActive(false);
            mainMenu.SetActive(true);
        }

        if (isInGame)
        {
            credits.SetActive(false);
            pauseMenu.SetActive(true);
        }
    }

    public void GoToMainMenu()
    {
        mainMenuBackground.SetActive(true);
        mainMenu.SetActive(true);
        gameUI.SetActive(false);
        credits.SetActive(false);
        pauseMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;

        playerCombat.enabled = false;
        isInGame = false;
        isPaused = false;
    }

    public void EndOfFinalCredits()
    {
        finalCredits = false;
        saveManager.DeleteSaveFile();
    }

    public void SaveGame()
    {
        if (IsAnyEnemyInCombat())
        {
            ShowSaveWarning();
            return;
        }

        saveManager.QuickSaveGame();
    }


    public void SaveAndQuit()
    {
        if (IsAnyEnemyInCombat())
        {
            ShowSaveWarning();
            return;
        }

        saveManager.QuickSaveGame();
        GoToMainMenu();
    }

    private void ShowSaveWarning()
    {
        saveWarningText.SetActive(true);
    }

    public void CloseSaveWarning()
    {
        saveWarningText.SetActive(false);
    }
}
