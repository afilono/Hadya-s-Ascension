using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button playButton;
    public Button levelsButton;
    public Button settingsButton;
    public Button exitButton;

    public GameObject mainMenuPanel;      // Панель главного меню
    public GameObject levelsMenuPanel;    // Панель выбора уровней  
    public GameObject settingsMenuPanel;  // Панель настроек

    private int lastLevelIndex = 1; // Индекс последнего уровня (измените на нужный)

    void Start()
    {
        playButton.onClick.AddListener(PlayLastLevel);
        levelsButton.onClick.AddListener(OpenLevelsMenu);
        settingsButton.onClick.AddListener(OpenSettingsMenu);
        exitButton.onClick.AddListener(ExitGame);

        // Показываем главное меню при старте, скрываем остальные
        ShowMainMenu();
    }

    void PlayLastLevel()
    {
        // Загружаем последний доступный уровень
        SceneManager.LoadScene(lastLevelIndex);
    }

    void OpenLevelsMenu()
    {
        // Скрываем главное меню и показываем меню выбора уровней
        mainMenuPanel.SetActive(false);
        levelsMenuPanel.SetActive(true);
    }

    void OpenSettingsMenu()
    {
        // Скрываем главное меню и показываем меню настроек
        mainMenuPanel.SetActive(false);
        settingsMenuPanel.SetActive(true);
    }

    public void ShowMainMenu()
    {
        // Показываем главное меню, скрываем остальные
        mainMenuPanel.SetActive(true);
        levelsMenuPanel.SetActive(false);
        settingsMenuPanel.SetActive(false);
    }

    void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}