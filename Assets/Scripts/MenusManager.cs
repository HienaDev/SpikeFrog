using UnityEngine;

public class MenusManager : MonoBehaviour
{
    [SerializeField] private PlayerCombat    playerCombat;
    [SerializeField] private GameObject      gameUI;
    [SerializeField] private GameObject      mainMenu;
    [SerializeField] private GameObject      loadingScreen;
    [SerializeField] private SaveManager     saveManager;
    [SerializeField] private GameObject      newGameQuestion;
    [SerializeField] private GameObject      exitGameQuestion;

    private void Start()
    {
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;

        mainMenu.SetActive(true);
        gameUI.SetActive(false);
        loadingScreen.SetActive(false);

        playerCombat.enabled = false;
    }

    public void ContinueGame()
    {
        LoadLevel();
    }

    private void LoadLevel()
    {
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
}