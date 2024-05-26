using UnityEngine;

public class MenusManager : MonoBehaviour
{
    [SerializeField] private GameObject      gameUI;
    [SerializeField] private GameObject      mainMenu;
    [SerializeField] private GameObject      loadingScreen;
    [SerializeField] private SaveManager     saveManager;
    [SerializeField] private PlayerCombat    playerCombat;

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

        if (saveManager.QuickLoadGame())
        {
            loadingScreen.SetActive(false);
            
            gameUI.SetActive(true);  
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;

            playerCombat.enabled = true;
        }
        else
        {
            Debug.LogError("No save file found.");
            mainMenu.SetActive(true);
            loadingScreen.SetActive(false);
        }
    }
}