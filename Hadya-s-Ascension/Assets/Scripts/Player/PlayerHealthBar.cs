using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider; // ������ �� UI Slider
    public Image fillImage; // ������ �� Image, ������� ��������� ������� ��������

    public void SetMaxHealth(float health)
    {
        slider.maxValue = health; // ������������� ������������ �������� ��������
        slider.value = health; // ������������� ������� �������� ��������
    }

    public void SetHealth(float health)
    {
        slider.value = health; // ��������� ������� �������� ��������

        // �������� ���� ������� �������� � ����������� �� �������� ��������
        float healthPercentage = health / slider.maxValue;
        fillImage.color = Color.Lerp(Color.red, Color.green, healthPercentage);
    }
}