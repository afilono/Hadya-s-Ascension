using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider; // Ссылка на UI Slider
    public Image fillImage; // Ссылка на Image, который заполняет полоску здоровья
    public Gradient gradient; // Градиент для изменения цвета полоски здоровья

    public void SetMaxHealth(float health)
    {
        slider.maxValue = health; // Устанавливаем максимальное значение здоровья
        slider.value = health; // Устанавливаем текущее значение здоровья

        // Устанавливаем начальный цвет полоски здоровья в соответствии с градиентом
        fillImage.color = gradient.Evaluate(1f); // 1f соответствует максимальному здоровью
    }

    public void SetHealth(float health)
    {
        slider.value = health; // Обновляем текущее значение здоровья

        // Вычисляем процент здоровья и изменяем цвет полоски в соответствии с градиентом
        float healthPercentage = health / slider.maxValue;
        fillImage.color = gradient.Evaluate(healthPercentage);
    }
}