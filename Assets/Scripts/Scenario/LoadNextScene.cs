using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNextScene : MonoBehaviour
{
    [SerializeField] private float timer;
    [SerializeField] private string scene;
    [SerializeField] private bool outsideTrigger;
    private float justDied = 0f;

    // Update is called once per frame
    void Update()
    {
        if (Time.timeSinceLevelLoad - justDied > timer && !outsideTrigger)
        {
            SceneManager.LoadScene(scene);
        }
    }

    public void SceneLoad()
    {
        justDied = Time.timeSinceLevelLoad;
        outsideTrigger = false;
        
    }
}
