using UnityEngine;
using UnityEngine.UI;

public class HealthBarColorChanger : MonoBehaviour
{
    public Slider healthSlider; // ������ �� UI Slider
    public Image fillImage; // ������ �� Image, ������� ��������� ������� ��������
    public Boss boss; // ������ �� ������ Boss

    public Color fullHealthColor = Color.green; // ���� ��� ������ ��������
    public Color lowHealthColor = Color.red; // ���� ��� ������ ��������

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

        // ������������� �� ������� ������ �����
        Boss.OnBossDeath += HideHealthBar;
    }

    void Update()
    {
        if (boss != null)
        {
            // ��������� �������� Slider � ����������� �� �������� �������� �����
            healthSlider.value = boss.currentHealth;

            // �������� ���� ������� �������� � ����������� �� �������� ��������
            float healthPercentage = (float)boss.currentHealth / boss.maxHealth;
            fillImage.color = Color.Lerp(lowHealthColor, fullHealthColor, healthPercentage);
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