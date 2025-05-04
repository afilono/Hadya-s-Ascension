using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BrightnessSettings : MonoBehaviour
{
    [SerializeField] private Slider brightnessSlider;
    [SerializeField] private Volume postProcessVolume;
    
    [Tooltip("Минимальное значение яркости (затемнение)")]
    [SerializeField] private float minBrightness = -2f;
    
    [Tooltip("Максимальное значение яркости (осветление)")]
    [SerializeField] private float maxBrightness = 2f;
    
    private ColorAdjustments colorAdjustments;
    
    private void Awake()
    {
        // Настройка слайдера с центром в нуле
        brightnessSlider.minValue = minBrightness;
        brightnessSlider.maxValue = maxBrightness;
    }
    
    private void Start()
    {
        // Получение компонента ColorAdjustments из Volume
        if (postProcessVolume.profile.TryGet(out colorAdjustments))
        {
            // Загрузка сохраненного значения яркости
            float savedBrightness = PlayerPrefs.GetFloat("Brightness", 0f);
            
            // Устанавливаем значение слайдера (в центре будет 0)
            brightnessSlider.value = savedBrightness;
            
            // Применяем начальное значение
            AdjustBrightness(savedBrightness);
        }
        else
        {
            Debug.LogWarning("ColorAdjustments не найден в профиле Volume!");
        }
        
        // Подписываемся на изменение слайдера
        brightnessSlider.onValueChanged.AddListener(AdjustBrightness);
    }

    private void OnDestroy()
    {
        brightnessSlider.onValueChanged.RemoveListener(AdjustBrightness);
    }

    public void AdjustBrightness(float value)
    {
        if (colorAdjustments != null)
        {
            colorAdjustments.postExposure.Override(value);
            
            PlayerPrefs.SetFloat("Brightness", value);
            PlayerPrefs.Save();
            
            Debug.Log($"Установлена яркость: {value}");
        }
    }
    
    // Метод для сброса яркости к нейтральному значению (0)
    public void ResetBrightness()
    {
        brightnessSlider.value = 0f;
    }
}
