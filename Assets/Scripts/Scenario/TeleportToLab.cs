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

    public void TeleportPlayer()
    {
        player.transform.position = teleportTo.position;
    }
}
