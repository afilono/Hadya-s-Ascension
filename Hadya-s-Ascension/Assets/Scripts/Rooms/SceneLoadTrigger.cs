using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadTrigger : MonoBehaviour
{
    // Индекс сцены, которую нужно загрузить
    public int sceneIndex;
    
    // Функция вызывается при входе другого объекта в триггер
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Проверяем, является ли вошедший объект игроком
        if (collision.CompareTag("Player"))
        {
            // Загружаем сцену по индексу
            SceneManager.LoadScene(sceneIndex);
        }
    }
    
    // Версия для 3D-коллайдеров
    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, является ли вошедший объект игроком
        if (other.CompareTag("Player"))
        {
            // Загружаем сцену по индексу
            SceneManager.LoadScene(sceneIndex);
        }
    }
}