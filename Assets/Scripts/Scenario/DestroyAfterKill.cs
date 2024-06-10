using UnityEngine;

public class DestroyAfterKill : MonoBehaviour
{
    [SerializeField] private GameObject[] enemiesToKill;

    private bool alreadyDestroyed = false;

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
            gameObject.SetActive(false);
        }
    }
}
