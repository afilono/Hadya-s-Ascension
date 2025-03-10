using UnityEngine;
using UnityEngine.UI;

public class BossHealthUI : MonoBehaviour
{
    public Slider healthSlider; // ������ �� UI Slider
    public Boss boss; // ������ �� ������ Boss

    void Start()
    {
        if (boss == null)
        {
            Debug.LogError("Boss reference is not set in BossHealthUI.");
            return;
        }

        // ������������� ������������ �������� �������� ����� � Slider
        healthSlider.maxValue = boss.maxHealth;
        healthSlider.value = boss.currentHealth;
    }

    void Update()
    {
        // ��������� �������� Slider � ����������� �� �������� �������� �����
        if (boss != null)
        {
            healthSlider.value = boss.currentHealth;
        }
    }
}