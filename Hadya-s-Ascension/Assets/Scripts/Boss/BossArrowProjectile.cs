using UnityEngine;

public class BossArrowProjectile : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private int damage;

    public void Initialize(Vector2 shootDirection, float projectileSpeed, int projectileDamage)
    {
        direction = shootDirection.normalized;
        speed = projectileSpeed;
        damage = projectileDamage;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Ищем компонент IDamageable вместо PlayerController
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }
            else
            {
                // Если интерфейс не найден, можно попробовать найти HealthSystem напрямую
                HealthSystem healthSystem = other.GetComponent<HealthSystem>();
                if (healthSystem != null)
                {
                    healthSystem.TakeDamage(damage);
                }
            }
            
            Destroy(gameObject);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            Destroy(gameObject);
        }
    }
}