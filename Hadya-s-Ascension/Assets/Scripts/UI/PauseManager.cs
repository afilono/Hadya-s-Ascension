using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject[] turnOffObject;
    
    [SerializeField] private Button continueButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private Button backFromSettingsButton;
    
    [Header("Settings")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private int mainMenuSceneIndex = 0;
    [SerializeField] private bool useSceneNameInsteadOfIndex = true;
    
    private bool isPaused = false;
    
    private void Start()
    {
        // Подписываемся на события кнопок
        continueButton.onClick.AddListener(ContinueGame);
        settingsButton.onClick.AddListener(OpenSettings);
        menuButton.onClick.AddListener(ReturnToMainMenu);
        backFromSettingsButton.onClick.AddListener(CloseSettings);
        
        // Скрываем панели при старте
        pausePanel.SetActive(false);
        settingsPanel.SetActive(false);
        
        // Убеждаемся, что время идёт нормально при старте
        Time.timeScale = 1f;
    }
    
    private void Update()
    {
        // Проверяем нажатие Escape для переключения состояния паузы
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Если открыты настройки, закрываем их и возвращаемся в главное меню паузы
            if (settingsPanel.activeSelf)
            {
                CloseSettings();
                return;
            }
            
            // Иначе переключаем состояние паузы
            TogglePause();
        }
    }
    
    private void TogglePause()
    {
        isPaused = !isPaused;
        
        if (isPaused)
        {
            // Ставим игру на паузу
            PauseGame();
        }
        else
        {
            // Возобновляем игру
            ResumeGame();
        }
    }
    
    private void PauseGame()
    {
        // Останавливаем время
        Time.timeScale = 0f;
        
        // Скрываем игровые элементы
        foreach (var obj in turnOffObject)
        {
            obj.SetActive(false);
        }
        
        // Показываем основную панель паузы, скрываем настройки
        pausePanel.SetActive(true);
        pauseMenu.SetActive(true);
        settingsPanel.SetActive(false);
        
        // Показываем курсор, если он был скрыт
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    
    private void ResumeGame()
    {
        // Возобновляем нормальное течение времени
        Time.timeScale = 1f;
        
        // Показываем игровые элементы
        foreach (var obj in turnOffObject)
        {
            obj.SetActive(true);
        }
        
        // Скрываем все меню паузы
        pausePanel.SetActive(false);
        settingsPanel.SetActive(false);
    }
    
    public void ContinueGame()
    {
        // Снимаем паузу
        isPaused = false;
        ResumeGame();
    }
    
    public void OpenSettings()
    {
        // Показываем панель настроек, скрываем основную панель паузы
        settingsPanel.SetActive(true);
        pauseMenu.SetActive(false);
    }
    
    public void CloseSettings()
    {
        // Скрываем панель настроек, показываем основную панель паузы
        settingsPanel.SetActive(false);
        pauseMenu.SetActive(true);
    }
    
    public void ReturnToMainMenu()
    {
        // Важно! Возобновляем время перед загрузкой новой сцены
        Time.timeScale = 1f;
        
        // Загружаем сцену главного меню
        if (useSceneNameInsteadOfIndex)
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
        else
        {
            SceneManager.LoadScene(mainMenuSceneIndex);
        }
    }
}
