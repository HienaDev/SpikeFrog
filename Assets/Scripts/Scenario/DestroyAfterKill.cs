using UnityEngine;
using UnityEngine.Audio;

public class DestroyAfterKill : MonoBehaviour
{
    [SerializeField] private GameObject[] enemiesToKill;
    private AudioSource  audioSource;
    [SerializeField] private AudioMixerGroup audioMixer;
    [SerializeField] private AudioClip    soundEffect;

    private Animator animator;
    private bool alreadyDestroyed = false;

    private void Start()
    {
        animator = GetComponent<Animator>();

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = audioMixer;
    }

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
        if (allDestroyed && !alreadyDestroyed)
        {
            alreadyDestroyed = true;
            // Play animation Open
            OpenDoor();
            // Play sound effect
            audioSource.clip = soundEffect;
            audioSource.Play();
        }
    }

    public void CloseDoor()
    {
        // Play animation Close
        animator.SetTrigger("Close");
    }

    public void OpenDoor()
    {
        // Play animation Open
        animator.SetTrigger("Open");
    }
}
