using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider; // Ссылка на UI Slider
    public Image fillImage; // Ссылка на Image, который заполняет полоску здоровья

    public void SetMaxHealth(float health)
    {
        slider.maxValue = health; // Устанавливаем максимальное значение здоровья
        slider.value = health; // Устанавливаем текущее значение здоровья
    }

    public void SetHealth(float health)
    {
        slider.value = health; // Обновляем текущее значение здоровья

        // Изменяем цвет полоски здоровья в зависимости от процента здоровья
        float healthPercentage = health / slider.maxValue;
        fillImage.color = Color.Lerp(Color.red, Color.green, healthPercentage);
    }
}