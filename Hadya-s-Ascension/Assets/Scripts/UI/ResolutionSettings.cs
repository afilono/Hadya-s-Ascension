using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionSettings : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Dropdown resolutionDropdown;
    
    private Resolution[] resolutions;
    private List<Resolution> filteredResolutions;
    private int currentResolutionIndex = 0;
    
    void Start()
    {
        // Получаем все доступные разрешения
        resolutions = Screen.resolutions;
        filteredResolutions = new List<Resolution>();
        
        // Очищаем дропдаун
        resolutionDropdown.ClearOptions();
        
        // Создаем HashSet для отслеживания уникальных разрешений
        HashSet<string> resolutionSet = new HashSet<string>();
        
        // Добавляем уникальные разрешения (без учета частоты обновления)
        for (int i = 0; i < resolutions.Length; i++)
        {
            string resolutionString = $"{resolutions[i].width}x{resolutions[i].height}";
            
            if (!resolutionSet.Contains(resolutionString))
            {
                resolutionSet.Add(resolutionString);
                filteredResolutions.Add(resolutions[i]);
            }
        }
        
        List<string> options = new List<string>();
        
        // Заполняем дропдаун и находим текущее разрешение
        for (int i = 0; i < filteredResolutions.Count; i++)
        {
            string option = filteredResolutions[i].width + " x " + filteredResolutions[i].height;
            options.Add(option);
            
            if (filteredResolutions[i].width == Screen.width && 
                filteredResolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }
        
        // Добавление вариантов в dropdown
        resolutionDropdown.AddOptions(options);
        
        // Установка текущего разрешения
        if (options.Count > 0) 
        {
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
        }
        
        // Загрузка сохраненного разрешения
        LoadSettings();
        
        // Подписка на изменение значения dropdown
        resolutionDropdown.onValueChanged.AddListener(SetResolution);
        
        // Отладочная информация
        Debug.Log($"Доступно {filteredResolutions.Count} разрешений");
    }
    
    public void SetResolution(int resolutionIndex)
    {
        if (resolutionIndex >= 0 && resolutionIndex < filteredResolutions.Count) 
        {
            Resolution resolution = filteredResolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
            
            // Сохранение настройки
            PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);
            PlayerPrefs.Save();
            
            Debug.Log($"Установлено разрешение: {resolution.width}x{resolution.height}");
        }
    }
    
    void LoadSettings()
    {
        if (PlayerPrefs.HasKey("ResolutionIndex"))
        {
            int savedResIndex = PlayerPrefs.GetInt("ResolutionIndex");
            if (savedResIndex >= 0 && savedResIndex < filteredResolutions.Count)
            {
                resolutionDropdown.value = savedResIndex;
                SetResolution(savedResIndex);
            }
        }
    }
}
