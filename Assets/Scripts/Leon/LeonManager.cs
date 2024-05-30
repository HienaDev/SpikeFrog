using UnityEngine;

public class LeonManager : MonoBehaviour
{
    [Header("[Leon Settings]")]
    [SerializeField] private float health = 100;

    private LeonController  leonController;
    private Animator        animator;

    void Start()
    {
        leonController  = GetComponent<LeonController>();
        animator        = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DestroyController();
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
        //animator.Play("Destroy Controller");
        leonController.CurrentState = LeonState.NotControlled;
    }
}
