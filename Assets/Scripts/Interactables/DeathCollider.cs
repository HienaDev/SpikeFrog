using UnityEngine;
using UnityEngine.SceneManagement;


public class DeathCollider : MonoBehaviour
{

    private PlayerHealth playerHealth;

    // Start is called before the first frame update
    void Start()
    {
        playerHealth = FindObjectOfType<PlayerHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerCombat playerCombat = other.GetComponent<PlayerCombat>();

        if (playerCombat != null)
        {
            SceneManager.LoadScene("Loading Death");
        }
    }
}
