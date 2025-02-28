using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider; // Ссылка на слайдер здоровья
    public Vector3 offset; // Смещение полоски относительно врага

    // Установить максимальное здоровье
    public void SetMaxHealth(float maxHealth)
    {
        if (slider != null)
        {
            slider.maxValue = maxHealth;
            slider.value = maxHealth;
        }
    }

    // Обновить текущее здоровье
    public void SetHealth(float health)
    {
        if (slider != null)
        {
            slider.value = health;
        }
    }

    void Update()
    {
        // Полоска здоровья следует за врагом
        if (transform.parent != null)
        {
            transform.position = transform.parent.position + offset;
        }
    }
}