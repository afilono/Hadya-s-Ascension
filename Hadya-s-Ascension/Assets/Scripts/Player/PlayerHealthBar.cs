using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider; // ������ �� UI Slider
    public Image fillImage; // ������ �� Image, ������� ��������� ������� ��������
    public Gradient gradient; // �������� ��� ��������� ����� ������� ��������

    public void SetMaxHealth(float health)
    {
        slider.maxValue = health; // ������������� ������������ �������� ��������
        slider.value = health; // ������������� ������� �������� ��������

        // ������������� ��������� ���� ������� �������� � ������������ � ����������
        fillImage.color = gradient.Evaluate(1f); // 1f ������������� ������������� ��������
    }

    public void SetHealth(float health)
    {
        slider.value = health; // ��������� ������� �������� ��������

        // ��������� ������� �������� � �������� ���� ������� � ������������ � ����������
        float healthPercentage = health / slider.maxValue;
        fillImage.color = gradient.Evaluate(healthPercentage);
    }
}