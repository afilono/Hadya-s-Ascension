using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 10;
    public float lifetime = 5f;
    public LayerMask whatIsSolid;
    public LayerMask playerLayer;
    
    [Header("Visual Effects")]
    [SerializeField] private GameObject hitEffect;
    
    private Vector2 moveDirection;
    private float timeSinceSpawn = 0f;
    private Rigidbody2D rb;

    void Awake()
    {
        // Получаем компонент Rigidbody2D, если он есть
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        // Игнорируем коллизии с игроком
        int projectileLayer = gameObject.layer;
        int playerLayerIndex = LayerMaskToLayerIndex(playerLayer);
        
        if (playerLayerIndex >= 0)
        {
            Physics2D.IgnoreLayerCollision(projectileLayer, playerLayerIndex, true);
        }
    }

    void Update()
    {
        // Если снаряд использует Rigidbody2D, движение обрабатывается через него
        if (rb == null)
        {
            // Если Rigidbody2D нет, двигаем через Transform
            transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
        }
        
        // Проверяем столкновения через райкаст
        CheckCollisions();
        
        // Уничтожаем снаряд по истечении времени жизни
        timeSinceSpawn += Time.deltaTime;
        if (timeSinceSpawn >= lifetime)
        {
            Destroy(gameObject);
        }
    }
    
    // Устанавливает направление движения снаряда
    public void SetDirection(Vector2 direction)
    {
        moveDirection = direction.normalized;
        
        // Если есть Rigidbody2D, используем его для движения
        if (rb != null)
        {
            // Используем velocity вместо AddForce для постоянной скорости
            rb.velocity = moveDirection * speed;
            
            // Отключаем гравитацию для снаряда
            rb.gravityScale = 0;
        }
        
        // Поворачиваем снаряд в направлении движения
        if (moveDirection != Vector2.zero)
        {
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
    
    private void CheckCollisions()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, moveDirection, speed * Time.deltaTime, whatIsSolid);
        
        if (hit.collider != null)
        {
            HandleCollision(hit.collider, hit.point);
        }
    }
    
    private void HandleCollision(Collider2D collider, Vector2 hitPoint)
    {
        // Проверяем на компонент IDamageable
        IDamageable damageable = collider.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
        }
        else
        {
            // Альтернативные проверки на старые компоненты
            EnemyController enemy = collider.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
        
        // Создаем эффект при попадании
        if (hitEffect != null)
        {
            Instantiate(hitEffect, hitPoint, Quaternion.identity);
        }
        
        // Уничтожаем снаряд
        Destroy(gameObject);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Игнорируем игрока
        if (((1 << other.gameObject.layer) & playerLayer) != 0)
        {
            return;
        }
        
        // Обрабатываем столкновение через триггер
        HandleCollision(other, transform.position);
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Игнорируем игрока
        if (((1 << collision.gameObject.layer) & playerLayer) != 0)
        {
            return;
        }
        
        // Обрабатываем физическое столкновение
        HandleCollision(collision.collider, collision.GetContact(0).point);
    }

    // Вспомогательный метод для преобразования LayerMask в индекс слоя
    private int LayerMaskToLayerIndex(LayerMask mask)
    {
        int layerValue = mask.value;
        if (layerValue == 0) return -1;
        
        for (int i = 0; i < 32; i++)
        {
            if ((layerValue & (1 << i)) != 0)
                return i;
        }
        
        return -1;
    }
}
