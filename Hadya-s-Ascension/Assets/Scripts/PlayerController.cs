using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Параметры игрока
    public float health = 100f; // Здоровье
    public float moveSpeed = 5f; // Скорость движения

    [Header("Combat")]
    public GameObject projectilePrefab; // Префаб снаряда
    public Transform firePoint; // Точка выстрела
    public float projectileSpeed = 10f; // Скорость снаряда
    public float fireCooldown = 0.5f; // Кулдаун между выстрелами

    [Header("Animation")]
    public Animator animator; // Ссылка на компонент Animator

    private Rigidbody2D rb; // Для управления физикой
    private Vector2 movement; // Направление движения
    private float lastFireTime = 0f; // Время последнего выстрела

    [Header("Health System")]
    public HealthBar healthBar; // Ссылка на полоску здоровья

    public int movementDirection; // 0 - стоит на месте, 1 - вперёд, 2 - назад, 3 - влево, 4 - вправо

    void Start()
    {
        // Инициализация Rigidbody
        rb = GetComponent<Rigidbody2D>();

        // Инициализация полоски здоровья
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(health);
        }
    }

    void Update()
    {
        // Обработка ввода с клавиатуры
        movement.x = Input.GetAxis("Horizontal");
        movement.y = Input.GetAxis("Vertical");

        // Определение направления движения
        if (movement.y > 0) 
        {
            movementDirection = 1; // Вперёд
        }
        else if (movement.y < 0)
        {
            movementDirection = 2; // Назад
        }
        else if (movement.x < 0)
        {
            movementDirection = 3; // Влево
        }
        else if (movement.x > 0)
        {
            movementDirection = 4; // Вправо
        }
        else
        {
            movementDirection = 0; // Стоит на месте
        }

        // Передаем переменную для анимации
        animator.SetInteger("MovementDirection", movementDirection);

        // Ограничиваем здоровье
        health = Mathf.Clamp(health, 0, 100);

        // Атака снарядом
        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= lastFireTime + fireCooldown)
        {
            FireProjectile();
            lastFireTime = Time.time; // Сбрасываем таймер кулдауна
        }
    }

    void FixedUpdate()
    {
        // Передвижение
        rb.velocity = movement * moveSpeed;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        // Обновляем полоску здоровья
        if (healthBar != null)
        {
            healthBar.SetHealth(health);
        }

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Игрок погиб!");
        // Перезапуск уровня
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    void FireProjectile()
    {
        if (projectilePrefab == null || firePoint == null) return;

        // Создаем снаряд
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        // Передаем направление движения в снаряд
        projectile.GetComponent<Projectile>().SetDirection(movementDirection);
    }
}
