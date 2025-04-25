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
        // ��� ������������� ��������� ������ ������ (�����������)
        if (refreshEnemiesOnUpdate)
        {
            RefreshEnemiesList();
        }
        
        // ���������, ���� �� ����� � �������
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
            // ��������� ������ ������ ��� ����� ������
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
        // ����� ���� ������ � �������� ������ �������
        // ��� ����� ������� ����������� ����� ������������ ��������� �������
        // � ���������, ��������� �� ����� ������ ����
        
        // ������� ������� - ����� ���� ������ �� �����
        return FindObjectsOfType<EnemyController>();
    }
    
    private bool AreAllEnemiesDefeated()
    {
        bool allDefeated = true;
        
        // ��������� ��� ��������� ���������� ������
        foreach (EnemyController enemy in enemies)
        {
            // ��������� ������������� ������� � ��� ��������
            if (enemy != null && enemy.Health > 0)
            {
                allDefeated = false;
                break;
            }
        }
        
        return allDefeated;
    }
}
