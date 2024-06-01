using UnityEngine;
using UnityEngine.UI;

public class LeonManager : MonoBehaviour
{
    [Header("[Leon Settings]")]
    [SerializeField] private int health = 500;

    [Header("[Health Bar UI Elements]")]
    [SerializeField] private GameObject healthBar;
    [SerializeField] private Slider     healthBarSlider;
    [SerializeField] private Image      healthBarFill;

    private LeonController  leonController;

    void Start()
    {
        leonController = GetComponent<LeonController>();

        SetMaxHealth();
    }

    private void OnEnable()
    {
        healthBar.SetActive(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            DestroyController();
        }
    }

    public void TakeDamage(int damage)
    {
        if (leonController.CurrentState == LeonState.Stunned)
        {
            health -= damage;

            UpdateHealthBar();

            Debug.Log("Leon took " + damage + " damage. Health: " + health);

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

    private void SetMaxHealth()
    {
        healthBarSlider.maxValue = health;
        healthBarSlider.value    = health;
    }

    private void UpdateHealthBar()
    {
        healthBarSlider.value = health;
    }

    private void DestroyController()
    {
        Debug.Log("DestroyController called");
        leonController.SetNotControlled();
        healthBar.SetActive(false);
    }

    public bool HaveTheControllerOnLeon => (health > 0);

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

        UpdateHealthBar();
    }
}