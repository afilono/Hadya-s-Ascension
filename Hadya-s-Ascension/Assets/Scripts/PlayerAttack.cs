using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public int attackDamage = 10; // Урон игрока
    public float attackRange = 1f; // Радиус атаки
    public LayerMask enemyMask; // Слой врагов (босса)

    void Update()
    {
        // Проверяем нажатие кнопки атаки (например, левая кнопка мыши)
        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }
    }

    void Attack()
    {
        // Получаем позицию атаки (перед игроком)
        Vector2 attackPos = (Vector2)transform.position + Vector2.right * transform.localScale.x;

        // Проверяем, есть ли враг в радиусе атаки
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPos, attackRange, enemyMask);
        foreach (Collider2D enemy in hitEnemies)
        {
            // Наносим урон боссу
            if (enemy.TryGetComponent<Boss>(out Boss boss))
            {
                boss.TakeDamage(attackDamage);
                Debug.Log("Игрок нанёс урон боссу!");
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // Отображаем радиус атаки в редакторе
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere((Vector2)transform.position + Vector2.right * transform.localScale.x, attackRange);
    }
}