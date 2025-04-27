using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthBar : MonoBehaviour
{
    public Image fillImage;
    public Gradient gradient;
    private float maxHealth;
    private float currentHealth;

    /// <summary>
    /// Устанавливает максимальное здоровье и инициализирует текущий уровень здоровья
    /// </summary>
    public void SetMaxHealth(float health)
    {
        maxHealth = health;
        currentHealth = health;
        UpdateHealthBar(1f);
    }

    /// <summary>
    /// Плавно изменяет здоровье с помощью корутины
    /// </summary>
    public void SetHealth(float health)
    {
        StopAllCoroutines();
        StartCoroutine(ChangeHealthSmoothly(health));
    }

    private IEnumerator ChangeHealthSmoothly(float targetHealth)
    {
        float startHealth = currentHealth;
        float elapsed = 0f;
        float duration = 0.5f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            currentHealth = Mathf.Lerp(startHealth, targetHealth, elapsed / duration);
            UpdateHealthBar(currentHealth / maxHealth);
            yield return null;
        }

        currentHealth = targetHealth;
        UpdateHealthBar(currentHealth / maxHealth);
    }

    private void UpdateHealthBar(float healthPercentage)
    {
        fillImage.fillAmount = healthPercentage;
        fillImage.color = gradient.Evaluate(healthPercentage);
    }
}