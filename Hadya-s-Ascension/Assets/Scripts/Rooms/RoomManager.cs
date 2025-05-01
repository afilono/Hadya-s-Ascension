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
    [SerializeField] private bool refreshEnemiesOnUpdate = false;
    
    [Header("Враги")]
    [SerializeField] private List<EnemyController> roomEnemies = new List<EnemyController>();
    [SerializeField] private bool autoDetectEnemiesInBounds = true;
    
    // События комнаты
    public event Action OnPlayerEnterRoom;
    public event Action OnPlayerExitRoom;
    public event Action OnDoorsOpened;
    public event Action OnDoorsClosed;
    public event Action OnAllEnemiesDefeated;
    
    private EnemyController[] enemies;
    private bool isPlayerInside = false;
    private bool areDoorsLocked = false;
    private Transform playerTransform;
    private Door lastEnteredDoor;
    
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
        
        // Подписываемся на события уничтожения врагов
        EnemyController.OnEnemyDeath += CheckEnemiesStatus;
        
        // Обновляем список врагов при старте
        RefreshEnemiesList();
    }
    
    void Update()
    {
        // По необходимости обновляем список врагов
        if (refreshEnemiesOnUpdate)
        {
            RefreshEnemiesList();
        }
        
        // Проверяем статус врагов в комнате
        if (isPlayerInside && areDoorsLocked)
        {
            if (AreAllEnemiesDefeated())
            {
                OpenDoors();
            }
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
            OnPlayerEnterRoom?.Invoke();
            
            // Определяем, через какую дверь зашел игрок
            if (doors.Length > 0)
            {
                lastEnteredDoor = FindNearestDoor(other.transform.position);
            }
            
            // Обновляем список врагов при входе игрока
            RefreshEnemiesList();
            
            // Если в комнате есть враги и нужно закрывать двери при входе
            if (lockRoomOnEnter && !AreAllEnemiesDefeated())
            {
                StartCoroutine(WaitAndCheckDistance());
            }
        }
    }
    
    private System.Collections.IEnumerator WaitAndCheckDistance()
    {
        // Небольшая задержка, чтобы игрок успел войти
        yield return new WaitForSeconds(0.5f);
        
        if (isPlayerInside && playerTransform != null && lastEnteredDoor != null)
        {
            float distanceToEnteredDoor = Vector3.Distance(playerTransform.position, lastEnteredDoor.transform.position);
            
            // Если игрок отошел на безопасное расстояние от двери - закрываем двери
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
            areDoorsLocked = false;
            
            foreach (Door door in doors)
            {
                if (door != null)
                    door.OpenDoor();
            }
            
            OnDoorsOpened?.Invoke();
            
            if (AreAllEnemiesDefeated())
            {
                OnAllEnemiesDefeated?.Invoke();
            }
        }
    }
    
    private Door FindNearestDoor(Vector3 position)
    {
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
            RefreshEnemiesList();
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
            RefreshEnemiesList();
        }
    }
    
    private void RefreshEnemiesList()
    {
        // Если нужно автоматически определять врагов в границах комнаты
        if (autoDetectEnemiesInBounds)
        {
            Collider2D roomCollider = GetComponent<Collider2D>();
            if (roomCollider != null)
            {
                // Очищаем старый список, если он был заполнен автоматически
                if (roomEnemies == null)
                    roomEnemies = new List<EnemyController>();
                
                // Определяем границы комнаты
                Bounds roomBounds = roomCollider.bounds;
                
                // Находим врагов только если список пуст или нужно обновлять
                if (roomEnemies.Count == 0 || refreshEnemiesOnUpdate)
                {
                    // Добавляем врагов из сцены, которые находятся в границах комнаты
                    if (refreshEnemiesOnUpdate)
                        roomEnemies.Clear();
                    
                    EnemyController[] allEnemies = FindObjectsOfType<EnemyController>();
                    foreach (EnemyController enemy in allEnemies)
                    {
                        if (enemy != null && roomBounds.Contains(enemy.transform.position))
                        {
                            if (!roomEnemies.Contains(enemy))
                                roomEnemies.Add(enemy);
                        }
                    }
                }
            }
        }
        
        // Удаляем null-ссылки из списка
        roomEnemies.RemoveAll(e => e == null);
        
        // Преобразуем список в массив для более быстрого доступа
        enemies = roomEnemies.ToArray();
    }
    
    private bool AreAllEnemiesDefeated()
    {
        if (enemies == null || enemies.Length == 0)
            return true;
            
        foreach (EnemyController enemy in enemies)
        {
            if (enemy != null && enemy.Health > 0)
                return false;
        }
        
        return true;
    }
    
    private void CheckEnemiesStatus(EnemyController deadEnemy)
    {
        // Проверяем, входил ли этот враг в нашу комнату
        if (roomEnemies.Contains(deadEnemy))
        {
            // Удаляем мертвого врага из списка
            roomEnemies.Remove(deadEnemy);
            
            // Обновляем массив
            enemies = roomEnemies.ToArray();
            
            // Проверяем, не пора ли открыть двери
            if (isPlayerInside && areDoorsLocked && AreAllEnemiesDefeated())
            {
                OpenDoors();
            }
        }
    }
    
    private void OnDestroy()
    {
        // Отписываемся от события
        EnemyController.OnEnemyDeath -= CheckEnemiesStatus;
    }
}
