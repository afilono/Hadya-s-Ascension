using UnityEngine;

public class VaseController : MonoBehaviour, IDamageable
{
    [Header("Настройки вазы")]
    [SerializeField] private float maxHealth = 20f;
    [SerializeField] private float currentHealth;
    [SerializeField] private GameObject breakEffect; // Эффект разрушения вазы
    
    [Header("Настройки выпадения бонуса")]
    [SerializeField] private GameObject healthPickupPrefab;
    [Range(0f, 1f)]
    [SerializeField] private float dropChance = 0.3f; // 30% шанс выпадения бонуса
    
    public float Health => currentHealth;
    
    private void Start()
    {
        currentHealth = maxHealth;
    }
    
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    public void Die()
    {
        // Создаем эффект разрушения, если он назначен
        if (breakEffect != null)
        {
            Instantiate(breakEffect, transform.position, Quaternion.identity);
        }
        
        // Проверяем, должен ли выпасть бонус здоровья
        if (Random.value <= dropChance && healthPickupPrefab != null)
        {
            Instantiate(healthPickupPrefab, transform.position, Quaternion.identity);
        }
        
        // Уничтожаем вазу
        Destroy(gameObject);
    }
}