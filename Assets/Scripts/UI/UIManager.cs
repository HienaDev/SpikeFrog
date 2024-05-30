using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Slider   healthSlider;
    [SerializeField] private Gradient healthGradient;
    [SerializeField] private Image    healthFill;

    public void SetMaxHealth(int health)
    {
        healthSlider.maxValue = health;
        healthSlider.value = health;

        healthFill.color = healthGradient.Evaluate(1f);
    }
    public void SetHealth(int health)
    {
        healthSlider.value = health;

        healthFill.color = healthGradient.Evaluate(healthSlider.normalizedValue);
    }
}