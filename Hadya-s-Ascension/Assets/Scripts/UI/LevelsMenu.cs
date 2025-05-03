using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public class LevelButton
{
    public Button button;
    public int sceneIndex;
    public string levelName; // Опционально - название уровня
}

public class LevelsMenu : MonoBehaviour
{
    public List<LevelButton> levelButtons;
    public Button backButton;
    
    public GameObject mainMenuPanel;    // Ссылка на панель главного меню
    public GameObject levelsMenuPanel;  // Ссылка на панель меню уровней

    void Start()
    {
        // Настраиваем каждую кнопку уровня
        foreach (var levelButton in levelButtons)
        {
            int index = levelButton.sceneIndex;
            levelButton.button.onClick.AddListener(() => LoadLevel(index));
        }
        
        // Настраиваем кнопку "Назад"
        backButton.onClick.AddListener(BackToMainMenu);
    }

    void LoadLevel(int sceneIndex)
    {
        // Загружаем выбранный уровень
        SceneManager.LoadScene(sceneIndex);
    }

    void BackToMainMenu()
    {
        // Скрываем меню уровней и показываем главное меню
        levelsMenuPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}