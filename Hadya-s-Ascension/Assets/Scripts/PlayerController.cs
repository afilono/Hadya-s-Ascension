using UnityEngine;


public class PlayerController : MonoBehaviour
{
    // Параметры игрока
    public float health = 100f; // Здоровье
    public float attackPower = 20f; // Сила атаки
    public float moveSpeed = 5f; // Скорость движения

    private Rigidbody2D rb; // Для управления физикой
    private Vector2 movement; // Направление движения

    void Start()
    {
        // Инициализация Rigidbody
        rb = GetComponent<Rigidbody2D>();

        // Инициализация полоски здоровья
        healthBar.SetMaxHealth(health);
    }
    // Ссылка на полоску здоровья
    public HealthBar healthBar; 


    public void TakeDamage(float damage)
    {
        health -= damage;
        healthBar.SetHealth(health);

        if (health <= 0)
        {
            Die();
        }
    }

    void Update()
    {
        // Обработка ввода с клавиатуры
        movement.x = Input.GetAxis("Horizontal");
        movement.y = Input.GetAxis("Vertical");

        // Ограничиваем здоровье
        health = Mathf.Clamp(health, 0, 100);

        // Атака
    if (Input.GetKeyDown(KeyCode.Space))
    {
        AttackNearbyEnemies();
    }
    }

    void FixedUpdate()
    {
        // Передвижение
        rb.linearVelocity = movement * moveSpeed;
    }


    // Метод нанесения урона врагу
    public void Attack(EnemyController enemy)
    {
        enemy.TakeDamage(attackPower);
    }

    // Метод смерти игрока
    void Die()
    {
        Debug.Log("Игрок погиб!");
        // Например, перезапустить уровень
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }



void AttackNearbyEnemies()
{
    float attackRange = 0.5f; // Дистанция атаки
    Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange);

    foreach (Collider2D enemyCollider in hitEnemies)
    {
        EnemyController enemy = enemyCollider.GetComponent<EnemyController>();
        if (enemy != null)
        {
            Attack(enemy);
        }
    }
}

// Рисование радиуса атаки в редакторе
void OnDrawGizmosSelected()
{
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(transform.position, 0.5f);
}

    
}

