using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class SwapMusic : MonoBehaviour
{
    [SerializeField] private AudioSource normalMusic;
    [SerializeField] private AudioMixerGroup normalMusicMixerGroup;

    [SerializeField] private AudioSource bossMusic;
    [SerializeField] private AudioMixerGroup bossMusicMixerGroup;

    [SerializeField] private float transitionTime;

    private float defaultVolume;
    private float bossVolume;

    // Start is called before the first frame update
    void Start()
    {
        defaultVolume = normalMusic.volume;
        bossVolume = bossMusic.volume;

        bossMusic.volume = 0;

        normalMusic.outputAudioMixerGroup = normalMusicMixerGroup;
        bossMusic.outputAudioMixerGroup = bossMusicMixerGroup;
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