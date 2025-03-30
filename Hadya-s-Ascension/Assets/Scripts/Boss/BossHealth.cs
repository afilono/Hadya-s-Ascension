using UnityEngine;
using UnityEngine.UI;

public class BossHealthUI : MonoBehaviour
{
    public Slider healthSlider;
    public Boss boss;

    void Start()
    {
        // Автопоиск, если не назначено
        if (boss == null) boss = FindObjectOfType<Boss>();
        if (healthSlider == null) healthSlider = GetComponentInChildren<Slider>();

        if (boss == null || healthSlider == null)
        {
            Debug.LogError("Boss or HealthSlider not assigned!");
            enabled = false;
            return;
        }

        healthSlider.maxValue = boss.maxHealth;
        healthSlider.value = boss.currentHealth;
    }

    void Update()
    {
        if (boss == null || healthSlider == null)
        {
            enabled = false;
            return;
        }
        healthSlider.value = boss.currentHealth;
        if (boss.currentHealth < 0)
            HideHealthBar();
    }
    void HideHealthBar()
    {
        if (healthSlider != null)
        { 
            healthSlider.gameObject.SetActive(false);
        }
    }
    private void OnDestroy()
    {
    }
}