using UnityEngine;
using System.Collections.Generic;
using System;

public class RoomManager : MonoBehaviour
{
    [Header("Двери")]
    [SerializeField] private Door[] doors;
    [SerializeField] private float safeDistanceFromDoor = 2.5f;
    
    [Header("Настройки комнаты")]
    [SerializeField] private bool lockRoomOnEnter = true;
    
    [Header("Враги")]
    [SerializeField] private List<EnemyController> roomEnemies = new List<EnemyController>();
    
    // События комнаты
    public event Action OnPlayerEnterRoom;
    public event Action OnPlayerExitRoom;
    public event Action OnDoorsOpened;
    public event Action OnDoorsClosed;
    public event Action OnAllEnemiesDefeated;
    
    private bool isPlayerInside = false;
    private bool areDoorsLocked = false;
    private Transform playerTransform;
    private Door lastEnteredDoor;
    private WaveSpawner waveSpawner;
    
    void Start()
    {
        // Находим игрока
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerTransform = player.transform;
        
        // Находим двери, если они не назначены вручную
        if (doors == null || doors.Length == 0)
        {
            doors = GetComponentsInChildren<Door>();
        }
        
        // Очищаем список от null-ссылок
        roomEnemies.RemoveAll(e => e == null);
        
        // Подписываемся на события смерти для каждого врага
        foreach (EnemyController enemy in roomEnemies)
        {
            if (enemy != null)
            {
                enemy.OnEnemyDeath += HandleEnemyDeath;
            }
        }
        
        // Получаем WaveSpawner
        waveSpawner = GetComponentInChildren<WaveSpawner>();
        if (waveSpawner == null)
        {
            waveSpawner = GetComponent<WaveSpawner>();
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
            if (doors.Length > 0)
            {
                lastEnteredDoor = FindNearestDoor(other.transform.position);
            }
            OnPlayerEnterRoom?.Invoke();
            StartCoroutine(WaitAndCheckDistance());
        }
    }
    
    private System.Collections.IEnumerator DelayedRoomCheck()
    {
        yield return new WaitForSeconds(0.1f);
        bool hasWaves = waveSpawner != null && waveSpawner.HasWaves();
        if (lockRoomOnEnter && (roomEnemies.Count > 0 || hasWaves))
        {
            CloseDoors();
        }
    }
    
    private System.Collections.IEnumerator WaitAndCheckDistance()
    {
        // Задержка, чтобы игрок успел войти в комнату
        yield return new WaitForSeconds(0.5f);
        
        if (isPlayerInside && playerTransform != null && lastEnteredDoor != null)
        {
            float distanceToEnteredDoor = Vector3.Distance(
                playerTransform.position, 
                lastEnteredDoor.transform.position
            );
            
            // Закрываем двери, если игрок отошел на безопасное расстояние
            if (distanceToEnteredDoor > safeDistanceFromDoor)
            {
                CloseDoors();
            }
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
            OnPlayerExitRoom?.Invoke();
        }
    }
    
    public void CloseDoors()
    {
        if (!areDoorsLocked)
        {
            Debug.Log("Закрываем двери комнаты");
            areDoorsLocked = true;
            
            foreach (Door door in doors)
            {
                if (door != null)
                    door.CloseDoor();
            }
            
            OnDoorsClosed?.Invoke();
        }
    }
    
    public void OpenDoors()
    {
        if (areDoorsLocked)
        {
            Debug.Log("Открываем двери комнаты");
            areDoorsLocked = false;
            
            foreach (Door door in doors)
            {
                if (door != null)
                    door.OpenDoor();
            }
            
            OnDoorsOpened?.Invoke();
            
            // Вызываем событие о том, что все враги уничтожены
            if (AreAllEnemiesDefeated())
            {
                OnAllEnemiesDefeated?.Invoke();
            }
        }
    }
    
    private Door FindNearestDoor(Vector3 position)
    {
        if (doors.Length == 0) return null;
        
        Door nearest = doors[0];
        float minDistance = float.MaxValue;
        
        foreach (Door door in doors)
        {
            float distance = Vector3.Distance(position, door.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = door;
            }
        }
        
        return nearest;
    }
    
    /// <summary>
    /// Добавить врага в список врагов комнаты
    /// </summary>
    public void AddEnemy(EnemyController enemy)
    {
        if (enemy != null && !roomEnemies.Contains(enemy))
        {
            roomEnemies.Add(enemy);
            enemy.OnEnemyDeath += HandleEnemyDeath;
            Debug.Log($"Враг добавлен в комнату. Всего врагов: {roomEnemies.Count}");
        }
    }
    
    /// <summary>
    /// Удалить врага из списка врагов комнаты
    /// </summary>
    public void RemoveEnemy(EnemyController enemy)
    {
        if (enemy != null && roomEnemies.Contains(enemy))
        {
            roomEnemies.Remove(enemy);
            enemy.OnEnemyDeath -= HandleEnemyDeath;
            Debug.Log($"Враг удален из комнаты. Осталось: {roomEnemies.Count}");
        }
    }
    
    /// <summary>
    /// Проверить наличие врагов в комнате
    /// </summary>
    public bool HasEnemies()
    {
        // Очищаем null-ссылки
        roomEnemies.RemoveAll(e => e == null);
        return roomEnemies.Count > 0;
    }
    
    /// <summary>
    /// Проверить, все ли враги уничтожены
    /// </summary>
    public bool AreAllEnemiesDefeated()
    {
        // Очищаем null-ссылки
        roomEnemies.RemoveAll(e => e == null);
        
        if (roomEnemies.Count == 0)
            return true;
            
        foreach (EnemyController enemy in roomEnemies)
        {
            if (enemy != null && enemy.Health > 0)
                return false;
        }
        
        return true;
    }
    
    /// <summary>
    /// Обработчик события смерти врага
    /// </summary>
    private void HandleEnemyDeath(EnemyController deadEnemy)
    {
        if (roomEnemies.Contains(deadEnemy))
        {
            roomEnemies.Remove(deadEnemy);
            Debug.Log($"Враг уничтожен. Осталось врагов: {roomEnemies.Count}");
            
            // Вызываем событие, если все враги уничтожены
            if (roomEnemies.Count == 0)
            {
                Debug.Log("Все враги уничтожены, вызываем событие");
                OnAllEnemiesDefeated?.Invoke();
                if(waveSpawner == null)
                    OpenDoors();
            }
        }
    }
    
    private void OnDestroy()
    {
        // Безопасное отписывание от событий
        List<EnemyController> enemiesCopy = new List<EnemyController>(roomEnemies);
        
        foreach (EnemyController enemy in enemiesCopy)
        {
            if (enemy != null)
            {
                enemy.OnEnemyDeath -= HandleEnemyDeath;
            }
        }
        
        roomEnemies.Clear();
    }
}
