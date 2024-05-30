using UnityEngine;

public class LeonManager : MonoBehaviour
{
    [Header("[Leon Settings]")]
    [SerializeField] private float health = 100;

    private LeonController  leonController;

    void Start()
    {
        leonController  = GetComponent<LeonController>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DestroyController();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            leonController.CurrentState = LeonState.Stunned;
        }
    }

    public void TakeDamage(int damage)
    {
        if (leonController.CurrentState == LeonState.Stunned)
        {
            health -= damage;

            if (health <= 0)
            {
                DestroyController();
            }
            else
            {
                //TriggerDamageEffects();
            }
        }
    }

    private void DestroyController()
    {
        leonController.CurrentState = LeonState.NotControlled;
    }
}
