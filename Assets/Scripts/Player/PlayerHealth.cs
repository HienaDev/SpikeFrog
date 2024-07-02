using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;
    [SerializeField] private int       maxHealth;

    private PlayerMovement playerMovement;
    private PlayerCombat   playerCombat;
    private Grappling      playerGrappling;
    private Animator       animator;
    private int            health;
    private bool           dead;
    private bool           canBeDamaged;
    private PlayerSounds playerSounds;

    void Start()
    {
        health = maxHealth;

        playerMovement = GetComponent<PlayerMovement>();
        playerCombat = GetComponentInChildren<PlayerCombat>();
        playerGrappling = GetComponentInChildren<Grappling>();
        animator     = GetComponentInChildren<Animator>();

        dead = false;
        canBeDamaged = true;

        playerSounds = GetComponent<PlayerSounds>();

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
        {
            health = Mathf.Max(health - amount, 0);
            playerSounds.PlayPunchSound();
        }

        UpdateUI();

        if (health <= 0 && !dead)
        {
            playerMovement.enabled = false;
            playerCombat.enabled = false;
            playerGrappling.enabled = false;
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
