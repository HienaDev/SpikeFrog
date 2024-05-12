using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class DestroyAfterKill : MonoBehaviour
{

    [SerializeField] private GameObject[] enemiesToKill;

    private void FixedUpdate()
    {

        // Check if all objects in the array have been destroyed
        bool allDestroyed = true;
        foreach (GameObject obj in enemiesToKill)
        {
            if (obj != null)
            {
                allDestroyed = false;
                break;
            }
        }

        // If all objects are destroyed, perform the action
        if (allDestroyed)
        {
            Destroy(gameObject);
        }
    }
}
