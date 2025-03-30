using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Enemy Settings")]
    public float health = 50f; // Здоровье врага
    public float speed = 2f;   // Скорость перемещения врага
    public float attackRange = 1.5f; // Радиус атаки
    public float attackCooldown = 2f; // Время между атаками
    public int attackDamage = 10; // Урон врага
    public Transform target;   // Цель, например, игрок

    private bool isDead = false;
    private float lastAttackTime = 0f;

    public delegate void EnemyDeathHandler(EnemyController enemy);
    public static event EnemyDeathHandler OnEnemyDeath;

    void Update()
    {
        if (isDead || target == null) return;

        float distanceToTarget = Vector2.Distance(transform.position, target.position);

        if (distanceToTarget <= attackRange)
        {
            Attack();
        }
        else
        {
            MoveTowardsTarget();
        }
    }

    void MoveTowardsTarget()
    {
        if (target == null) return;

        // Направление движения
        Vector3 direction = (target.position - transform.position).normalized;

        // Проверка на наличие стены перед врагом
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, speed * Time.deltaTime, LayerMask.GetMask("Wall"));
        if (hit.collider == null)
        {
            // Движение в сторону цели
            transform.position += direction * speed * Time.deltaTime;
        }
    }

    void Attack()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            // Логика атаки
            lastAttackTime = Time.time;
            Debug.Log("Враг атакует!");

            // Проверяем наличие компонента здоровья у цели
            if (target.TryGetComponent<PlayerController>(out PlayerController player))
            {
                player.TakeDamage(attackDamage); // Наносим урон игроку
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return; // Если враг уже мертв, игнорируем урон

        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        Debug.Log("Враг погиб!");
        Destroy(gameObject); // Уничтожить объект врага
        if (isDead) return;

        isDead = true;

        // Уведомляем о смерти
        OnEnemyDeath?.Invoke(this);


        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        // Отображение радиуса атаки в редакторе
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    void Start()
    {
    }
}