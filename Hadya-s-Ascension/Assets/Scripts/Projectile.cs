using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed; // Скорость снаряда
    public int damage; // Урон снаряда
    public LayerMask whatIsSolid; // Слой для столкновений (стены и объекты)

    private Vector2 moveDirection; // Направление движения снаряда

    // Метод для установки направления снаряда
    public void SetDirection(int movementDirection)
    {
        // В зависимости от направления движения игрока задаём направление снаряда
        switch (movementDirection)
        {
            case 1: // Вперёд (вверх)
                moveDirection = Vector2.up;
                break;
            case 2: // Назад (вниз)
                moveDirection = Vector2.down;
                break;
            case 3: // Влево
                moveDirection = Vector2.left;
                break;
            case 4: // Вправо
                moveDirection = Vector2.right;
                break;
            default: // Если игрок не двигается (например, стоит на месте)
                moveDirection = Vector2.zero;
                break;
        }
    }

    private void Update()
    {
        // Проверка столкновения с объектами
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, moveDirection, Mathf.Infinity, whatIsSolid);
        if (hitInfo.collider != null)
        {
            if (hitInfo.collider.CompareTag("EnemyController"))
            {
                hitInfo.collider.GetComponent<EnemyController>().TakeDamage(damage);
            }
            Destroy(gameObject); // Уничтожаем снаряд после столкновения
        }

        // Перемещение снаряда
        transform.Translate(moveDirection * speed * Time.deltaTime);
    }
}
