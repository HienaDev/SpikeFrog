using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerSounds : MonoBehaviour
{

    [SerializeField] private AudioClip[] punchSound;
    private AudioSource audioSourcePunch;
    [SerializeField] private AudioMixerGroup punchMixer;

    // Start is called before the first frame update
    void Start()
    {
        audioSourcePunch = gameObject.AddComponent<AudioSource>();
        audioSourcePunch.outputAudioMixerGroup = punchMixer;

        audioSourcePunch.volume = 0.5f;
    }

    // Update is called once per frame  
    void Update()
    {
        
    }

    public void PlayPunchSound()
    {
        audioSourcePunch.clip = punchSound[Random.Range(0, punchSound.Length)];

        audioSourcePunch.pitch = Random.Range(0.8f, 1.2f);

        audioSourcePunch.Play();
    }
}
