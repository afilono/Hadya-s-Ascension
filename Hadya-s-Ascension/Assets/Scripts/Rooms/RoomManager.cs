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
    [SerializeField] private List<Enemy> roomEnemies = new List<Enemy>();
    
    // События комнаты
    public event Action OnPlayerEnterRoom;
    public event Action OnPlayerExitRoom;
    public event Action OnDoorsOpened;
    public event Action OnDoorsClosed;
    public event Action OnAllEnemiesDefeated;
    
    private bool isPlayerInside = false;
    private bool areDoorsLocked = false;
    private bool wasRoomCleared = false; // Новый флаг для отслеживания, была ли комната очищена
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
        foreach (Enemy enemy in roomEnemies)
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
            
            // Проверяем была ли комната очищена ранее
            if (!wasRoomCleared)
            {
                StartCoroutine(WaitAndCheckDistance());
            }
        }
    }
    
    private System.Collections.IEnumerator DelayedRoomCheck()
    {
        yield return new WaitForSeconds(0.1f);
        bool hasWaves = waveSpawner != null && waveSpawner.HasWaves();
        // Не закрываем двери, если комната уже была очищена
        if (lockRoomOnEnter && !wasRoomCleared && (roomEnemies.Count > 0 || hasWaves))
        {
            CloseDoors();
        }
    }
    
    private System.Collections.IEnumerator WaitAndCheckDistance()
    {
        // Задержка, чтобы игрок успел войти в комнату
        yield return new WaitForSeconds(0.5f);
        
        // Не проверяем дистанцию, если комната уже очищена
        if (isPlayerInside && playerTransform != null && lastEnteredDoor != null && !wasRoomCleared)
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
        // Не закрываем двери, если комната уже была очищена
        if (!areDoorsLocked && !wasRoomCleared)
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
            
            // Отмечаем комнату как очищенную при открытии дверей после победы
            if (AreAllEnemiesDefeated())
            {
                wasRoomCleared = true;
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
    
    public void AddEnemy(Enemy enemy)
    {
        if (enemy != null && !roomEnemies.Contains(enemy))
        {
            roomEnemies.Add(enemy);
            enemy.OnEnemyDeath += HandleEnemyDeath;
            Debug.Log($"Враг добавлен в комнату. Всего врагов: {roomEnemies.Count}");
        }
    }
    
    public void RemoveEnemy(Enemy enemy)
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
        roomEnemies.RemoveAll(e => e == null);
        
        if (roomEnemies.Count == 0)
            return true;
            
        foreach (Enemy enemy in roomEnemies)
        {
            if (enemy != null && !enemy.IsDead() && enemy.Health > 0)
                return false;
        }
        
        return true;
    }
    
    /// <summary>
    /// Обработчик события смерти врага
    /// </summary>
    private void HandleEnemyDeath(Enemy deadEnemy)
    {
        if (roomEnemies.Contains(deadEnemy))
        {
            roomEnemies.Remove(deadEnemy);
            Debug.Log($"Враг уничтожен. Осталось врагов: {roomEnemies.Count}");
            
            if (roomEnemies.Count == 0)
            {
                // Отмечаем комнату как очищенную, если все враги побеждены
                if (waveSpawner == null || !waveSpawner.HasWaves())
                {
                    wasRoomCleared = true;
                    OnAllEnemiesDefeated?.Invoke();
                    OpenDoors();
                }
            }
        }
    }
    
    private void OnDestroy()
    {
        List<Enemy> enemiesCopy = new List<Enemy>(roomEnemies);
        
        foreach (Enemy enemy in enemiesCopy)
        {
            if (enemy != null)
            {
                enemy.OnEnemyDeath -= HandleEnemyDeath;
            }
        }
        
        roomEnemies.Clear();
    }
}
