public interface IRoomState
{
    bool AreEnemiesDefeated();
    bool IsPlayerInRoom();
    void RegisterEnemy(EnemyController enemy);
    void UnregisterEnemy(EnemyController enemy);
}