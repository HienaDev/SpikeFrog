using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerEndGame : MonoBehaviour
{

    [SerializeField] private GameObject[] enemiesToKill;
    [SerializeField] private SaveManager saveManager;

    private void FixedUpdate()
    {

        // Check if all objects in the array have been destroyed
        bool allDestroyed = true;
        foreach (GameObject obj in enemiesToKill)
        {
            if (obj.activeSelf)
            {
                allDestroyed = false;
                break;
            }
        }

        // If all objects are destroyed, perform the action
        if (allDestroyed)
        {
            SceneManager.LoadScene("Loading Celebration");
        }
    }
}
