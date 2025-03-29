using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Параметры игрока
    public float health = 100f; // 
    public float moveSpeed = 5f; // 

    [Header("Combat")]
    public GameObject projectilePrefab; // 
    public Transform firePoint; // 
    public float projectileSpeed = 10f; // 
    public float fireCooldown = 0.5f; // 

    [Header("Animation")]
    public Animator animator; // 

    private Rigidbody2D rb; // 
    private Vector2 movement; // 
    private float lastFireTime = 0f; // 

    [Header("Health System")]
    public HealthBar healthBar; // 

    public int movementDirection; // 

    void Start()
    {
        // 
        rb = GetComponent<Rigidbody2D>();

        // 
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(health);
        }
    }

    void Update()
    {
        // 
        HandleMovementInput();

        // 
        if (animator != null)
        {
            animator.SetInteger("MovementDirection", movementDirection);
        }

        // 
        if (Input.GetMouseButtonDown(0) && Time.time >= lastFireTime + fireCooldown)
        {
            FireProjectile();
            lastFireTime = Time.time;
        }
    }

    void FixedUpdate()
    {
        // 
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
        // 
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    void FireProjectile()
    {
        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogError("Префаб снаряда или точка выстрела не назначены!");
            return;
        }

        // 
        Vector2 direction = GetStrictDirection(movement);

        // 
        if (direction == Vector2.zero)
        {
            direction = Vector2.up;
        }

        // 
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        projectile.GetComponent<Projectile>().SetDirection(direction);
        projectile.GetComponent<Projectile>().speed = projectileSpeed;

        Debug.Log("Снаряд создан!");
    }

    void HandleMovementInput()
    {
        // 
        movement = Vector2.zero;
        movementDirection = 0; // 

        // 
        foreach (var key in new[] { KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D })
        {
            if (Input.GetKey(key))
            {
                switch (key)
                {
                    case KeyCode.W:
                        movement = Vector2.up;
                        movementDirection = 1; // 
                        break;
                    case KeyCode.S:
                        movement = Vector2.down;
                        movementDirection = 2; // 
                        break;
                    case KeyCode.A:
                        movement = Vector2.left;
                        movementDirection = 3; // 
                        break;
                    case KeyCode.D:
                        movement = Vector2.right;
                        movementDirection = 4; // 
                        break;
                }
                break; // 
            }
        }
    }

    Vector2 GetStrictDirection(Vector2 inputDirection)
    {
        // 
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