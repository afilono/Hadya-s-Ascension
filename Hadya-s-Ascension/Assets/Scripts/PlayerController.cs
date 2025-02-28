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
        HandleMovementInput();

        // Передаем переменную для анимации
        if (animator != null)
        {
            animator.SetInteger("MovementDirection", movementDirection);
        }

        // Атака снарядом
        if (Input.GetMouseButtonDown(0) && Time.time >= lastFireTime + fireCooldown)
        {
            FireProjectile();
            lastFireTime = Time.time;
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
        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogError("Префаб снаряда или точка выстрела не назначены!");
            return;
        }

        // Направление выстрела (в направлении движения игрока)
        Vector2 direction = GetStrictDirection(movement);

        // Если игрок не двигается, стреляем вверх по умолчанию
        if (direction == Vector2.zero)
        {
            direction = Vector2.up;
        }

        // Создаем снаряд
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        projectile.GetComponent<Projectile>().SetDirection(direction);
        projectile.GetComponent<Projectile>().speed = projectileSpeed;

        Debug.Log("Снаряд создан!");
    }

    void HandleMovementInput()
    {
        // Сбрасываем движение
        movement = Vector2.zero;
        movementDirection = 0; // Сбрасываем направление для анимации

        // Проверяем нажатие клавиш и обновляем направление
        foreach (var key in new[] { KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D })
        {
            if (Input.GetKey(key))
            {
                switch (key)
                {
                    case KeyCode.W:
                        movement = Vector2.up;
                        movementDirection = 1; // Вперёд
                        break;
                    case KeyCode.S:
                        movement = Vector2.down;
                        movementDirection = 2; // Назад
                        break;
                    case KeyCode.A:
                        movement = Vector2.left;
                        movementDirection = 3; // Влево
                        break;
                    case KeyCode.D:
                        movement = Vector2.right;
                        movementDirection = 4; // Вправо
                        break;
                }
                break; // Прерываем цикл, если найдено направление
            }
        }
    }

    Vector2 GetStrictDirection(Vector2 inputDirection)
    {
        // Используем switch для определения строгого направления
        switch (inputDirection)
        {
            case Vector2 v when v == Vector2.up:
                return Vector2.up;
            case Vector2 v when v == Vector2.down:
                return Vector2.down;
            case Vector2 v when v == Vector2.left:
                return Vector2.left;
            case Vector2 v when v == Vector2.right:
                return Vector2.right;
            default:
                return Vector2.zero;
        }
    }
}