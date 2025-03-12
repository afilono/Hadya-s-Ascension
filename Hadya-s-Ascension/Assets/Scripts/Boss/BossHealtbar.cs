using UnityEngine;
using UnityEngine.UI;

public class HealthBarColorChanger : MonoBehaviour
{
    public Slider healthSlider; // ������ �� UI Slider
    public Image fillImage; // ������ �� Image, ������� ��������� ������� ��������
    public Boss boss; // ������ �� ������ Boss
    public Gradient healthGradient; // �������� ��� ��������� ����� ������� ��������

    void Start()
    {
        if (boss == null)
        {
            Debug.LogError("Boss reference is not set in HealthBarColorChanger.");
            return;
        }

        // ������������� ������������ �������� �������� ����� � Slider
        healthSlider.maxValue = boss.maxHealth;
        healthSlider.value = boss.currentHealth;

        // ������������� ��������� ���� ������� �������� � ������������ � ����������
        fillImage.color = healthGradient.Evaluate(1f); // 1f ������������� ������������� ��������

        // ������������� �� ������� ������ �����
        Boss.OnBossDeath += HideHealthBar;
    }

    void Update()
    {
        if (boss != null)
        {
            // ��������� �������� Slider � ����������� �� �������� �������� �����
            healthSlider.value = boss.currentHealth;

            // �������� ���� ������� �������� � ������������ � ����������
            float healthPercentage = (float)boss.currentHealth / boss.maxHealth;
            fillImage.color = healthGradient.Evaluate(healthPercentage);
        }
    }

    // ����� ��� ������� ����� ��������
    void HideHealthBar()
    {
        if (healthSlider != null)
        {
            healthSlider.gameObject.SetActive(false); // ��������� Slider
        }
    }

    // ������������ �� ������� ��� ����������� �������
    private void OnDestroy()
    {
        Boss.OnBossDeath -= HideHealthBar;
    }
}