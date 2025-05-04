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
    
    public event Action OnPlayerEnterRoom;
    public event Action OnPlayerExitRoom;
    public event Action OnDoorsOpened;
    public event Action OnDoorsClosed;
    public event Action OnAllEnemiesDefeated;
    
    private bool isPlayerInside = false;
    private bool areDoorsLocked = false;
    private bool wasRoomCleared = false;
    private Transform playerTransform;
    private Door lastEnteredDoor;
    private WaveSpawner waveSpawner;
    
    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerTransform = player.transform;
        
        if (doors == null || doors.Length == 0)
        {
            doors = GetComponentsInChildren<Door>();
        }
        
        roomEnemies.RemoveAll(e => e == null);
        
        foreach (Enemy enemy in roomEnemies)
        {
            if (enemy != null)
            {
                enemy.OnEnemyDeath += HandleEnemyDeath;
            }
        }
        
        waveSpawner = GetComponentInChildren<WaveSpawner>();
        if (waveSpawner == null)
        {
            waveSpawner = GetComponent<WaveSpawner>();
        }
        
        DisableEnemies();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
            EnableEnemies();
            
            if (doors.Length > 0)
            {
                lastEnteredDoor = FindNearestDoor(other.transform.position);
            }
            OnPlayerEnterRoom?.Invoke();
            
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
        if (lockRoomOnEnter && !wasRoomCleared && (roomEnemies.Count > 0 || hasWaves))
        {
            CloseDoors();
        }
    }
    
    private System.Collections.IEnumerator WaitAndCheckDistance()
    {
        yield return new WaitForSeconds(0.5f);
        
        if (isPlayerInside && playerTransform != null && lastEnteredDoor != null && !wasRoomCleared)
        {
            float distanceToEnteredDoor = Vector3.Distance(
                playerTransform.position, 
                lastEnteredDoor.transform.position
            );
            
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
            DisableEnemies();
            OnPlayerExitRoom?.Invoke();
        }
    }
    
    public void EnableEnemies()
    {
        foreach (Enemy enemy in roomEnemies)
        {
            if (enemy != null)
            {
                enemy.gameObject.SetActive(true);
            }
        }
    }
    
    public void DisableEnemies()
    {
        foreach (Enemy enemy in roomEnemies)
        {
            if (enemy != null)
            {
                enemy.gameObject.SetActive(false);
            }
        }
    }
    
    public void CloseDoors()
    {
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
            
            if (!isPlayerInside)
            {
                enemy.gameObject.SetActive(false);
            }
            
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
    
    public bool HasEnemies()
    {
        roomEnemies.RemoveAll(e => e == null);
        return roomEnemies.Count > 0;
    }
    
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
    
    private void HandleEnemyDeath(Enemy deadEnemy)
    {
        if (roomEnemies.Contains(deadEnemy))
        {
            roomEnemies.Remove(deadEnemy);
            Debug.Log($"Враг уничтожен. Осталось врагов: {roomEnemies.Count}");
            
            if (roomEnemies.Count == 0)
            {
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
