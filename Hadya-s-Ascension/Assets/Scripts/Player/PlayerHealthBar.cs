using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Image fillImage;
    public Gradient healthGradient; // �������� ��� ��������� �����

    public void SetMaxHealth(float health)
    {
        slider.maxValue = health;
        slider.value = health;

        // ������������� ��������� ���� ������� ��������
        fillImage.color = healthGradient.Evaluate(1f);
    }

    public void SetHealth(float health)
    {
        slider.value = health;

        float healthPercentage = health / slider.maxValue;
        fillImage.color = healthGradient.Evaluate(healthPercentage);

        if (health <= 0)
        {
            slider.gameObject.SetActive(false);
        }
    }
}