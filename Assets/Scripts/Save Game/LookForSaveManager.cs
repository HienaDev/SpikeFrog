using UnityEngine;

public class LookForSaveManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SaveManager saveManager = FindObjectOfType<SaveManager>();

        if (saveManager != null)
        {
            saveManager.death = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
