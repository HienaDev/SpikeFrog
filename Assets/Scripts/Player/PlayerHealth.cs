using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;
    [SerializeField] private int       maxHealth;

    private PlayerMovement playerScript;
    private PlayerCombat   playerCombat;
    private Animator       animator;
    private int            health;
    private bool           dead;
    private bool           canBeDamaged;

    void Start()
    {
        health = maxHealth;

        playerScript = GetComponent<PlayerMovement>();
        playerCombat = GetComponentInChildren<PlayerCombat>();
        animator = GetComponentInChildren<Animator>();

        dead = false;
        canBeDamaged = true;

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
        if (canBeDamaged)
            health = Mathf.Max(health - amount, 0);


        UpdateUI();

        if (health <= 0 && !dead)
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

    public bool IsAlive => health > 0;

    public bool ToggleDamageable() => canBeDamaged = !canBeDamaged;

    [System.Serializable]
    public struct SaveData
    {
        public int health;
    }

    public SaveData GetSaveData()
    {
        SaveData saveData;

        saveData.health = health;

        return saveData;
    }

    public void LoadSaveData(SaveData saveData)
    {
        health = saveData.health;

        UpdateUI();
    }
}
