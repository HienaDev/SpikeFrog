using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportAfterKill : MonoBehaviour
{
    [SerializeField] private GameObject[] enemiesToKill;
    [SerializeField] private Transform teleportTo;
    [SerializeField] private GameObject player;

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
        if (allDestroyed)
        {
            player.transform.position = teleportTo.position;
            Destroy(gameObject, 0.1f);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha9)) { player.transform.position = teleportTo.position; }
    }
}
