using UnityEngine;

public class SaveTrigger : MonoBehaviour
{
    private SaveManager saveManager;
    private bool        isTriggered;

    private void Start() 
    {
        saveManager = FindObjectOfType<SaveManager>();
        isTriggered = false;
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (other.CompareTag("Player") && !isTriggered)
        {
            isTriggered = true;
            gameObject.SetActive(false);
            saveManager.QuickSaveGame();
        }
    }
}
