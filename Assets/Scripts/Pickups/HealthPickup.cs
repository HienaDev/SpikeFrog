using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [SerializeField] private int regen;

    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

        if (playerHealth && !playerHealth.IsFullHealth())
        {
            playerHealth.Regen(regen);

            Destroy(gameObject);
        }
    }
}
