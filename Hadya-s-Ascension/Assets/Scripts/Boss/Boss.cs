using UnityEngine;

public class Boss : MonoBehaviour
{
    public Transform Player; // ������ �� ������
    public int maxHealth = 100; // ������������ �������� �����
    public int currentHealth; // ������� �������� �����
    public int phase2HealthThreshold = 50; // ����� �������� ��� �������� �� ������ ����

    private Animator animator; // ������ �� ��������
    private bool isPhase2 = false; // ���� ������ ����
    public bool isFlipped = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        // ������������� ������� ������, ���� �� �� ��������
        if (Player == null)
        {
            Player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void Update()
    {
        // ��������� �������� ��� �������� �� ������ ����
        if (!isPhase2 && currentHealth <= phase2HealthThreshold)
        {
            isPhase2 = true;
            animator.SetTrigger("HpDrain"); // ������� �� ������ ����
            Debug.Log("���� ������� �� ������ ����!");
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0); // �������� �� ����� ���� ������ 0

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("���� �����!");
        // ����� ����� �������� ������ ������ ����� (��������, �������� ��� ���������� ������)
        Destroy(gameObject);
    }


    public void LookAtPlayer()
    {
        if (Player == null) return; // ���������, ��� ����� ��������

        // �������� ������� ������� �������
        Vector3 scale = transform.localScale;

        // ���� ����� ��������� ����� �� ����� � ���� �� ������
        if (Player.position.x < transform.position.x && isFlipped)
        {
            scale.x = Mathf.Abs(scale.x); // �������� ������ �� �����������
            isFlipped = false;
        }
        // ���� ����� ��������� ������ �� ����� � ���� ������
        else if (Player.position.x > transform.position.x && !isFlipped)
        {
            scale.x = -Mathf.Abs(scale.x); // ���������� ������ � �������� ���������
            isFlipped = true;
        }

        // ��������� ����� �������
        transform.localScale = scale;
    }
}