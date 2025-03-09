using UnityEngine;

public class BossWeapon : MonoBehaviour
{
    public int attackDamage = 20; // Урон обычной атаки
    public int enragedAttackDamage = 40; // Урон усиленной атаки

    public Vector3 attackOffset; // Смещение атаки
    public float attackRange = 1f; // Радиус атаки
    public LayerMask attackMask; // Слой для атаки (игрок)

    public void Attack()
    {
        Vector3 pos = transform.position;
        pos += transform.right * attackOffset.x;
        pos += transform.up * attackOffset.y;

        // Проверяем, есть ли игрок в радиусе атаки
        Collider2D colInfo = Physics2D.OverlapCircle(pos, attackRange, attackMask);
        if (colInfo != null)
        {
            // Наносим урон игроку
            if (colInfo.TryGetComponent<PlayerController>(out PlayerController player))
            {
                player.TakeDamage(attackDamage);
                Debug.Log("Босс нанёс урон игроку!");
            }
        }
        else
        {
            Debug.Log("Игрок не в радиусе атаки.");
        }
    }

    public void EnragedAttack()
    {
        Vector3 pos = transform.position;
        pos += transform.right * attackOffset.x;
        pos += transform.up * attackOffset.y;

        // Проверяем, есть ли игрок в радиусе атаки
        Collider2D colInfo = Physics2D.OverlapCircle(pos, attackRange, attackMask);
        if (colInfo != null)
        {
            // Наносим урон игроку
            if (colInfo.TryGetComponent<PlayerController>(out PlayerController player))
            {
                player.TakeDamage(enragedAttackDamage);
                Debug.Log("Босс нанёс усиленный урон игроку!");
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // Отображаем радиус атаки в редакторе
        Vector3 pos = transform.position;
        pos += transform.right * attackOffset.x;
        pos += transform.up * attackOffset.y;

        Gizmos.DrawWireSphere(pos, attackRange);
    }
}