using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveSpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemyType
    {
        public GameObject enemyPrefab;
        [Tooltip("Время задержки после спавна этого типа врага")]
        public float delayAfterSpawn = 0.5f;
    }

    [System.Serializable]
    public class Wave
    {
        public string waveName;
        [Tooltip("Враги, которые появятся в этой волне")]
        public EnemyType[] enemies;
        [Tooltip("Время до начала этой волны")]
        public float timeBeforeWave = 2f;
        [Tooltip("Время после завершения волны до начала следующей")]
        public float timeAfterWave = 3f;
    }

    [Header("Настройки волн")]
    [SerializeField] private Wave[] waves;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private bool waitForWaveClear = true;
    [SerializeField] private bool autoStart = true;

    [Header("Интеграция с комнатой")]
    [SerializeField] private RoomManager roomManager;

    private int currentWaveIndex = 0;
    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private bool isSpawning = false;
    private bool hasPlayerEnteredRoom = false;
    private Coroutine spawnCoroutine;
    private bool isWaitingForNextWave = false;
    private bool allWavesCompleted = false; // Новый флаг для отслеживания завершения всех волн

    private void Start()
    {
        // Находим RoomManager, если он не задан
        if (roomManager == null)
        {
            roomManager = GetComponentInParent<RoomManager>();
            if (roomManager == null)
            {
                roomManager = GetComponent<RoomManager>();
            }
        }

        if (roomManager != null)
        {
            roomManager.OnPlayerEnterRoom += HandlePlayerEnterRoom;
            roomManager.OnPlayerExitRoom += HandlePlayerExitRoom;
            roomManager.OnAllEnemiesDefeated += HandleAllEnemiesDefeated;
            
            Debug.Log("WaveSpawner подписался на события RoomManager");
        }
        else
        {
            Debug.LogError("RoomManager не найден для WaveSpawner!");
        }
    }

    private void HandlePlayerEnterRoom()
    {
        Debug.Log("Игрок вошел в комнату (WaveSpawner)");
        hasPlayerEnteredRoom = true;
        
        // Не начинаем спавн, если все волны уже были завершены
        if (autoStart && !isSpawning && HasWaves() && !allWavesCompleted)
        {
            StartWaveSpawning();
        }
    }

    private void HandlePlayerExitRoom()
    {
        hasPlayerEnteredRoom = false;
        
        // Останавливаем волны при выходе из комнаты
        if (isSpawning && spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            isSpawning = false;
        }
    }

    public bool HasWaves()
    {
        // Проверяем не только наличие волн, но и то, что они ещё не были завершены
        return waves != null && waves.Length > 0 && !allWavesCompleted;
    }

    public void StartWaveSpawning()
    {
        // Не начинаем спавн, если все волны уже были завершены
        if (!isSpawning && hasPlayerEnteredRoom && !allWavesCompleted)
        {
            Debug.Log("Начинаем спавн волн");
            isSpawning = true;
            currentWaveIndex = 0;
            spawnCoroutine = StartCoroutine(SpawnWave(currentWaveIndex));
        }
    }

    private IEnumerator SpawnWave(int waveIndex)
    {
        if (waveIndex >= waves.Length)
        {
            Debug.Log("Все волны завершены!");
            isSpawning = false;
            allWavesCompleted = true; // Отмечаем, что все волны завершены
            if (roomManager != null)
            {
                roomManager.OpenDoors();
            }
            yield break;
        }

        Wave currentWave = waves[waveIndex];
        Debug.Log($"Начало волны: {currentWave.waveName} (Индекс: {waveIndex})");

        yield return new WaitForSeconds(currentWave.timeBeforeWave);

        spawnedEnemies.Clear();

        foreach (EnemyType enemyType in currentWave.enemies)
        {
            if (!hasPlayerEnteredRoom) break;

            Transform spawnPoint = GetRandomSpawnPoint();
            GameObject enemy = Instantiate(enemyType.enemyPrefab, spawnPoint.position, Quaternion.identity);
            spawnedEnemies.Add(enemy);

            Enemy enemyComponent = enemy.GetComponent<Enemy>();
            if (enemyComponent != null && roomManager != null)
            {
                roomManager.AddEnemy(enemyComponent);
            }

            yield return new WaitForSeconds(enemyType.delayAfterSpawn);
        }

        if (!waitForWaveClear)
        {
            yield return new WaitForSeconds(currentWave.timeAfterWave);
            MoveToNextWave();
        }
        else
        {
            // Ждем уничтожения всех врагов
            StartCoroutine(WaitForWaveClear());
        }
    }

    private IEnumerator WaitForWaveClear()
    {
        while (spawnedEnemies.Count > 0)
        {
            spawnedEnemies.RemoveAll(e => e == null);
            yield return null;
        }
        yield return new WaitForSeconds(waves[currentWaveIndex].timeAfterWave);
        MoveToNextWave();
    }

    private void CheckWaveStatus()
    {
        // Очищаем список от уничтоженных врагов
        spawnedEnemies.RemoveAll(e => e == null);
        
        Debug.Log($"Проверка статуса волны {currentWaveIndex}, осталось врагов: {spawnedEnemies.Count}");
        
        if (spawnedEnemies.Count == 0 && !isWaitingForNextWave)
        {
            isWaitingForNextWave = true;
            Debug.Log($"Все враги волны {currentWaveIndex} уничтожены, переход к следующей...");
            StartCoroutine(WaitAndMoveToNextWave(waves[currentWaveIndex].timeAfterWave));
        }
    }

    private void HandleAllEnemiesDefeated()
    {
        Debug.Log("Получено событие OnAllEnemiesDefeated");
        
        if (waitForWaveClear && isSpawning && !isWaitingForNextWave)
        {
            CheckWaveStatus();
        }
    }

    private IEnumerator WaitAndMoveToNextWave(float delay)
    {
        Debug.Log($"Ожидание {delay} секунд перед следующей волной");
        yield return new WaitForSeconds(delay);
        
        isWaitingForNextWave = false;
        MoveToNextWave();
    }

    private void MoveToNextWave()
    {
        currentWaveIndex++;
        Debug.Log($"Переход к волне {currentWaveIndex}");
        
        if (currentWaveIndex < waves.Length && hasPlayerEnteredRoom)
        {
            spawnCoroutine = StartCoroutine(SpawnWave(currentWaveIndex));
        }
        else
        {
            Debug.Log("Все волны завершены!");
            isSpawning = false;
            allWavesCompleted = true; // Отмечаем, что все волны завершены
            
            // Открываем двери после завершения всех волн
            if (roomManager != null)
            {
                roomManager.OpenDoors();
            }
        }
    }

    private Transform GetRandomSpawnPoint()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("Не заданы точки спавна!");
            return transform;
        }
        
        return spawnPoints[Random.Range(0, spawnPoints.Length)];
    }

    private void OnDestroy()
    {
        if (roomManager != null)
        {
            roomManager.OnPlayerEnterRoom -= HandlePlayerEnterRoom;
            roomManager.OnPlayerExitRoom -= HandlePlayerExitRoom;
            roomManager.OnAllEnemiesDefeated -= HandleAllEnemiesDefeated;
        }
    }
}
