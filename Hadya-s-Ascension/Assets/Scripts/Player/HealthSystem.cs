using UnityEngine;

public class HealthSystem : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    
    public HealthBar healthBar;
    
    public float Health => currentHealth;
    
    public event System.Action OnDeath;

    private void Start()
    {
        currentHealth = maxHealth;
        
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    public void AddHp(float health)
    {
        currentHealth += Mathf.Clamp(health, 0, maxHealth);

        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }
    }

    public void Die()
    {
        Debug.Log("Игрок погиб!");
        OnDeath?.Invoke();
    }
}