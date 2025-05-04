using UnityEngine;

public class SpiderWebProjectile : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private int damage;
    private float slowAmount;
    private float slowDuration;
    private Rigidbody2D rb;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
    
    public void Initialize(Vector2 shootDirection, float projectileSpeed, int projectileDamage, 
                          float webSlowAmount, float webSlowDuration)
    {
        direction = shootDirection.normalized;
        speed = projectileSpeed;
        damage = projectileDamage;
        slowAmount = webSlowAmount;
        slowDuration = webSlowDuration;
        
        // Установка правильной начальной ориентации
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
    
    void FixedUpdate()
    {
        rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Наносим урон игроку
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }
            else
            {
                HealthSystem healthSystem = other.GetComponent<HealthSystem>();
                if (healthSystem != null)
                {
                    healthSystem.TakeDamage(damage);
                }
            }
            
            // Применяем эффект замедления
            WebEffect webEffect = other.GetComponent<WebEffect>();
            if (webEffect == null)
            {
                webEffect = other.gameObject.AddComponent<WebEffect>();
            }
            webEffect.ApplySlow(slowAmount, slowDuration);
            
            Destroy(gameObject);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
