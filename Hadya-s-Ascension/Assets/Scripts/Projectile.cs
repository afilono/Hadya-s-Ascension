using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed; // Скорость снаряда
    public int damage; // Урон снаряда
    public LayerMask whatIsSolid; // Слой для столкновений (стены и объекты)
    public LayerMask playerLayer; // Слой игрока

    private Vector2 moveDirection; // Направление движения снаряда

    private void Start()
    {
        // Активируем снаряд, чтобы он был видимым
        gameObject.SetActive(true);

        // Игнорируем коллизии с игроком
        Physics2D.IgnoreLayerCollision(gameObject.layer, playerLayer);
    }

    // Метод для установки направления снаряда
    public void SetDirection(Vector2 direction)
    {
        moveDirection = direction.normalized; // Нормализуем направление
    }

    private void Update()
    {
        // Перемещение снаряда
        transform.Translate(moveDirection * speed * Time.deltaTime);

        // Проверка столкновения с объектами
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, moveDirection, speed * Time.deltaTime, whatIsSolid);
        if (hitInfo.collider != null)
        {
            if (hitInfo.collider.CompareTag("EnemyController"))
            {
                hitInfo.collider.GetComponent<EnemyController>().TakeDamage(damage);
            }
            Destroy(gameObject); // Уничтожаем снаряд после столкновения
        }
    }
}