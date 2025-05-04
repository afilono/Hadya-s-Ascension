using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public Button backButton;
    
    public GameObject mainMenuPanel;    // Ссылка на панель главного меню
    public GameObject settingsMenuPanel; // Ссылка на панель настроек

    void Start()
    {
        // Настраиваем кнопку "Назад"
        backButton.onClick.AddListener(BackToMainMenu);
    }

    void BackToMainMenu()
    {
        // Скрываем меню настроек и показываем главное меню
        settingsMenuPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}