using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private UIManager  uiManager;
    [SerializeField] private int        maxHealth;

    private PlayerMovement playerScript;
    private PlayerCombat playerCombat;
    private Animator animator;
    private int health;
    private bool dead;

    void Start()
    {
        health = maxHealth;

        playerScript = GetComponent<PlayerMovement>();
        playerCombat = GetComponentInChildren<PlayerCombat>();
        animator = GetComponentInChildren<Animator>();

        dead = false;

        UpdateUI();
    }

    private void UpdateUI()
    {
        uiManager.SetHealth(health);
    }

    public bool IsFullHealth()
    {
        return health == maxHealth;
    }

    public void Regen(int amount)
    {
        health = Mathf.Min(health + amount, maxHealth);

        UpdateUI();
    }

    public void Damage(int amount)
    {
        health = Mathf.Max(health - amount, 0);


        UpdateUI();

        if(health <= 0 && !dead)
        {
            playerScript.enabled = false;
            playerCombat.enabled = false;
            animator.SetTrigger("Death");
            dead = true;
        }
    }

    public void FullHealth()
    {
        health = maxHealth;

        UpdateUI();
    }
}
