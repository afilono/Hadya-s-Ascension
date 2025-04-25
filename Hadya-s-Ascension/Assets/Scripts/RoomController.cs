using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public Transform door1;
    public Transform door2;
    public Vector3 closedPosition1;
    public Vector3 openPosition1;
    public Vector3 closePosition2;
    public Vector3 openPosition2;
    
    [Header("Room Settings")]
    [SerializeField] private bool refreshEnemiesOnUpdate = false;
    private EnemyController[] enemies;
    private bool isPlayerInside = false;

    void Start()
    {
        door1.position = openPosition1;
        door2.position = openPosition2;
        RefreshEnemiesList();
    }

    void Update()
    {
        // При необходимости обновляем список врагов (опционально)
        if (refreshEnemiesOnUpdate)
        {
            RefreshEnemiesList();
        }
        
        // Проверяем, есть ли враги в комнате
        if (isPlayerInside)
        {
            if (AreAllEnemiesDefeated())
            {
                OpenDoor();
            }
            else
            {
                CloseDoor();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
            // Обновляем список врагов при входе игрока
            RefreshEnemiesList();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
        }
    }

    private void CloseDoor()
    {
        door1.position = closedPosition1;
        door2.position = closePosition2;
    }

    private void OpenDoor()
    {
        door1.position = openPosition1;
        door2.position = openPosition2;
    }
    
    private void RefreshEnemiesList()
    {
        enemies = FindEnemiesInRoom();
    }
    
    private EnemyController[] FindEnemiesInRoom()
    {
        // Найти всех врагов в пределах данной комнаты
        // Для более точного определения можно использовать коллайдер комнаты
        // и проверять, находятся ли враги внутри него
        
        // Простой вариант - найти всех врагов на сцене
        return FindObjectsOfType<EnemyController>();
    }
    
    private bool AreAllEnemiesDefeated()
    {
        bool allDefeated = true;
        
        // Проверяем все найденные экземпляры врагов
        foreach (EnemyController enemy in enemies)
        {
            // Проверяем существование объекта и его здоровье
            if (enemy != null && enemy.Health > 0)
            {
                allDefeated = false;
                break;
            }
        }
        
        return allDefeated;
    }
}
