using UnityEngine;
using UnityEngine.UI;

public class HealthBarColorChanger : MonoBehaviour
{
    public Slider healthSlider; // Ссылка на UI Slider
    public Image fillImage; // Ссылка на Image, который заполняет полоску здоровья
    public Boss boss; // Ссылка на скрипт Boss
    public Gradient healthGradient; // Градиент для изменения цвета полоски здоровья

    void Start()
    {
        if (boss == null)
        {
            Debug.LogError("Boss reference is not set in HealthBarColorChanger.");
            return;
        }

        // Устанавливаем максимальное значение здоровья босса в Slider
        healthSlider.maxValue = boss.maxHealth;
        healthSlider.value = boss.currentHealth;

        // Устанавливаем начальный цвет полоски здоровья в соответствии с градиентом
        fillImage.color = healthGradient.Evaluate(1f); // 1f соответствует максимальному здоровью

        // Подписываемся на событие смерти босса
        Boss.OnBossDeath += HideHealthBar;
    }

    void Update()
    {
        if (boss != null)
        {
            // Обновляем значение Slider в зависимости от текущего здоровья босса
            healthSlider.value = boss.currentHealth;

            // Изменяем цвет полоски здоровья в соответствии с градиентом
            float healthPercentage = (float)boss.currentHealth / boss.maxHealth;
            fillImage.color = healthGradient.Evaluate(healthPercentage);
        }
    }

    // Метод для скрытия шкалы здоровья
    void HideHealthBar()
    {
        if (healthSlider != null)
        {
            healthSlider.gameObject.SetActive(false); // Отключаем Slider
        }
    }

    // Отписываемся от события при уничтожении объекта
    private void OnDestroy()
    {
        Boss.OnBossDeath -= HideHealthBar;
    }
}