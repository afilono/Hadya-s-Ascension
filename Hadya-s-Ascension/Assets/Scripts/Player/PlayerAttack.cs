using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public int attackDamage = 10; // ���� ������
    public float attackRange = 1f; // ������ �����
    public LayerMask enemyMask; // ���� ������ (�����)

    void Update()
    {
        // ��������� ������� ������ ����� (��������, ����� ������ ����)
        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }
    }

    void Attack()
    {
        // �������� ������� ����� (����� �������)
        Vector2 attackPos = (Vector2)transform.position + Vector2.right * transform.localScale.x;

        // ���������, ���� �� ���� � ������� �����
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPos, attackRange, enemyMask);
        foreach (Collider2D enemy in hitEnemies)
        {
            // ������� ���� �����
            if (enemy.TryGetComponent<Boss>(out Boss boss))
            {
                boss.TakeDamage(attackDamage);
                Debug.Log("����� ���� ���� �����!");
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // ���������� ������ ����� � ���������
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere((Vector2)transform.position + Vector2.right * transform.localScale.x, attackRange);
    }
}