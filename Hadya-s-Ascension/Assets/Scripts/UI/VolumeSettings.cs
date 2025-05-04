using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider volumeSlider;
    private const string VOLUME_PARAM = "Volume";

    private void Start()
    {
        // Установка начального значения слайдера
        volumeSlider.value = PlayerPrefs.GetFloat("Volume", 1f);
        SetVolume(volumeSlider.value);
    }

    private void OnEnable()
    {
        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    private void OnDisable()
    {
        volumeSlider.onValueChanged.RemoveListener(SetVolume);
    }

    private void SetVolume(float sliderValue)
    {
        float mixerValue;
    
        // Проверка на нулевое значение слайдера
        if (sliderValue <= 0f)
        {
            mixerValue = -80f; // минимальное значение громкости в децибелах
        }
        else
        {
            // Преобразование линейного значения слайдера в логарифмическое для аудио
            mixerValue = Mathf.Log10(sliderValue) * 20;
        }
    
        audioMixer.SetFloat(VOLUME_PARAM, mixerValue);
    
        // Сохранение настройки
        PlayerPrefs.SetFloat("Volume", sliderValue);
        PlayerPrefs.Save();
    }

}