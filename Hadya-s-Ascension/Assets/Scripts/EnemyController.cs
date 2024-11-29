using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float health = 50f;

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Враг погиб!");
        Destroy(gameObject); // Уничтожить объект врага
    }
}
