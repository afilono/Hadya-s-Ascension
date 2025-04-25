using UnityEngine;

public class EnemyController : MonoBehaviour, IDamageable
{
    [Header("Enemy Settings")]
    [SerializeField] private float maxHealth = 50f;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private Transform target;

    private float currentHealth;
    private bool isDead = false;
    private float lastAttackTime = 0f;

    public float Health => currentHealth;

    public delegate void EnemyDeathHandler(EnemyController enemy);
    public static event EnemyDeathHandler OnEnemyDeath;

    void Start()
    {
        currentHealth = maxHealth;
        
        // Если цель не задана, ищем игрока
        if (target == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                target = playerObject.transform;
            }
        }
    }

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
            lastAttackTime = Time.time;
            Debug.Log("Враг атакует!");

            // Проверяем наличие компонента IDamageable или HealthSystem у цели
            IDamageable damageable = target.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(attackDamage);
            }
            else
            {
                // Если интерфейс не найден, можно попробовать найти HealthSystem напрямую
                HealthSystem healthSystem = target.GetComponent<HealthSystem>();
                if (healthSystem != null)
                {
                    healthSystem.TakeDamage(attackDamage);
                }
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        Debug.Log("Враг погиб!");
        
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
}
