using UnityEngine;
using UnityEngine.UI;

public class BossHealthUI : MonoBehaviour
{
    public Slider healthSlider; // —сылка на UI Slider
    public Boss boss; // —сылка на скрипт Boss

    void Start()
    {
        if (boss == null)
        {
            Debug.LogError("Boss reference is not set in BossHealthUI.");
            return;
        }

        // ”станавливаем максимальное значение здоровь€ босса в Slider
        healthSlider.maxValue = boss.maxHealth;
        healthSlider.value = boss.currentHealth;
    }

    void Update()
    {
        // ќбновл€ем значение Slider в зависимости от текущего здоровь€ босса
        if (boss != null)
        {
            healthSlider.value = boss.currentHealth;
        }
    }
}