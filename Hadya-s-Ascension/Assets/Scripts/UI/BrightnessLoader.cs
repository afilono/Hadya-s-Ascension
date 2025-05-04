using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BrightnessLoader : MonoBehaviour
{
    [SerializeField] private Volume globalVolume;
    [Tooltip("Если true, компонент найдет Volume автоматически")]
    [SerializeField] private bool findVolumeAutomatically = true;
    
    [Tooltip("Ключ для PlayerPrefs, где хранится значение яркости")]
    [SerializeField] private string brightnessPrefsKey = "Brightness";
    
    [Tooltip("Значение яркости по умолчанию, если настройка не найдена")]
    [SerializeField] private float defaultBrightness = 0f;

    void Awake()
    {
        // Автоматический поиск Volume в сцене, если не указан
        if (globalVolume == null && findVolumeAutomatically)
        {
            globalVolume = FindObjectOfType<Volume>();
            
            if (globalVolume == null)
            {
                Debug.LogWarning("Global Volume не найден в сцене!");
                return;
            }
        }
        
        ApplySavedBrightness();
    }

    public void ApplySavedBrightness()
    {
        if (globalVolume == null) return;
        
        // Загружаем сохраненное значение яркости
        float savedBrightness = PlayerPrefs.GetFloat(brightnessPrefsKey, defaultBrightness);
        
        // Получаем компонент Color Adjustments из Volume
        if (globalVolume.profile.TryGet(out ColorAdjustments colorAdjustments))
        {
            // Применяем сохраненное значение яркости
            colorAdjustments.postExposure.Override(savedBrightness);
            Debug.Log($"Применена яркость: {savedBrightness}");
        }
        else
        {
            Debug.LogWarning("ColorAdjustments не найден в профиле Volume!");
        }
    }
    
    // Метод для ручного обновления яркости (может быть вызван из других скриптов)
    public void UpdateBrightness(float newBrightness)
    {
        if (globalVolume == null) return;
        
        PlayerPrefs.SetFloat(brightnessPrefsKey, newBrightness);
        PlayerPrefs.Save();
        
        ApplySavedBrightness();
    }
}
