using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerSounds : MonoBehaviour
{
    [SerializeField] private AudioClip[] stepsSound;
    private AudioSource audioSourceSteps;
    [SerializeField] private AudioMixerGroup stepsMixer;

    [SerializeField] private AudioClip[] punchSound;
    private AudioSource audioSourcePunch;
    [SerializeField] private AudioMixerGroup punchMixer;

    [SerializeField] private AudioClip[] tongueSound;
    private AudioSource audioSourceTongue;
    [SerializeField] private AudioMixerGroup tongueMixer;

    [SerializeField] private AudioClip[] jumpSound;
    private AudioSource audioSourceJump;
    [SerializeField] private AudioMixerGroup jumpMixer;





    private float justStep;

    // Start is called before the first frame update
    void Start()
    {
        audioSourceSteps = gameObject.AddComponent<AudioSource>();
        audioSourceSteps.outputAudioMixerGroup = stepsMixer;

        audioSourcePunch = gameObject.AddComponent<AudioSource>();
        audioSourcePunch.outputAudioMixerGroup = punchMixer;

        audioSourceTongue = gameObject.AddComponent<AudioSource>();
        audioSourceTongue.outputAudioMixerGroup = tongueMixer;

        audioSourceJump = gameObject.AddComponent<AudioSource>();
        audioSourceJump.outputAudioMixerGroup = jumpMixer;


        audioSourcePunch.volume = 0.2f;
        audioSourceSteps.volume = 0.2f;
        audioSourceTongue.volume = 0.2f;
        audioSourceJump.volume = 0.2f;


    }




    public void PlayStepsSound()
    {
        audioSourceSteps.clip = stepsSound[Random.Range(0, stepsSound.Length)];
        audioSourceSteps.pitch = Random.Range(0.8f, 1.20f);

        audioSourceSteps.Play();
    }

    public void PlayPunchSound()
    {
        audioSourcePunch.clip = punchSound[Random.Range(0, punchSound.Length)];
        audioSourcePunch.pitch = Random.Range(0.8f, 1.20f);

        audioSourcePunch.Play();
    }

    public void PlaytongueSound()
    {
        audioSourceTongue.clip = tongueSound[Random.Range(0, tongueSound.Length)];
        audioSourceTongue.pitch = Random.Range(0.8f, 1.20f);

        audioSourceTongue.Play();
    }

    public void PlayjumpSound()
    {
        audioSourceJump.clip = jumpSound[Random.Range(0, jumpSound.Length)];
        audioSourceJump.pitch = Random.Range(0.8f, 1.20f);

        audioSourceJump.Play();
    }



}
