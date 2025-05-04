using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class BossHealth : MonoBehaviour
{
    [Header("Компоненты UI")]
    public Image fillImage;
    public Gradient gradient;
    public TextMeshProUGUI healthText;
    
    [Header("Ссылка на босса")]
    public Boss boss;
    
    private float currentDisplayHealth;
    private Coroutine healthChangeCoroutine;

    void Start()
    {
        // Автопоиск босса, если не назначен
        if (boss == null)
        {
            boss = FindObjectOfType<Boss>();
            if (boss == null)
            {
                Debug.LogError("Ссылка на босса не найдена!");
                gameObject.SetActive(false);
                return;
            }
        }

        // Инициализация
        currentDisplayHealth = boss.Health;
        UpdateHealthBar(currentDisplayHealth / boss.MaxHealth);
        
        // Подписка на событие смерти босса
        boss.OnEnemyDeath += HandleBossDeath;
    }

    void Update()
    {
        if (boss != null && Mathf.Abs(boss.Health - currentDisplayHealth) > 0.01f)
        {
            // Запускаем плавное изменение здоровья
            if (healthChangeCoroutine != null)
            {
                StopCoroutine(healthChangeCoroutine);
            }
            healthChangeCoroutine = StartCoroutine(ChangeHealthSmoothly(boss.Health));
        }
    }

    private IEnumerator ChangeHealthSmoothly(float targetHealth)
    {
        float startHealth = currentDisplayHealth;
        float elapsed = 0f;
        float duration = 0.5f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            currentDisplayHealth = Mathf.Lerp(startHealth, targetHealth, elapsed / duration);
            UpdateHealthBar(currentDisplayHealth / boss.MaxHealth);
            yield return null;
        }

        currentDisplayHealth = targetHealth;
        UpdateHealthBar(currentDisplayHealth / boss.MaxHealth);
        healthChangeCoroutine = null;
    }

    private void UpdateHealthBar(float healthPercentage)
    {
        fillImage.fillAmount = healthPercentage;
        fillImage.color = gradient.Evaluate(healthPercentage);
        
        // Если есть текст для отображения значения здоровья
        if (healthText != null)
        {
            healthText.text = $"{Mathf.Ceil(currentDisplayHealth)}/{boss.MaxHealth}";
        }
    }

    private void HandleBossDeath(Enemy enemy)
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        boss.OnEnemyDeath  -= HandleBossDeath;
    }
}
