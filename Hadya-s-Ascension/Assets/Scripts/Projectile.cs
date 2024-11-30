using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage = 20f; // Урон, который наносит снаряд
    public float lifeTime = 2f; // Время жизни снаряда

    void Start()
    {
        // Уничтожаем снаряд через заданное время
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Проверяем, столкнулся ли снаряд с врагом
        EnemyController enemy = collision.GetComponent<EnemyController>();
        if (enemy != null)
        {
            // Наносим урон врагу
            enemy.TakeDamage(damage);
            Destroy(gameObject); // Уничтожаем снаряд после попадания
        }
    }
}
