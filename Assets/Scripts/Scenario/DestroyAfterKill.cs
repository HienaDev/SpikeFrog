using UnityEngine;

public class DestroyAfterKill : MonoBehaviour
{
    [SerializeField] private GameObject[] enemiesToKill;

    private Animator animator;
    private bool alreadyDestroyed = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
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
            animator.SetTrigger("Open");
        }
    }
}
