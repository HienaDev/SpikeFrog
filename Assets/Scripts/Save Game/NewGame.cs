using UnityEngine;

public class NewGame : MonoBehaviour
{
    private SaveManager saveManager;

    // Start is called before the first frame update
    void Start()
    {
        saveManager = FindObjectOfType<SaveManager>();

        if (saveManager == null)
            return;

        saveManager.DeleteSaveFile();
    }
}
