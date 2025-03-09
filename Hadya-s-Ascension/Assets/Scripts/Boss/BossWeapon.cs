using UnityEngine;

public class BossWeapon : MonoBehaviour
{
    public int attackDamage = 20; // ���� ������� �����
    public int enragedAttackDamage = 40; // ���� ��������� �����

    public Vector3 attackOffset; // �������� �����
    public float attackRange = 1f; // ������ �����
    public LayerMask attackMask; // ���� ��� ����� (�����)

    public void Attack()
    {
        Vector3 pos = transform.position;
        pos += transform.right * attackOffset.x;
        pos += transform.up * attackOffset.y;

        // ���������, ���� �� ����� � ������� �����
        Collider2D colInfo = Physics2D.OverlapCircle(pos, attackRange, attackMask);
        if (colInfo != null)
        {
            // ������� ���� ������
            if (colInfo.TryGetComponent<PlayerController>(out PlayerController player))
            {
                player.TakeDamage(attackDamage);
                Debug.Log("���� ���� ���� ������!");
            }
        }
        else
        {
            Debug.Log("����� �� � ������� �����.");
        }
    }

    public void EnragedAttack()
    {
        Vector3 pos = transform.position;
        pos += transform.right * attackOffset.x;
        pos += transform.up * attackOffset.y;

        // ���������, ���� �� ����� � ������� �����
        Collider2D colInfo = Physics2D.OverlapCircle(pos, attackRange, attackMask);
        if (colInfo != null)
        {
            // ������� ���� ������
            if (colInfo.TryGetComponent<PlayerController>(out PlayerController player))
            {
                player.TakeDamage(enragedAttackDamage);
                Debug.Log("���� ���� ��������� ���� ������!");
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // ���������� ������ ����� � ���������
        Vector3 pos = transform.position;
        pos += transform.right * attackOffset.x;
        pos += transform.up * attackOffset.y;

        Gizmos.DrawWireSphere(pos, attackRange);
    }
}