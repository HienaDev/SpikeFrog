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

    [Header("[Regain Control Dialog Trigger]")]
    [SerializeField] private DialogTrigger dialogTrigger;

    private LeonController  leonController;

    void Start()
    {
        leonController = GetComponent<LeonController>();

        SetMaxHealth();
    }

    public void EnableHealthBar()
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
        else if (damage == 50)
        {
            leonController.SetStunned();
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
        healthBar.SetActive(false);
        leonController.DeactivateControllerModel();
        dialogTrigger.TriggerDialog();
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