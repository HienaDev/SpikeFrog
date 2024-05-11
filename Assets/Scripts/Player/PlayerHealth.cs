using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private UIManager  uiManager;
    [SerializeField] private int        maxHealth;

    private int health;

    void Start()
    {
        health = maxHealth;

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
    }
}
