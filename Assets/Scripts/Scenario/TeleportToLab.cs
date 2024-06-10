using UnityEngine;

public class TeleportToLab : MonoBehaviour
{
    [SerializeField] private Transform teleportTo;
    [SerializeField] private GameObject player;

    // On trigger enter teleport to the position
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player.transform.position = teleportTo.position;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha9)) { player.transform.position = teleportTo.position; }
    }
}
