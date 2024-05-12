using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNextScene : MonoBehaviour
{
    [SerializeField] private float timer;
    [SerializeField] private string scene;
    [SerializeField] private bool outsideTrigger;

    // Update is called once per frame
    void Update()
    {
        if (Time.timeSinceLevelLoad > timer && !outsideTrigger)
        {
            SceneLoad();
        }
    }

    public void SceneLoad()
    {
        SceneManager.LoadScene(scene);
    }
}
