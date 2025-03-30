using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public Transform door1;
    public Transform door2;// ������ �� ������ ����� (Transform)
    public Vector3 closedPosition1; // ������� �������� �����
    public Vector3 openPosition1;
    public Vector3 closePosition2;
    public Vector3 openPosition2;
    public EnemyController[] enemies; // ������ ������ � �������
    private bool isPlayerInside = false;

    void Start()
    {
        door1.position = openPosition1;
        door2.position = openPosition2;
        enemies = enemyControllers();
    }

    void Update()
    {
        // ���������, ���� �� ����� � �������
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
            if (enemy.health > 0) // ���������, ��� �� ����
            {
                return false; // ���� ���� �� ���� ���� ���, ���������� false
            }
        }
        return true; // ��� ����� �����
    }
}
