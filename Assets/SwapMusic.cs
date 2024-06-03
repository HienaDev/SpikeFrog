using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SwapMusic : MonoBehaviour
{

    [SerializeField] private AudioSource normalMusic;
    private float defaultVolume;
    [SerializeField] private AudioSource bossMusic;
    private float bossVolume;
    [SerializeField] private float transitionTime;

    // Start is called before the first frame update
    void Start()
    {
        defaultVolume = normalMusic.volume;
        bossVolume = bossMusic.volume;

        bossMusic.volume = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            StartCoroutine(ChangeMusic(true, transitionTime));
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            StartCoroutine(ChangeMusic(false, transitionTime));
        }
    }


    public IEnumerator ChangeMusic(bool boss, float duration)
    {
        if (boss)
        {

            bossMusic.time = 0;
            for (float t = 0f; t < duration; t += Time.deltaTime)
            {
                bossMusic.volume = Mathf.Lerp(0, bossVolume, t / duration);
                normalMusic.volume = Mathf.Lerp(defaultVolume, 0, t / duration);
                yield return null;
            }

        }
        else
        {
            for (float t = 0f; t < duration; t += Time.deltaTime)
            {
                bossMusic.volume = Mathf.Lerp(bossVolume, 0, t / duration);
                normalMusic.volume = Mathf.Lerp(0, defaultVolume, t / duration);
                yield return null;
            }


        }

    }
}