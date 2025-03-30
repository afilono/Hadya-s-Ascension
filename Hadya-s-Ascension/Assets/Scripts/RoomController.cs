using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public Transform door1;
    public Transform door2;// Ссылка на объект двери (Transform)
    public Vector3 closedPosition1; // Позиция закрытой двери
    public Vector3 openPosition1;
    public Vector3 closePosition2;
    public Vector3 openPosition2;
    public EnemyController[] enemies; // Массив врагов в комнате
    private bool isPlayerInside = false;

    void Start()
    {
        door1.position = openPosition1;
        door2.position = openPosition2;
        enemies = enemyControllers();
    }

    void Update()
    {
        // Проверяем, есть ли враги в комнате
        if (isPlayerInside && AreAllEnemiesDefeated())
        {
            OpenDoor();
        }
        if (isPlayerInside && !AreAllEnemiesDefeated())
        {
            CloseDoor();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
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
    private EnemyController[] enemyControllers()
    {
        return Collider2D.FindObjectsOfType<EnemyController>();
    }
    private bool AreAllEnemiesDefeated()
    {
        foreach (EnemyController enemy in enemies)
        {
            if (enemy.health > 0) // Проверяем, жив ли враг
            {
                return false; // Если хотя бы один враг жив, возвращаем false
            }
        }
        return true; // Все враги убиты
    }
}
